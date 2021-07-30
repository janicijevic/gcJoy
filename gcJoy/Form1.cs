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
        bool _continue, serialOpen = false;
        SerialPort _serialPort;
        Thread readThread;

        // Declaring jostick array and corresponding state objects
        static int joyCount = 2;
        public vJoy[] joysticks = new vJoy[joyCount];
        public vJoy.JoystickState[] iReports = new vJoy.JoystickState[joyCount];
        bool[] rumbles = { false, false };

        ///Serial communication parmaters
        string port = "COM4";
        int baud = 115200;
        List<string> prevPortList;
        bool detecting = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
                    if(id==1) SetText(infoBox1, String.Format("Failed to acquire vJoy device number {0}.", id)); 
                    else SetText(infoBox2, String.Format("Failed to acquire vJoy device number {0}.", id));
                    return;
                }
                else
                    if (id == 1) SetText(infoBox1, String.Format("Acquired: vJoy device number {0}.", id));
                else SetText(infoBox2, String.Format("Acquired: vJoy device number {0}.", id));

            }
        }

        public void Read()
        {
            SetText(infoLbl, "Connecting to Arduino...");
            while (_continue)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    string message = _serialPort.ReadLine();

                    if (message.Contains("ready"))
                    {
                        SetText(infoLbl, "Arduino connected.");
                        break;
                    }
                }
            }
            while (_continue)
            {
                //Unplugged arduino durring operation causes a crash !!
                //see stackoverflow for safeserialport class ?

                for (int i = 0; i < joyCount; i++)
                {
                    uint id = (uint)i + 1;
                    _serialPort.WriteLine("get" + id.ToString() + (rumbles[i] ? "1" : "0") + "-");

                    Thread.Sleep(10);
                    if (_serialPort.BytesToRead > 0)
                    {
                        int bytes = _serialPort.BytesToRead;
                        string message = _serialPort.ReadLine();
                        message = message.Substring(0, message.Length - 2);

                        //Show message recieved from arduino in textbox1
                        if (id == 1) SetText(textBox1, message);
                        else SetText(textBox2, message);

                        //Parse message
                        if (message.Length == 64 && message.All(c => c >= '0' && c <= '9'))
                        {
                            parseMessage(message, i);
                            if (!joysticks[i].UpdateVJD(id, ref iReports[i]))
                            {
                                if (id == 1) SetText(textBox1, String.Format("Failed to update joystick number {0}, reacquiring...", id));
                                else SetText(textBox2, String.Format("Failed to update joystick number {0}, reacquiring...", id));
                                joysticks[i].AcquireVJD(id);
                                VjdStat status = joysticks[i].GetVJDStatus(id);
                                //string mess;
                                //switch (status)
                                //{
                                //    case VjdStat.VJD_STAT_BUSY: mess = "busy"; break;
                                //    case VjdStat.VJD_STAT_FREE: mess = "free"; break;
                                //    case VjdStat.VJD_STAT_MISS: mess = "miss"; break;
                                //    case VjdStat.VJD_STAT_OWN: mess = "own"; break;
                                //    default: mess = "unknown"; break;
                                //}
                                //if (id == 1) SetText(textBox1, mess);
                                //else SetText(textBox2, mess);
                            }



                        }
                        else if (message == "disconnected") SetText(textBox1, "Controller disconnected");
                        else if (message == "disabled") SetText(textBox1, "Controller is disabled");
                        else SetText(textBox1, "Error");
                    }
                    else
                    {
                        SetText(textBox1, "No response");
                    }
                }

            }
        }

        public void parseMessage(string message, int i)
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
            iReports[i].Buttons = parseButtons(message.Substring(3, 13));
            iReports[i].AxisX = toInt(message.Substring(8 * 2, 8));     //Main joystick
            iReports[i].AxisY = toInt(message.Substring(8 * 3, 8));
            iReports[i].AxisXRot = toInt(message.Substring(8 * 4, 8));  //C stick
            iReports[i].AxisYRot = toInt(message.Substring(8 * 5, 8));
            iReports[i].Slider = toInt(message.Substring(8 * 6, 8));    //Triggers
            iReports[i].Dial = toInt(message.Substring(8 * 7, 8));
        }
        public int toInt(string s)
        {
            //Maps gamecube axis range to VJoy axis range
            int val = Convert.ToInt32(s, 2);
            return (int)((val / 255.0) * 0x7FFF);
        }
        public uint parseButtons(string s)
        {
            //String content: Start  Y  X  B  A  1  L  R  Z  D-Up  D - Down  D - Right  D - Left
            //Button mapping: A, B, Start, X, Y, Z, L, R, Up, Down, Left, Right
            //                0  1  2      3  4  5  6  7  8   9     10    11
            uint n = 0;
            n += (uint)(s[0] - '0') << 2; //Start
            n += (uint)(s[1] - '0') << 4; //Y
            n += (uint)(s[2] - '0') << 3; //X
            n += (uint)(s[3] - '0') << 1; //B
            n += (uint)(s[4] - '0') << 0; //A
            n += (uint)(s[6] - '0') << 6; //L
            n += (uint)(s[7] - '0') << 7; //R
            n += (uint)(s[8] - '0') << 5; //Z
            n += (uint)(s[9] - '0') << 8; //Dup
            n += (uint)(s[10] - '0') << 9; //Ddown
            n += (uint)(s[11] - '0') << 10; //Dright
            n += (uint)(s[12] - '0') << 11; //Dleft
            return n;
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialOpen)
            {
                _continue = false;
                readThread.Abort();
                _serialPort.Close();
            }
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
            //Connect serial port
            SetText(infoLbl, String.Format("Connecting to serial port on port {0}...", port));
            if (portList.Items.Count == 0)
            {
                port = "";
                SetText(infoLbl, "No Com port selected"); return;
            }
            port = portList.SelectedItem.ToString();
            _serialPort = new SerialPort(port, baud);
            _serialPort.DtrEnable = true; //Make sure Arduino is reset when port is opened

            readThread = new Thread(new ThreadStart(Read));

            _serialPort.Open();
            _continue = true;
            serialOpen = true;

            readThread.Start();
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            //Disconnect serial port
            SetText(infoLbl, "Disconnecting serial port...");
            if (port == "") { SetText(infoLbl, "No Com port selected"); return; }
            _continue = false;
            serialOpen = false;
            readThread.Abort();
            _serialPort.Close();
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

            }
        }

    }
}
