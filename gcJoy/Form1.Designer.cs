
namespace gcJoy
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rumbleCheck1 = new System.Windows.Forms.CheckBox();
            this.infoBox1 = new System.Windows.Forms.TextBox();
            this.rumbleCheck2 = new System.Windows.Forms.CheckBox();
            this.dividerLabel = new System.Windows.Forms.Label();
            this.detectBtn = new System.Windows.Forms.Button();
            this.connectBtn = new System.Windows.Forms.Button();
            this.infoLbl = new System.Windows.Forms.Label();
            this.portList = new System.Windows.Forms.ListBox();
            this.disconnectBtn = new System.Windows.Forms.Button();
            this.portTimer = new System.Windows.Forms.Timer(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.infoBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(27, 151);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(632, 37);
            this.textBox1.TabIndex = 0;
            // 
            // rumbleCheck1
            // 
            this.rumbleCheck1.AutoSize = true;
            this.rumbleCheck1.Location = new System.Drawing.Point(27, 205);
            this.rumbleCheck1.Name = "rumbleCheck1";
            this.rumbleCheck1.Size = new System.Drawing.Size(71, 17);
            this.rumbleCheck1.TabIndex = 1;
            this.rumbleCheck1.Text = "Rumble 1";
            this.rumbleCheck1.UseVisualStyleBackColor = true;
            this.rumbleCheck1.CheckedChanged += new System.EventHandler(this.rumbleCheck1_CheckedChanged);
            // 
            // infoBox1
            // 
            this.infoBox1.Enabled = false;
            this.infoBox1.Location = new System.Drawing.Point(27, 228);
            this.infoBox1.Name = "infoBox1";
            this.infoBox1.Size = new System.Drawing.Size(632, 20);
            this.infoBox1.TabIndex = 2;
            // 
            // rumbleCheck2
            // 
            this.rumbleCheck2.AutoSize = true;
            this.rumbleCheck2.Location = new System.Drawing.Point(27, 329);
            this.rumbleCheck2.Name = "rumbleCheck2";
            this.rumbleCheck2.Size = new System.Drawing.Size(71, 17);
            this.rumbleCheck2.TabIndex = 3;
            this.rumbleCheck2.Text = "Rumble 2";
            this.rumbleCheck2.UseVisualStyleBackColor = true;
            this.rumbleCheck2.CheckedChanged += new System.EventHandler(this.rumbleCheck2_CheckedChanged);
            // 
            // dividerLabel
            // 
            this.dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerLabel.Location = new System.Drawing.Point(12, 131);
            this.dividerLabel.Name = "dividerLabel";
            this.dividerLabel.Size = new System.Drawing.Size(668, 2);
            this.dividerLabel.TabIndex = 5;
            // 
            // detectBtn
            // 
            this.detectBtn.Location = new System.Drawing.Point(356, 34);
            this.detectBtn.Name = "detectBtn";
            this.detectBtn.Size = new System.Drawing.Size(156, 23);
            this.detectBtn.TabIndex = 7;
            this.detectBtn.Text = "Auto detect port";
            this.detectBtn.UseVisualStyleBackColor = true;
            this.detectBtn.Click += new System.EventHandler(this.detectBtn_Click);
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(356, 63);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 8;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // infoLbl
            // 
            this.infoLbl.AutoSize = true;
            this.infoLbl.Location = new System.Drawing.Point(355, 93);
            this.infoLbl.Name = "infoLbl";
            this.infoLbl.Size = new System.Drawing.Size(10, 13);
            this.infoLbl.TabIndex = 9;
            this.infoLbl.Text = "-";
            // 
            // portList
            // 
            this.portList.FormattingEnabled = true;
            this.portList.Location = new System.Drawing.Point(198, 21);
            this.portList.Name = "portList";
            this.portList.Size = new System.Drawing.Size(120, 95);
            this.portList.TabIndex = 6;
            // 
            // disconnectBtn
            // 
            this.disconnectBtn.Location = new System.Drawing.Point(437, 63);
            this.disconnectBtn.Name = "disconnectBtn";
            this.disconnectBtn.Size = new System.Drawing.Size(75, 23);
            this.disconnectBtn.TabIndex = 10;
            this.disconnectBtn.Text = "Disconnect";
            this.disconnectBtn.UseVisualStyleBackColor = true;
            this.disconnectBtn.Click += new System.EventHandler(this.disconnectBtn_Click);
            // 
            // portTimer
            // 
            this.portTimer.Enabled = true;
            this.portTimer.Interval = 300;
            this.portTimer.Tick += new System.EventHandler(this.portTimer_Tick);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(27, 275);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(632, 37);
            this.textBox2.TabIndex = 0;
            // 
            // infoBox2
            // 
            this.infoBox2.Enabled = false;
            this.infoBox2.Location = new System.Drawing.Point(27, 352);
            this.infoBox2.Name = "infoBox2";
            this.infoBox2.Size = new System.Drawing.Size(632, 20);
            this.infoBox2.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 481);
            this.Controls.Add(this.disconnectBtn);
            this.Controls.Add(this.infoLbl);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.detectBtn);
            this.Controls.Add(this.portList);
            this.Controls.Add(this.dividerLabel);
            this.Controls.Add(this.rumbleCheck2);
            this.Controls.Add(this.infoBox2);
            this.Controls.Add(this.infoBox1);
            this.Controls.Add(this.rumbleCheck1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox rumbleCheck1;
        private System.Windows.Forms.TextBox infoBox1;
        private System.Windows.Forms.CheckBox rumbleCheck2;
        private System.Windows.Forms.Label dividerLabel;
        private System.Windows.Forms.Button detectBtn;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Label infoLbl;
        private System.Windows.Forms.ListBox portList;
        private System.Windows.Forms.Button disconnectBtn;
        private System.Windows.Forms.Timer portTimer;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox infoBox2;
    }
}

