using System.Windows.Forms;

namespace RedRat.RaceTimingWinApp.Options
{
    public partial class OptionsDialog : Form
    {
        private readonly RaceTiming.Core.Config.Options options;

        public OptionsDialog(RaceTiming.Core.Config.Options options)
        {
            this.options = options;
            InitializeComponent();

            reopenDbFileCheckbox.Checked = options.ReopenLastFile;
        }

        private void OkButtonClick(object sender, System.EventArgs e)
        {
            options.ReopenLastFile = reopenDbFileCheckbox.Checked;
        }
    }
}
