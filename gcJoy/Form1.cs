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
        bool _continue;
        bool rumble = false;
        SerialPort _serialPort;
        Thread readThread;

        //// Declaring one joystick (Device id 1) and a position structure. 
        public vJoy joystick;
        public vJoy.JoystickState iReport;
        public uint id = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _serialPort = new SerialPort("COM5", 115200);
            _serialPort.DtrEnable = true; //Make sure Arduino is reset when port is opened

            joystick = new vJoy();
            iReport = new vJoy.JoystickState();

            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                infoBox.Text = String.Format("Failed to acquire vJoy device number {0}.", id); return;
            }
            else
                infoBox.Text = String.Format("Acquired: vJoy device number {0}.", id);


            readThread = new Thread(new ThreadStart(Read));

            _serialPort.Open();
            _continue = true;

            readThread.Start();
                
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }
        }

        public void Read()
        {
            while (_continue)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    string message = _serialPort.ReadLine();

                    if (message.Contains("ready"))
                    {
                        Console.WriteLine("Ready");
                        break;
                    }
                }
            }
            while (_continue)
            {

                _serialPort.WriteLine("get1"+ (rumble ? "1" : "0") + "-" );

                Thread.Sleep(10);
                if (_serialPort.BytesToRead > 0)
                {
                    int bytes = _serialPort.BytesToRead;    
                    string message = _serialPort.ReadLine();
                    message = message.Substring(0, message.Length - 2);

                    SetText(message);

                    //Parse message
                    if (message.Length == 64 && message.All(c => c >= '0' && c <= '9'))
                    {
                        parseMessage(message);
                        if (!joystick.UpdateVJD(id, ref iReport)) SetText(String.Format("Failed to update joystick number {0}", id));
                    }
                    else if (message == "disconnected") SetText("Controller disconnected");
                    else if (message == "disabled") SetText("Controller is disabled");
                    else SetText("Error");
                }
                else
                {
                    Console.WriteLine("No response");
                }

            }
        }
        public void parseMessage(string message)
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
            iReport.Buttons = parseButtons(message.Substring(3, 13));
            iReport.AxisX = toInt(message.Substring(8 * 2, 8));     //Main joystick
            iReport.AxisY = toInt(message.Substring(8 * 3, 8));
            iReport.AxisXRot = toInt(message.Substring(8 * 4, 8));  //C stick
            iReport.AxisYRot = toInt(message.Substring(8 * 5, 8));
            iReport.Slider = toInt(message.Substring(8 * 6, 8));    //Triggers
            iReport.Dial = toInt(message.Substring(8 * 7, 8));
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _continue = false;
            readThread.Abort();
            _serialPort.Close();
        }

        private void rumbleCheck_CheckedChanged(object sender, EventArgs e)
        {
            rumble = !rumble;
        }
    }
}
