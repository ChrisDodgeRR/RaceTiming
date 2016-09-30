using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using RedRat.RaceTiming.Core;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Core.Web;
using RedRat.RaceTiming.Data.Model;
using RedRat.RaceTimingWinApp.ExtendedListView;
using RedRat.RaceTimingWinApp.Options;

namespace RedRat.RaceTimingWinApp
{
    public partial class RaceTimingForm : Form
    {
        private readonly AppController appController;
        private ClockControlDialog clockControl;
        private readonly ClockLabel clockLabel;
        private bool clockRunning;

        public RaceTimingForm()
        {
            Application.EnableVisualStyles();
            InitializeComponent();

            // Add the clock label
            clockLabel = new ClockLabel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font( "Microsoft Sans Serif", 25F, FontStyle.Regular, GraphicsUnit.Millimeter, 0 ),
                Location = new Point( 12, 24 ),
                Name = "clockLabel",
                Size = new Size(splitContainer1.Panel1.Size.Width, 166),
            };
            splitContainer1.Panel1.Controls.Add( clockLabel );
            spaceBarLabel.Visible = false;

            Application.ThreadException += (o, e) => ShowExceptionMessageBox(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (o, e) => ShowExceptionMessageBox((Exception)e.ExceptionObject);

            appController = new ControllerFactory().AppController;
            appController.ClockTime.ClockChangeHandler += clockLabel.ClockChangeEventListener;
            appController.ClockTime.ClockRunningHandler += ClockTimeOnClockRunningHandler;
            appController.ResultDataChange += ResultsQueueOnNewResult;
            SetTitle();

            // Setup result list view
            var listViewExtender = new ListViewExtender(resultListView);
            var deleteResultColumn = new ListViewButtonColumn(3) { FixedWidth = true, DrawIfEmpty = false };
            deleteResultColumn.Click += DeleteResult;
            listViewExtender.AddColumn(deleteResultColumn);
            ListResults();

            var webController = new WebController( appController.GetRootUrl() );
            webController.Start();
		}


        private void RaceTimingFormShown(object sender, EventArgs e)
        {
            if ( appController.IsDbOpen )
            {
                // On initial start, need to set clock time from file.
                appController.ClockTime.CurrentTime = appController.CurrentRace.ClockTime;
            }
        }

        private void SetTitle()
        {
            var name = appController.IsDbOpen ? appController.CurrentRace.Name : "---";
            Text = string.Format( "Race Timing: {0}", name );
        }

        private void ShowExceptionMessageBox( Exception ex )
        {
            var msg = "Exception from application: " + ex.Message;
            MessageBox.Show( msg );
            Trace.WriteLineIf( AppController.traceSwitch.TraceError, msg );
            Trace.WriteLineIf( AppController.traceSwitch.TraceError, ex.StackTrace );
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
            var raceDetailsDialog = new RaceDetailsDialog()
            {
                Icon = Icon,
            };
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
            ListResults();
        }

        private void ImportRunnerDataToolStripMenuItemClick( object sender, EventArgs e )
        {
            if (!CheckHaveDb()) return;

            var openFileDlg = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = "Excel CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Open a race entrant CSV file..."
            };

            if ( openFileDlg.ShowDialog() != DialogResult.OK )
            {
                // OK - user wants to quit
                return;
            }
            var filename = openFileDlg.FileName;
            var result = appController.LoadCsvFile( filename );

