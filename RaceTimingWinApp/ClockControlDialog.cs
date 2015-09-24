using System;
using System.Drawing;
using System.Windows.Forms;
using RedRat.RaceTiming.Core;

namespace RedRat.RaceTimingWinApp
{
    public partial class ClockControlDialog : Form
    {
        private readonly AppController appController;
        private readonly ClockLabel clockLabel;

        public ClockControlDialog(AppController appController)
        {
            InitializeComponent();
            this.appController = appController;

            // Add the clock label
            clockLabel = new ClockLabel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Microsoft Sans Serif", 25F, FontStyle.Regular, GraphicsUnit.Millimeter, 0),
                Location = new Point(20, 50),
                Name = "clockLabel",
                Size = new Size(270, 110),
            };
            clockControlGroupBox.Controls.Add(clockLabel);
            appController.ClockTime.ClockChangeHandler += clockLabel.ClockChangeEventListener;
            clockLabel.SetTimeLabel( appController.ClockTime.CurrentTime );
            clockLabel.Redraw();
        }

        private TimeSpan GetDelta( int inc )
        {
            var delta = TimeSpan.FromSeconds(inc);
            if (minsRadioButton.Checked)
            {
                delta = TimeSpan.FromMinutes(inc);
            }
            else if (hoursRadioButton.Checked)
            {
                delta = TimeSpan.FromHours(inc);
            }
            return delta;
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.Start();
        }

        private void StopButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.Stop();
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.Reset();
        }

        private void P10ButtonClick( object sender, EventArgs e )
        {
            appController.ClockTime.AddTime( GetDelta( 10 ), secRadioButton.Checked );
        }

        private void P1ButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.AddTime( GetDelta( 1 ), secRadioButton.Checked );
        }

        private void M1ButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.AddTime( GetDelta( -1 ), secRadioButton.Checked );
        }

        private void M10ButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.AddTime( GetDelta( -10 ), secRadioButton.Checked );
        }

        private void ClockControlDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            appController.ClockTime.ClockChangeHandler -= clockLabel.ClockChangeEventListener;
        }


    }
}
