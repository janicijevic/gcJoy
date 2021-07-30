
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
            this.rumbleCheck1 = new System.Windows.Forms.CheckBox();
            this.infoBox = new System.Windows.Forms.TextBox();
            this.rumbleCheck2 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(622, 124);
            this.textBox1.TabIndex = 0;
            // 
            // rumbleCheck1
            // 
            this.rumbleCheck1.AutoSize = true;
            this.rumbleCheck1.Location = new System.Drawing.Point(12, 223);
            this.rumbleCheck1.Name = "rumbleCheck1";
            this.rumbleCheck1.Size = new System.Drawing.Size(71, 17);
            this.rumbleCheck1.TabIndex = 1;
            this.rumbleCheck1.Text = "Rumble 1";
            this.rumbleCheck1.UseVisualStyleBackColor = true;
            this.rumbleCheck1.CheckedChanged += new System.EventHandler(this.rumbleCheck1_CheckedChanged);
            // 
            // infoBox
            // 
            this.infoBox.Enabled = false;
            this.infoBox.Location = new System.Drawing.Point(12, 299);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(622, 20);
            this.infoBox.TabIndex = 2;
            // 
            // rumbleCheck2
            // 
            this.rumbleCheck2.AutoSize = true;
            this.rumbleCheck2.Location = new System.Drawing.Point(12, 247);
            this.rumbleCheck2.Name = "rumbleCheck2";
            this.rumbleCheck2.Size = new System.Drawing.Size(71, 17);
            this.rumbleCheck2.TabIndex = 3;
            this.rumbleCheck2.Text = "Rumble 2";
            this.rumbleCheck2.UseVisualStyleBackColor = true;
            this.rumbleCheck2.CheckedChanged += new System.EventHandler(this.rumbleCheck2_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 331);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rumbleCheck2);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.rumbleCheck1);
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
        private System.Windows.Forms.TextBox infoBox;
        private System.Windows.Forms.CheckBox rumbleCheck2;
        private System.Windows.Forms.Label label1;
    }
}