            var msg = string.Format( "File '{0}' loaded.\n\n\t{1} runners added\n\t{2} already in DB\n\t{3} line(s) ignored", 
                filename, result.Imported, result.AlreadyExisting, result.Ignored);
            Trace.WriteLineIf( AppController.traceSwitch.TraceInfo, (string)msg );  // Need to cast with dynamic object
            MessageBox.Show( msg, "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }

        private void BackupDbToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                if ( !CheckHaveDb() ) return;

                var backupFilename = appController.BackupDb();
                if ( backupFilename != null )
                {
                    var msg = string.Format( "Database backed up to '{0}'", backupFilename );
                    MessageBox.Show( msg, "Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information );
                }
            }
            catch ( Exception ex )
            {
                var msg = string.Format( "Unable to backup database - {0}", ex.Message );
                MessageBox.Show(msg, "Unable to Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetCsvFilename(string dataType)
        {
            var saveFileDlg = new SaveFileDialog
            {
                RestoreDirectory = true,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = string.Format("{0}-{1}.csv", appController.CurrentRace.Name, dataType),
                Title = "CSV file for the race results data...",
                OverwritePrompt = true,
            };

            return saveFileDlg.ShowDialog() != DialogResult.OK ? null : saveFileDlg.FileName;
        }

        private void ExportEntrantsToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Saves entrants to CSV file.
            var filename = GetCsvFilename("Entrants");
            if (filename == null) return;

            using (var writer = new StreamWriter(filename))
            {
                // Title lines
                writer.WriteLine("{0} - {1:D}", appController.CurrentRace.Name, appController.CurrentRace.Date);
                writer.WriteLine("Number,First Name,Last Name,Gender,DoB,Category,Club,URN");

                var entrants = appController.GetRunners().OrderBy( r => r.Number );
                foreach (var entrant in entrants)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",
                        entrant.Number, entrant.FirstName, entrant.LastName, entrant.Gender, entrant.DateOfBirth.ToShortDateString(),
                        AgeGroup.GetAgeGroup(appController.CurrentRace.Date, entrant.DateOfBirth, entrant.Gender),
                        entrant.Club, entrant.Urn);
                }
                writer.Close();
            }
        }

        private void ExportResultsToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Saves results to CSV file.
            var filename = GetCsvFilename("Results");
            if ( filename == null ) return;

            using ( var writer = new StreamWriter( filename ) )
            {
                // Title lines
                writer.WriteLine( "{0} - {1:D}", appController.CurrentRace.Name, appController.CurrentRace.Date );
                writer.WriteLine( "Position, Name, Time, Race Number, Category, Category Position, WMA Score, Club" );

                var finishers = appController.GetFinishers().OrderBy( f => f.Position );
                foreach ( var finisher in finishers )
                {
                    writer.WriteLine( "{0},{1},{2},{3},{4},{5},{6},{7}",
                        finisher.Position, finisher.Name, TimeSpan.FromMilliseconds( finisher.Time ).ToString( @"hh\:mm\:ss" ),
                        finisher.Number,
                        finisher.Category, finisher.CategoryPosition, finisher.Wma, finisher.Club );
                }
                writer.Close();
            }
        }

        private void ExportEmailsToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Saves emails to CSV file.
            var filename = GetCsvFilename("Emails");
            if (filename == null) return;

