using System;
using System.Windows.Forms;

namespace RedRat.RaceTiming
{
    public partial class RaceTimingForm : Form
    {
        public RaceTimingForm()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show( "Hello..." );
        }
    }
}
