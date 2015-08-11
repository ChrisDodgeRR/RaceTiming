namespace RedRat.RaceTimingWinApp
{
    partial class ClockControlDialog
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
            this.clockControlGroupBox = new System.Windows.Forms.GroupBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.p1Button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m1Button = new System.Windows.Forms.Button();
            this.secRadioButton = new System.Windows.Forms.RadioButton();
            this.minsRadioButton = new System.Windows.Forms.RadioButton();
            this.hoursRadioButton = new System.Windows.Forms.RadioButton();
            this.p10Button = new System.Windows.Forms.Button();
            this.m10Button = new System.Windows.Forms.Button();
            this.clockControlGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clockControlGroupBox
            // 
            this.clockControlGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clockControlGroupBox.Controls.Add(this.resetButton);
            this.clockControlGroupBox.Controls.Add(this.stopButton);
            this.clockControlGroupBox.Controls.Add(this.startButton);
            this.clockControlGroupBox.Location = new System.Drawing.Point(13, 13);
            this.clockControlGroupBox.Name = "clockControlGroupBox";
            this.clockControlGroupBox.Size = new System.Drawing.Size(308, 163);
            this.clockControlGroupBox.TabIndex = 0;
            this.clockControlGroupBox.TabStop = false;
            this.clockControlGroupBox.Text = "Clock Control";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(212, 31);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(116, 31);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButtonClick);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(20, 31);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(438, 182);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // p1Button
            // 
            this.p1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.p1Button.Location = new System.Drawing.Point(126, 55);
            this.p1Button.Name = "p1Button";
            this.p1Button.Size = new System.Drawing.Size(38, 23);
            this.p1Button.TabIndex = 3;
            this.p1Button.Text = "+1";
            this.p1Button.UseVisualStyleBackColor = true;
            this.p1Button.Click += new System.EventHandler(this.P1ButtonClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.m10Button);
            this.groupBox1.Controls.Add(this.p10Button);
            this.groupBox1.Controls.Add(this.m1Button);
            this.groupBox1.Controls.Add(this.secRadioButton);
            this.groupBox1.Controls.Add(this.minsRadioButton);
            this.groupBox1.Controls.Add(this.hoursRadioButton);
            this.groupBox1.Controls.Add(this.p1Button);
            this.groupBox1.Location = new System.Drawing.Point(328, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(185, 163);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Adjustment";
            // 
            // m1Button
            // 
            this.m1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m1Button.Location = new System.Drawing.Point(126, 94);
            this.m1Button.Name = "m1Button";
            this.m1Button.Size = new System.Drawing.Size(38, 23);
            this.m1Button.TabIndex = 7;
            this.m1Button.Text = "-1";
            this.m1Button.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.m1Button.UseVisualStyleBackColor = true;
            this.m1Button.Click += new System.EventHandler(this.M1ButtonClick);
            // 
            // secRadioButton
            // 
            this.secRadioButton.AutoSize = true;
            this.secRadioButton.Checked = true;
            this.secRadioButton.Location = new System.Drawing.Point(23, 79);
            this.secRadioButton.Name = "secRadioButton";
            this.secRadioButton.Size = new System.Drawing.Size(67, 17);
            this.secRadioButton.TabIndex = 6;
            this.secRadioButton.TabStop = true;
            this.secRadioButton.Text = "Seconds";
            this.secRadioButton.UseVisualStyleBackColor = true;
            // 
            // minsRadioButton
            // 
            this.minsRadioButton.AutoSize = true;
            this.minsRadioButton.Location = new System.Drawing.Point(23, 55);
            this.minsRadioButton.Name = "minsRadioButton";
            this.minsRadioButton.Size = new System.Drawing.Size(62, 17);
            this.minsRadioButton.TabIndex = 5;
            this.minsRadioButton.Text = "Minutes";
            this.minsRadioButton.UseVisualStyleBackColor = true;
            // 
            // hoursRadioButton
            // 
            this.hoursRadioButton.AutoSize = true;
            this.hoursRadioButton.Location = new System.Drawing.Point(23, 31);
            this.hoursRadioButton.Name = "hoursRadioButton";
            this.hoursRadioButton.Size = new System.Drawing.Size(53, 17);
            this.hoursRadioButton.TabIndex = 4;
            this.hoursRadioButton.Text = "Hours";
            this.hoursRadioButton.UseVisualStyleBackColor = true;
            // 
            // p10Button
            // 
            this.p10Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.p10Button.Location = new System.Drawing.Point(126, 25);
            this.p10Button.Name = "p10Button";
            this.p10Button.Size = new System.Drawing.Size(38, 23);
            this.p10Button.TabIndex = 8;
            this.p10Button.Text = "+10";
            this.p10Button.UseVisualStyleBackColor = true;
            this.p10Button.Click += new System.EventHandler(this.P10ButtonClick);
            // 
            // m10Button
            // 
            this.m10Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m10Button.Location = new System.Drawing.Point(126, 123);
            this.m10Button.Name = "m10Button";
            this.m10Button.Size = new System.Drawing.Size(38, 23);
            this.m10Button.TabIndex = 9;
            this.m10Button.Text = "-10";
            this.m10Button.UseVisualStyleBackColor = true;
            this.m10Button.Click += new System.EventHandler(this.M10ButtonClick);
            // 
            // ClockControlDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 217);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.clockControlGroupBox);
            this.MinimumSize = new System.Drawing.Size(510, 256);
            this.Name = "ClockControlDialog";
            this.Text = "Clock Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClockControlDialogFormClosing);
            this.clockControlGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox clockControlGroupBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button p1Button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button m1Button;
        private System.Windows.Forms.RadioButton secRadioButton;
        private System.Windows.Forms.RadioButton minsRadioButton;
        private System.Windows.Forms.RadioButton hoursRadioButton;
        private System.Windows.Forms.Button m10Button;
        private System.Windows.Forms.Button p10Button;
    }
}