using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RedRat.RaceTiming.Core;
using RedRat.RaceTiming.Data.Model;

namespace RedRat.RaceTimingWinApp
{
    //
    // To Do:
    //  - Add tracing and logging.
    //
    //
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
            SetTitle();

            Application.ThreadException += (o, e) => ShowExceptionMessageBox(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (o, e) => ShowExceptionMessageBox((Exception)e.ExceptionObject);
        }

        private void SetTitle()
        {
            var name = appController.IsDbOpen ? appController.CurrentRace.Name : "---";
            Text = string.Format( "Race Timing: {0}", name );
        }

        private void ShowExceptionMessageBox( Exception ex )
        {
            MessageBox.Show( "Exception from application: " + ex.Message );
        }

        /// <summary>
        /// Used to make sure we have race data before performing any operations.
        /// </summary>
        private bool CheckHaveDb()
        {
            if ( appController.IsDbOpen ) return true;

            MessageBox.Show( "Have not yet loaded or created a race database. Use the 'New' or 'Open' menu items.",
                "No race data", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            return false;
        }

        /// <summary>
        /// Checks what the user wants to do if we already have a DB open. This should be used before
        /// we create a new DB or open a new DB file.
        /// </summary>
        /// <returns>true if it is OK to close the current db file and open a new one.</returns>
        private bool CheckWhatToDoIfHaveDbOpen()
        {
            if ( appController.IsDbOpen )
            {
                var msg = string.Format( "The race '{0}' is currently open. Are you sure you want to close it?",
                    appController.CurrentRace.Name );
                var result = MessageBox.Show( msg, "Race database currently open", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
                if ( result != DialogResult.Yes )
                {
                    // Don't want to open a new DB
                    return false;
                }
            }
            return true;
        }

        # region File Menu

        private void NewToolStripMenuItemClick( object sender, EventArgs e )
        {
            // If we have a DB open, check if we want it to be closed first.
            if (! CheckWhatToDoIfHaveDbOpen() ) return;

            // 1. Get race details from user
            var raceDetailsDialog = new RaceDetailsDialog();
            if ( raceDetailsDialog.ShowDialog() != DialogResult.OK )
            {
                // OK - user wants to quit
                return;
            }

            // Create race object
            var race = new Race
            {
                Name = raceDetailsDialog.RaceName,
                Description = raceDetailsDialog.RaceDescritpion,
                Distance = raceDetailsDialog.RaceDistance,
                Date = raceDetailsDialog.RaceDate
            };

            // 2. Get a filename for data storage from user. This also checks to see if the
            //    file can be overwritten if it exists.
            var saveFileDlg = new SaveFileDialog
            {
                RestoreDirectory = true,
                Filter = "database files (*.dbs)|*.dbs|All files (*.*)|*.*",
                FileName = race.Name + ".dbs",
                Title = "Database file for the race data..."
            };

            if ( saveFileDlg.ShowDialog() != DialogResult.OK )
            {
                // OK - user wants to quit
                return;                
            }

            // This will close the current DB and create a new one.
            appController.CreateNewRace( race, saveFileDlg.FileName );
            SetTitle();
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // If we have a DB open, check if we want it to be closed first.
            if (!CheckWhatToDoIfHaveDbOpen()) return;

            var openFileDlg = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = "database files (*.dbs)|*.dbs|All files (*.*)|*.*",
                Title = "Open a race database file..."
            };

            if (openFileDlg.ShowDialog() != DialogResult.OK)
            {
                // OK - user wants to quit
                return;
            }
            appController.OpenRace( openFileDlg.FileName );
            SetTitle();
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

        #endregion

        #region Edit Menu

        private void RaceDetailsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if ( !CheckHaveDb()) return;

            var raceDetailsDialog = new RaceDetailsDialog(appController.CurrentRace);
            if (raceDetailsDialog.ShowDialog() != DialogResult.OK)
            {
                // OK - user wants to quit
                return;
            }

            // Create race object
            var race = new Race
            {
                Name = raceDetailsDialog.RaceName,
                Description = raceDetailsDialog.RaceDescritpion,
                Distance = raceDetailsDialog.RaceDistance,
                Date = raceDetailsDialog.RaceDate
            };

            appController.UpdateCurrentRace( race );
        }

        #endregion
    }
}
