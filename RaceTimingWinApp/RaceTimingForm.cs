using System;
using System.Drawing;
using System.Windows.Forms;
using RedRat.RaceTiming.Core;
using RedRat.RaceTiming.Data;

namespace RedRat.RaceTimingWinApp
{
    public partial class RaceTimingForm : Form
    {
        private AppController appController;

        public RaceTimingForm()
        {
            Application.EnableVisualStyles();
            InitializeComponent();

            appController = new AppController();

            // Add the clock label
            var clockLabel = new ClockLabel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font( "Microsoft Sans Serif", 25F, FontStyle.Regular, GraphicsUnit.Millimeter, 0 ),
                Location = new Point( 12, 24 ),
                Name = "clockLabel",
                Size = new Size( 512, 166 ),
            };
            Controls.Add( clockLabel );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using ( var dbController = new DbController() )
            {
                dbController.Open( "RaceTimingData.dbs" );
                //dbController.TestInsertData();
                dbController.PrintRunners();
            }
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // ToDo: Open race DB file.
        }

        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // ToDo: Open up new race dialog
        }

        private void ImportRunnerDataToolStripMenuItemClick(object sender, EventArgs e)
        {
            // ToDo: Open CSV file
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // ToDo: Put up message box to confirm.
            Close();
        }
    }
}