            using (var writer = new StreamWriter(filename))
            {
                // Title lines
                writer.WriteLine("{0} - {1:D}", appController.CurrentRace.Name, appController.CurrentRace.Date);
                writer.WriteLine("Name, Email");

                var runners = appController.GetRunners();
                foreach (var runner in runners.Where( r => !string.IsNullOrEmpty(r.Email) ) )
                {
                    writer.WriteLine("{0} {1},{2}", runner.FirstName, runner.LastName, runner.Email);
                }
                writer.Close();
            }
        }

        private void OptionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            var optionsDialog = new OptionsDialog( appController.Options )
            {
                Icon = Icon,
            };
            optionsDialog.ShowDialog();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
			Application.Exit();
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
            SetTitle();
        }

        #endregion

        #region View Menu

        private void HomePageToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage( appController.GetRootUrl() );
        }

        private void RaceEntrybrowserToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage(appController.GetRootUrl() + "addrunner");
        }

        private void RaceEntrantsToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage(appController.GetRootUrl() + "runners");
        }

        private void AddFinishPositionsbrowserToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage(appController.GetRootUrl() + "enterpositions");
        }

        private void ListAllFinishersbrowserToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage(appController.GetRootUrl() + "allresults");
        }

        private void PrizeWinnersbrowserToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage(appController.GetRootUrl() + "winners");
        }

        private void TeamResultsbrowserToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWebBrowserAtPage(appController.GetRootUrl() + "teams");
        }

        private void RaceStatsToolStripMenuItemClick( object sender, EventArgs e )
        {
            var stats = appController.GetRaceStats();
            var msg = "RACE STATISTICS:\n\n" +
                      "Number of entrants:\t\t" + stats.NumberEntrants.Total + "\n" +
                      "\t- Male:\t\t" + stats.NumberEntrants.Male + "\n" +
                      "\t- Female:\t\t" + stats.NumberEntrants.Female + "\n\n" +

                      "Number of finishers:\t\t" + stats.NumberFinishers.Total + "\n" +
                      "\t- Male:\t\t" + stats.NumberFinishers.Male + "\n" +
                      "\t- Female:\t\t" + stats.NumberFinishers.Female + "\n\n" +

                      "Affiliated Entrants:\t\t" + stats.NumberAffiliatedEntrants.Total + "\n" +
                      "\t- Male:\t\t" + stats.NumberAffiliatedEntrants.Male + "\n" +
                      "\t- Female:\t\t" + stats.NumberAffiliatedEntrants.Female + "\n\n" +

                      "Unaffiliated Entrants:\t\t" + stats.NumberUnaffiliatedEntrants.Total + "\n" +
                      "\t- Male:\t\t" + stats.NumberUnaffiliatedEntrants.Male + "\n" +
                      "\t- Female:\t\t" + stats.NumberUnaffiliatedEntrants.Female + "\n\n";
            MessageBox.Show( msg, "Race Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }


        #endregion

        #region Timing Menu

        private void ClockControlToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!CheckHaveDb()) return;

            if ( clockControl == null )
            {
                clockControl = new ClockControlDialog( appController )
                {
                    Icon = Icon,
                };
                clockControl.Show();
                clockControl.Closed += ( o, args ) => { clockControl = null; };
            }
            else
            {
                clockControl.BringToFront();
            }
        }

        /// <summary>
        /// Resets all race result data.
        /// </summary>
        private void ResetRaceToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!CheckHaveDb()) return;

            var res = MessageBox.Show( "This will delete all result data - do you want to continue?", 
                "Delete Result Data?", MessageBoxButtons.YesNo, MessageBoxIcon.Question );

            if ( res != DialogResult.Yes ) return;

            appController.DeleteResultData();
        }

        #endregion

        #region Help Menu

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutBox(Assembly.GetExecutingAssembly()).ShowDialog();
        }

        #endregion

        private void StartWebBrowserAtPage( string url )
        {
            try
            {
                Process.Start( url );
            }
            catch (Exception ex)
            {
                var msg = "Unable to start web browser: " + ex.Message;
                Trace.WriteLineIf(AppController.traceSwitch.TraceInfo, msg);
                MessageBox.Show(msg, "Error starting web browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void ClockTimeOnClockRunningHandler(object sender, bool clockRunning)
        {
            this.clockRunning = clockRunning;
            spaceBarLabel.Visible = clockRunning;            
        }

        private void RaceTimingFormKeyPress(object sender, KeyPressEventArgs e)
        {
            if ( !clockRunning ) return;

            if ( e.KeyChar == ' ' )
            {
                appController.AddResultTime();
                Task.Run( () => clockLabel.Blink() );
            }
            e.Handled = true;
        }

        private void RaceTimingFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if ( MessageBox.Show( "Are you sure you want to exit?", "Close the Application?", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question ) == DialogResult.No )
            {
                e.Cancel = true;
                return;
            }

            // If the clock is running, then check again.
            if ( clockRunning )
            {
                if ( MessageBox.Show( "The clock is running, so are you REALLY sure?", "Close the Application?", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question ) == DialogResult.No )
                {
                    e.Cancel = true;
                    return;
                }
            }

            // OK - quit
            appController.ClockTime.Stop();
            if ( appController.IsDbOpen )
            {
                appController.SaveClockTime();
            }
        }

        private void ResultsQueueOnNewResult( object sender, EventArgs eventArgs )
        {
            Invoke( new Action( ListResults ) );
        }

        private void ListResults()
        {
			if (!appController.IsDbOpen) {
				return;
			}
            resultListView.Items.Clear();
            // Limit result presentation to 50 to ensure no GUI issues (not really tested yet).
            var results = appController.GetResults().OrderByDescending( r => r.Position ).Take( 50 );
            foreach ( var result in results )
            {
                resultListView.Items.Add( new ResultListViewItem( result ) );
            }
        }

        /// <summary>
        /// This allows the person capturing times to delete them, e.g. in case the space bar has been accidently pressed.
        /// </summary>
        private void DeleteResult(object sender, ListViewColumnMouseEventArgs e)
        {
            var lvi = e.Item;
            var msg = string.Format( "Do you want to delete result at position {0} with finishing time '{1}'?",
                lvi.Text, lvi.SubItems[1].Text );
            var res = MessageBox.Show( msg, "Delete Time?", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question );
            if ( res != DialogResult.Yes ) return;

            int pos;
            if ( int.TryParse( lvi.Text, out pos ) && pos != 0 )
            {
                try
                {
                    appController.DeleteResultAtPosition(pos);
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Unable to delete result: " + ex.Message, "Error from application", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation );
                }
            }
        }
    }
}
