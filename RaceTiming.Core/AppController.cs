
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using RedRat.RaceTiming.Core.Config;
using RedRat.RaceTiming.Core.Logging;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Data;
using RedRat.RaceTiming.Data.Model;

namespace RedRat.RaceTiming.Core
{
    /// <summary>
    /// This is the main application controller. The UI is a thin wrapper round this.
    /// </summary>
    public class AppController
    {
        public static TraceSwitch traceSwitch = new TraceSwitch("RT", "Race Timing");

        // Notifies listeners of new results or race numbers
        public event EventHandler ResultDataChange;

        private readonly DbService db;
        private readonly ClockTime clockTime = new ClockTime();
        private readonly ResultsQueue resultQueue;
        private readonly LogController logController;
        private readonly Options options;

        public AppController(DbService db)
        {
            this.db = db;
            resultQueue = new ResultsQueue(this, db);
            resultQueue.NewResult += ResultsQueueOnNewResult;

            logController = new LogController();
            options = new Options();

            // Tracing
            Trace.Listeners.Add(logController);
            traceSwitch.Level = TraceLevel.Info;
            DbService.dbTraceSwitch.Level = TraceLevel.Info;

            if ( options.ReopenLastFile )
            {
                var lastFile = options.LastFile;
                if ( !string.IsNullOrEmpty( lastFile ) && File.Exists( lastFile ) )
                {
                    OpenRace( lastFile );
                }
            }
        }

        public string GetRootUrl()
        {
            // ToDo: Make configurable, and work out why "localhost" doesn't work on Mac.
            if (CurrentOS.IsMac)
            {
                return "http://0.0.0.0:1234/";
            }
            return "http://localhost:1234/";
        }

        public bool IsDbOpen
        {
            get { return db.IsDbOpen; }
        }

        public ClockTime ClockTime
        {
            get { return clockTime; }
        }

        public Options Options
        {
            get { return options; }
        }

        public void CreateNewRace(Race race, string dbFilename)
        {
            if (db.IsDbOpen)
            {
                db.Close();
            }
            // If we are overwriting, then delete the current file.
            if (File.Exists(dbFilename))
            {
                File.Delete(dbFilename);
            }
            db.Open(dbFilename);
            logController.Open(Path.ChangeExtension(dbFilename, ".log"));
            options.LastFile = dbFilename;
            db.AddRace(race);
        }

        public void OpenRace(string dbFilename)
        {
            if (db.IsDbOpen)
            {
                db.Close();
            }
            db.Open(dbFilename);
            logController.Open(Path.ChangeExtension(dbFilename, ".log"));
            options.LastFile = dbFilename;
        }

        /// <summary>
        /// Gets the current race.
        /// </summary>
        public Race CurrentRace
        {
            get
            {
                var races = db.GetRaces();
                // If we don't yet have a race, create a new one.
                if (races.Count == 0)
                {
                    var race = new Race { Name = "-- New Race --" };
                    db.AddRace(race);
                    return race;
                }

                // We should only have one race - current logic imposes this.
                // DB should in principle support having multiple races.
                if (races.Count > 1)
                {
                    throw new Exception(string.Format("Should have only one race object. Currently have {0}.", races.Count));
                }
                return races[0];
            }
        }

        public void UpdateCurrentRace(Race newRaceDetails)
        {
            db.UpdateRace(CurrentRace.Oid, newRaceDetails);
        }

        private void ResultsQueueOnNewResult(object sender, EventArgs eventArgs)
        {
            OnResultDataChange();
        }

        private void OnResultDataChange()
        {
            if ( ResultDataChange != null )
            {
                ResultDataChange( this, new EventArgs() );
            }
        }

        public IList<Runner> GetRunners()
        {
            return db.GetRunners();
        }

        public IList<Result> GetResults()
        {
            return db.GetResults();
        }

        /// <summary>
        /// Loads a CSV file containing race entrant information.
        /// </summary>
        public dynamic LoadCsvFile(string filename)
        {
            // ToDo: Long term, make this more configurable. At the moment we're only interested
            //       in Runner's World files.

            dynamic result = new ExpandoObject();
            result.Imported = 0;
            result.AlreadyExisting = 0;
            result.Ignored = 0;

            using (var reader = new StreamReader(File.OpenRead(filename)))
            {
                string line;
                // Count number of lines - can specify which line throws error
                var lineCount = 0;

                // Do process while line is not empty
                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;
						
                    // If line throws exception then skip
                    try
                    {
                        // Split line into array of info (\\s* removes space around object)
                        var runnerInfo = line.Split(@"\\s*,\\s*".ToCharArray()).Select( s => s.Replace( "\"", "" ).Trim() ).ToArray();

                        var runner = new Runner
                        {
                            FirstName = runnerInfo[0], 
                            LastName = runnerInfo[1],
                            Gender = (runnerInfo[2] == "F") ? GenderEnum.Female : GenderEnum.Male,
                            DateOfBirth = DateParser.ParseRwDate( runnerInfo[4] ),
                            Club = runnerInfo[5],
                            Address = runnerInfo
                                .Skip(8).Take(6)
                                .Where( s => !string.IsNullOrEmpty( s ) )
                                .Aggregate( (current, next) => current + ", " + next ),
                           Number = db.GetNextNumber(),
                        };

                        // Check that they don't already exist in the DB (use firstname, lastname and DoB)
                        if (!db.TestDuplicate(runner))
                        {
                            db.AddRunner(runner);
                            result.Imported++;
                        }
                        else
                        {
                            result.AlreadyExisting++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf( traceSwitch.TraceWarning,
                            string.Format( "Error with line {0}: {1}. Line is '{2} ", lineCount, ex.Message, line ) );
                        result.Ignored++;
                    }
                }		
            }
            return result;
        }

        /// <summary>
        /// Delete all results, but not runner info.
        /// </summary>
        public void DeleteResultData()
        {
            db.DeleteResultData();
        }

        /// <summary>
        /// Adds a new race result time.
        /// </summary>
        public void AddResultTime()
        {
            resultQueue.Enqueue(new ResultsQueue.ResultSlot { datetime = clockTime.CurrentTime });
        }

        /// <summary>
        /// When runners arrive, add this runner number.
        /// </summary>
        public void AddResultRunnerNumber(int number)
        {
            db.AddResultNumber( number );
            OnResultDataChange();
        }
    }
}
