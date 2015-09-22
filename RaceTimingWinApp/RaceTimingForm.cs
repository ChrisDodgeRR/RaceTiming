using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using RedRat.RaceTiming.Core;
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
            var results = appController.GetResults().OrderByDescending( r => r.Position );
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
                    appController.DeleteResultTimeAtPosition(pos);
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
