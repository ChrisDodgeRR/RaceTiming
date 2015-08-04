using System;
using System.Windows.Forms;
using RedRat.RaceTiming.Data.Model;

namespace RedRat.RaceTimingWinApp
{
    /// <summary>
    /// Dialog for editing race details.
    /// </summary>
    public partial class RaceDetailsDialog : Form
    {
        public RaceDetailsDialog()
        {
            InitializeComponent();
        }

        public RaceDetailsDialog( Race race )
        {
            InitializeComponent();
            if ( race == null ) return;

            // Set GUI state
            raceNameTextBox.Text = race.Name;
            raceDescriptionTextBox.Text = race.Description;
            distanceNumericUpDown.Value = (decimal)race.Distance;
            raceDateTimePicker.Value = race.Date;
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            // Check the entered data - only the name at the moment.
            if (string.IsNullOrWhiteSpace(raceNameTextBox.Text))
            {
                MessageBox.Show("The race must be given a name.", "No name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }

        public string RaceName
        {
            get { return raceNameTextBox.Text; }
        }

        public string RaceDescritpion
        {
            get { return raceDescriptionTextBox.Text; }
        }

        public double RaceDistance
        {
            get { return (double)distanceNumericUpDown.Value; }
        }

        public DateTime RaceDate
        {
            get { return raceDateTimePicker.Value; }
        }
    }
}
