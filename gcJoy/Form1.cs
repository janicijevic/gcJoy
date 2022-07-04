using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Windows.Forms;
using vJoyInterfaceWrap;

namespace gcJoy
{
    public partial class Form1 : Form
    {
        Thread readThread;
        AutoResetEvent readStop = new AutoResetEvent(false);    // Signal for read to stop

        // Declaring jostick array and corresponding state objects
        static int joyCount = 2;          // If this is changed text and info boxes should be added
        public vJoy[] joysticks = new vJoy[joyCount];
        public vJoy.JoystickState[] iReports = new vJoy.JoystickState[joyCount];
        bool[] rumbles = { false, false };
        TextBox[] textBoxes = new TextBox[joyCount];
        TextBox[] infoBoxes = new TextBox[joyCount];

        ///Serial communication parmaters
        SerialPort _serialPort;
        string port = "";
        int baud = 19200;
        List<string> prevPortList;
        bool detecting = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxes[0] = textBox1;
            textBoxes[1] = textBox2;
            infoBoxes[0] = infoBox1;
            infoBoxes[1] = infoBox2;
            vJoyInit();
            portTimer.Start();
        }


        public void vJoyInit()
        {
            for (int i = 0; i < joyCount; i++)
            {
                uint id = (uint)i + 1;
                joysticks[i] = new vJoy();
                iReports[i] = new vJoy.JoystickState();

                // Get the state of the requested device
                VjdStat status = joysticks[i].GetVJDStatus(id);
                // Acquire the target
                if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joysticks[i].AcquireVJD(id))))
                {
                    SetText(infoBoxes[i], String.Format("Failed to acquire vJoy device number {0}.", id)); 
                    return;
                }
                else
                    SetText(infoBoxes[i], String.Format("Acquired: vJoy device number {0}.", id));

            }
        }

        // Read thread
        public void Read()
        {
            try
            {
                Console.WriteLine("In read thread");
                bool connected = false;
                SetText(infoLbl, "Connecting to Arduino...");
                while (_serialPort.IsOpen)
                {
                    if (!connected)
                    {
                        //Console.WriteLine("Connecting");
                        if (_serialPort.BytesToRead > 0)
                        {
                            string message = _serialPort.ReadLine();
                            SetText(textBox1, message);
                            Console.WriteLine(message);
                            if (message.Contains("ready"))
                            {
                                SetText(infoLbl, "Arduino connected.");
                                connected = true;
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Probing controllers");
                        for (int i = 0; i < joyCount; i++)
                        {
                            readJoy(i);
                        }
                    }

                    // Uslov da se zaustavi readThread
                    if (readStop.WaitOne(0))
                    {
                        Console.WriteLine("Signal to stop read thread");
                        break;
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("Caught exception in read thread: " + e.Message);
            }
            finally
            {
                if (_serialPort != null)
                {
                    Console.WriteLine("Closing serialPort");
                    _serialPort.Close();
                }
            }
        }

        private void readJoy(int i)
        {
            uint id = (uint)i + 1;
            _serialPort.WriteLine("get" + id.ToString() + (rumbles[i] ? "1" : "0") + "-");

            //Header with format: <controller id 2b><status 5b><parity bit 1b>
            //  status = 0 indicates controller is connected and the next 8 bytes are controller data + 1 checksum byte
            //  status = 1 indicates controller is currently not connected to port
            //  status = 2 indicates controller is disabled by driver
            // If error in header detected ignore next 9 bytes just in case? until i send a request again ili flush idk


            // Read header
            byte tmp = (byte)_serialPort.ReadByte();
            //Console.WriteLine(String.Format("Probing controller {0} ({1})", tmp >> 6, i));
            //byteToString(tmp);
            
            //if (!checkParity(tmp)) {Thread.Sleep(20); _serialPort.DiscardIn<or Out>Buffer(); return;}
            int stat = (tmp>>1) & 0b11111;
            int cid = (tmp >> 6);
            if (cid != i)
            {
                Console.WriteLine(String.Format("Probed {0} but got response from {1}", i, cid));
                return;
            }

            // Read data
            if (stat == 0)
            {

                byte[] data = new byte[8];
                byte checksum = 0;
                for (int x = 0; x < 8; x++)
                {
                    tmp = (byte)_serialPort.ReadByte();
                    data[x] = tmp;
                    //byteToString(tmp);
                    checksum ^= tmp;
                }
                tmp = (byte)_serialPort.ReadByte();
                if (tmp == checksum)
                {
                    // Checksum correct -> parse the message
                    parseMessage(data, i);
                    writeMessageToTextBox(data, i);
                    if (!joysticks[i].UpdateVJD(id, ref iReports[i]))
                    {
                        SetText(infoBoxes[i], String.Format("Failed to update joystick number {0}, reacquiring...", id));
                        joysticks[i].AcquireVJD(id);
                        //VjdStat _status = joysticks[i].GetVJDStatus(id);
                        //string mess;
                        //switch (status)
                        //{
                        //    case VjdStat.VJD_STAT_BUSY: mess = "busy"; break;
                        //    case VjdStat.VJD_STAT_FREE: mess = "free"; break;
                        //    case VjdStat.VJD_STAT_MISS: mess = "miss"; break;
                        //    case VjdStat.VJD_STAT_OWN: mess = "own"; break;
                        //    default: mess = "unknown"; break;
                        //}
                        //SetText(textBoxes[i], mess);
                    }
                }
                else
                {
                    // Checksum for data incorrect
                    // TODO: Implement some synchronization? with some bit stuffing
                }
            }
            else
            {
                // Controller disconnected or disabled
                if(stat == 1)
                    SetText(infoBoxes[i], String.Format("Controller {0} is disconnected", i));
                else if(stat == 2)
                    SetText(infoBoxes[i], String.Format("Controller {0} is disabled", i));
            }
            return;



        }

        public void writeMessageToTextBox(byte[] message, int i)
        {
            string res = "";
            for(int x = 0; x<8; x++)
            {
                for (int y = 7; y >= 0; y--)
                {
                    res += ((message[x] >> y) & 1) == 1 ? "1" : "0";
                }
            }
            SetText(textBoxes[i], res);
        }

        public void byteToString(byte tmp)
        {
            for (int i = 7; i >=0; i--)
            {
                Console.Write((tmp>>i)&1);
            }
            Console.WriteLine();

        }

        public void parseMessage(byte[] message, int i)
        {
            //Parse the message to get button states
            /*
            Byte 0    0	 0	0	Start	Y	X	B	A
            Byte 1	  1	 L	R	Z	D-Up	D-Down	D-Right	  D-Left
            Byte 2	  Joystick X Value (8 bit)
            Byte 3	  Joystick Y Value (8 bit)
            Byte 4	  C-Stick X Value (8 bit)
            Byte 5	  C-Stick Y Value (8 bit)
            Byte 6	  Left Button Value (8 bit) - may be 4-bit mode also?
            Byte 7	  Right Button Value (8 bit) - may be 4-bit mode also?
            */
            iReports[i].Buttons = parseButtons(message);
            iReports[i].AxisX = message[2];
            iReports[i].AxisY = message[3];
            iReports[i].AxisXRot = message[4];
            iReports[i].AxisYRot = message[5];
            iReports[i].Slider = message[6];
            iReports[i].Dial = message[7];
        }

        public uint parseButtons(byte[] message)
        {
            // Rearrange buttons for simpler mapping?
            //Button mapping: A, B, Start, X, Y, Z, L, R, Up, Down, Left, Right
            //                0  1  2      3  4  5  6  7  8   9     10    11

            uint res = 0;
            res |= (uint)message[0] & 1; // A
            res |= ((uint)message[0] >> 1 & 1) << 1; // B
            res |= ((uint)message[0] >> 4 & 1) << 2; // Start
            res |= ((uint)message[0] >> 2 & 1) << 3; // X
            res |= ((uint)message[0] >> 3 & 1) << 4; // Y
            res |= ((uint)message[1] >> 4 & 1) << 5; // Z
            res |= ((uint)message[1] >> 6 & 1) << 6; // L
            res |= ((uint)message[1] >> 5 & 1) << 7; // R
            res |= ((uint)message[1] >> 3 & 1) << 8; // Up
            res |= ((uint)message[1] >> 2 & 1) << 9; // Down
            res |= ((uint)message[1] >> 0 & 1) << 10; // Left
            res |= ((uint)message[1] >> 1 & 1) << 11; // Right

            return res;
        }


        // Old message parsing with string
        //public void parseMessage(string message, int i)
        //{
        //    //Parse the message to get button states
        //    /*
        //    Byte 0    0	 0	0	Start	Y	X	B	A
        //    Byte 1	  1	 L	R	Z	D-Up	D-Down	D-Right	  D-Left
        //    Byte 2	  Joystick X Value (8 bit)
        //    Byte 3	  Joystick Y Value (8 bit)
        //    Byte 4	  C-Stick X Value (8 bit)
        //    Byte 5	  C-Stick Y Value (8 bit)
        //    Byte 6	  Left Button Value (8 bit) - may be 4-bit mode also?
        //    Byte 7	  Right Button Value (8 bit) - may be 4-bit mode also?
        //    */
        //    iReports[i].Buttons = parseButtons(message.Substring(3, 13));
        //    iReports[i].AxisX = toInt(message.Substring(8 * 2, 8));     //Main joystick
        //    iReports[i].AxisY = toInt(message.Substring(8 * 3, 8));
        //    iReports[i].AxisXRot = toInt(message.Substring(8 * 4, 8));  //C stick
        //    iReports[i].AxisYRot = toInt(message.Substring(8 * 5, 8));
        //    iReports[i].Slider = toInt(message.Substring(8 * 6, 8));    //Triggers
        //    iReports[i].Dial = toInt(message.Substring(8 * 7, 8));
        //}
        //public int toInt(string s)
        //{
        //    //Maps gamecube axis range to VJoy axis range
        //    int val = Convert.ToInt32(s, 2);
        //    return (int)((val / 255.0) * 0x7FFF);
        //}
        //public uint parseButtons(string s)
        //{
        //    //String content: Start  Y  X  B  A  1  L  R  Z  D-Up  D - Down  D - Right  D - Left
        //    //Button mapping: A, B, Start, X, Y, Z, L, R, Up, Down, Left, Right
        //    //                0  1  2      3  4  5  6  7  8   9     10    11
        //    uint n = 0;
        //    n += (uint)(s[0] - '0') << 2; //Start
        //    n += (uint)(s[1] - '0') << 4; //Y
        //    n += (uint)(s[2] - '0') << 3; //X
        //    n += (uint)(s[3] - '0') << 1; //B
        //    n += (uint)(s[4] - '0') << 0; //A
        //    n += (uint)(s[6] - '0') << 6; //L
        //    n += (uint)(s[7] - '0') << 7; //R
        //    n += (uint)(s[8] - '0') << 5; //Z
        //    n += (uint)(s[9] - '0') << 8; //Dup
        //    n += (uint)(s[10] - '0') << 9; //Ddown
        //    n += (uint)(s[11] - '0') << 10; //Dright
        //    n += (uint)(s[12] - '0') << 11; //Dleft
        //    return n;
        //}


        private void stopReadThread()
        {
            readStop.Set();
        }

        delegate void SetTextCallback(Control ctrl, string text);

        public void SetText(Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { ctrl, text });
            }
            else ctrl.Text = text;
        }


        // ------------------------ Events ------------------------

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopReadThread();
        }

        private void rumbleCheck1_CheckedChanged(object sender, EventArgs e)
        {
            rumbles[0] = !rumbles[0];
        }

        private void rumbleCheck2_CheckedChanged(object sender, EventArgs e)
        {
            rumbles[1] = !rumbles[1];
        }

        private void detectBtn_Click(object sender, EventArgs e)
        {
            SetText(infoLbl, "Plug Arduino in...");
            //Save current portlist to compare it
            prevPortList = portList.Items.Cast<string>().ToList();
            detecting = true;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                SetText(infoLbl, "Disconnecting port...");
                stopReadThread();
                Thread.Sleep(5000);
                _serialPort.Close();
            }
            //Connect serial port
            Console.WriteLine("Start connecting");
            SetText(infoLbl, String.Format("Connecting to serial port on port {0}...", port));
            if (portList.Items.Count == 0)
            {
                port = "";
                SetText(infoLbl, "No Com port selected");
                return;
            }
            port = portList.SelectedItem.ToString();
            _serialPort = new SerialPort(port, baud);
            _serialPort.DtrEnable = true; // Reset Arduino on connect

            _serialPort.Open();

            readThread = new Thread(new ThreadStart(Read));
            readThread.Start();
        }


        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            //Disconnect serial port
            if (port == "") { SetText(infoLbl, "No Com port selected"); return; }
            SetText(infoLbl, "Disconnecting serial port...");
            stopReadThread();
            SetText(infoLbl, "Serial port disconnected.");

        }

        private void portTimer_Tick(object sender, EventArgs e)
        {
            //Update portList while keeping selected item
            int i = portList.SelectedIndex;
            string[] portNames = SerialPort.GetPortNames();
            portList.DataSource = portNames;
            if (i >= 0 && i < portNames.Length)
                portList.SetSelected(i, true);

            if (detecting)
            {
                //Compare current to saved portlist to select newly connected com port
                List<string> tmp = portNames.ToList();
                var items = tmp.Except(prevPortList).ToList();
                if (items.Count > 0)
                {
                    detecting = false;
                    port = items[0];
                    //Select new port
                    portList.SetSelected(tmp.IndexOf(port), true);
                    SetText(infoLbl, String.Format("Selected port {0}.", port));
                }
                prevPortList = tmp;

            }
        }

    }
}
