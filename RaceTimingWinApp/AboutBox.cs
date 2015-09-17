using System.Reflection;
using System.Windows.Forms;

namespace RedRat.RaceTimingWinApp
{
    public class AboutBox : Form
    {

        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label copyrightLabel;
        protected System.Windows.Forms.GroupBox groupBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public AboutBox(Assembly assembly)
            : this(assembly, null)
        {
        }

        public AboutBox(Assembly assembly, string title)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            MaximizeBox = false;

            // == Load version info. etc. from assembly ==

            // Title
            if (title == null)
            {
                var ata = (AssemblyTitleAttribute[])assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if (ata.Length > 0)
                {
                    // Take the first one. Do we ever have more???
                    titleLabel.Text = ata[0].Title;
                    Text = "About " + titleLabel.Text;
                }
            }
            else
            {
                titleLabel.Text = title;
                Text = "About " + titleLabel.Text;
            }

            // Version
            versionLabel.Text = "Version: " + assembly.GetName().Version;

            // Description
            var ada = (AssemblyDescriptionAttribute[])assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
            if (ada.Length > 0)
            {
                // Take the first one. Do we ever have more???
                descLabel.Text = ada[0].Description;
            }

            // Copyright
            var aca = (AssemblyCopyrightAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if (aca.Length > 0)
            {
                // Take the first one. Do we ever have more???
                copyrightLabel.Text = aca[0].Copyright;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.descLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.ImageLocation = "";
            this.logoPictureBox.Location = new System.Drawing.Point(39, 57);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(250, 60);
            this.logoPictureBox.TabIndex = 0;
            this.logoPictureBox.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(240, 234);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(16, 16);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(290, 16);
            this.titleLabel.TabIndex = 2;
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // descLabel
            // 
            this.descLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descLabel.Location = new System.Drawing.Point(24, 8);
            this.descLabel.Name = "descLabel";
            this.descLabel.Size = new System.Drawing.Size(280, 16);
            this.descLabel.TabIndex = 3;
            this.descLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(16, 32);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(192, 16);
            this.versionLabel.TabIndex = 4;
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Location = new System.Drawing.Point(16, 48);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(192, 16);
            this.copyrightLabel.TabIndex = 5;
            this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.titleLabel);
            this.groupBox.Controls.Add(this.versionLabel);
            this.groupBox.Controls.Add(this.copyrightLabel);
            this.groupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox.Location = new System.Drawing.Point(8, 144);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(312, 82);
            this.groupBox.TabIndex = 6;
            this.groupBox.TabStop = false;
            // 
            // AboutBox
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(326, 266);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.descLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.logoPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AboutBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void okButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
