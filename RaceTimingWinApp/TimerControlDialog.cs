using System;
using System.Windows.Forms;
using RedRat.RaceTiming.Core;

namespace RedRat.RaceTimingWinApp
{
    public partial class TimerControlDialog : Form
    {
        private AppController appController;

        public TimerControlDialog(AppController appController)
        {
            InitializeComponent();
            this.appController = appController;
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            appController.ClockTime.Start();
        }
    }
}
