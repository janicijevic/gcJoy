
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rumbleCheck = new System.Windows.Forms.CheckBox();
            this.infoBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(478, 102);
            this.textBox1.TabIndex = 0;
            // 
            // rumbleCheck
            // 
            this.rumbleCheck.AutoSize = true;
            this.rumbleCheck.Location = new System.Drawing.Point(23, 159);
            this.rumbleCheck.Name = "rumbleCheck";
            this.rumbleCheck.Size = new System.Drawing.Size(62, 17);
            this.rumbleCheck.TabIndex = 1;
            this.rumbleCheck.Text = "Rumble";
            this.rumbleCheck.UseVisualStyleBackColor = true;
            this.rumbleCheck.CheckedChanged += new System.EventHandler(this.rumbleCheck_CheckedChanged);
            // 
            // infoBox
            // 
            this.infoBox.Enabled = false;
            this.infoBox.Location = new System.Drawing.Point(12, 230);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(478, 20);
            this.infoBox.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 262);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.rumbleCheck);
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
        private System.Windows.Forms.CheckBox rumbleCheck;
        private System.Windows.Forms.TextBox infoBox;
    }
}

