
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml;
using RedRat.RaceTiming.Core.Config;
using RedRat.RaceTiming.Core.Logging;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Core.ViewModels;
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
        private bool clockRunning;

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

            clockTime.ClockRunningHandler += (s, b) => clockRunning = b;
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

        public DbService DbService
        {
            get { return db; }
        }

        public ClockTime ClockTime
        {
            get { return clockTime; }
        }

        public bool IsClockRunning
        {
            get { return clockRunning; }
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
                        var runnerInfo = line.Split(@",".ToCharArray()).Select( s => s.Replace( "\"", "" ).Trim() ).ToArray();

                        var runner = new Runner
                        {
                            FirstName = runnerInfo[0].ToUpper(),
                            LastName = runnerInfo[1].ToUpper(),
                            Gender = ( runnerInfo[2] == "F" ) ? GenderEnum.Female : GenderEnum.Male,
                            DateOfBirth = DateParser.ParseRwDate( runnerInfo[4] ),
                            Club = RemoveUnwantedAttributes( runnerInfo[5] ),
                            Team = RemoveUnwantedAttributes( runnerInfo[6] ),
                            Email = ( runnerInfo.Count() > 15 ) ? RemoveUnwantedAttributes( runnerInfo[15] ) : null,
                            Urn = ( runnerInfo.Count() > 33 ) ? RemoveUnwantedAttributes( runnerInfo[33] ) : null,
                            Number = db.GetNextNumber(),
                        };

                        var addressFields = runnerInfo
                                .Skip( 8 ).Take( 6 )
                                .Where( s => !string.IsNullOrEmpty( s ) ).ToList();

                        if ( addressFields.Any() )
                        {
                            runner.Address = addressFields.Aggregate( ( current, next ) => current + ", " + next );
                        }

                        // If a club is given, then assume the runner is affiliated
                        runner.Affiliated = !string.IsNullOrEmpty( runner.Club );

                        // ToDo: Check for invalid team names - 'NONE' or 'N/A'.

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

        private string RemoveUnwantedAttributes(string field)
        {
            var attrs = new[] {"none", "n/a"};
            return attrs.Any( attr => field.Equals( attr, StringComparison.InvariantCultureIgnoreCase ) ) ? null : field;
        }

        /// <summary>
        /// Deletes the result time at the given position. If it contains a finishing number
        /// then they are shuffled down.
        /// </summary>
        public void DeleteResultTimeAtPosition( int pos )
        {
            db.DeleteResultAtPosition( pos, false );
            OnResultDataChange();
        }

        /// <summary>
        /// Delete all results, but not runner info.
        /// </summary>
        public void DeleteResultData()
        {
            db.DeleteResultData();
            OnResultDataChange();
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

        /// <summary>
        /// Gats all finishers and associated data
        /// </summary>
        public IList<Finisher> GetFinishers()
        {
            var catPostions = new Dictionary<AgeGroup.AgeGroupEnum, int>();

            var finishers = GetResults()
                .Where(r => r.RaceNumber != 0)
                .OrderBy(r => r.Position)
                .Select(r => new Finisher
                {
                    Position = r.Position,
                    Number = r.RaceNumber,
                    Time = r.Time.TotalMilliseconds,
                }).ToList();

            var runners = GetRunners();

            foreach (var finisher in finishers)
            {
                var runner = runners.FirstOrDefault(r => r.Number == finisher.Number);
                if (runner != null)
                {
                    finisher.Name = string.Format("{0} {1}", runner.FirstName, runner.LastName);
                    finisher.Club = runner.Club;
                    finisher.Team = runner.Team;
                    finisher.Gender = runner.Gender;

                    // Age group and category position
                    var cat = AgeGroup.GetAgeGroup(CurrentRace.Date, runner.DateOfBirth, runner.Gender);
                    finisher.Category = cat.ToString();
                    finisher.CategoryEnum = cat;
                    if (!catPostions.ContainsKey(cat))
                    {
                        catPostions.Add(cat, 1);
                    }
                    finisher.CategoryPosition = catPostions[cat].ToString();
                    catPostions[cat] = ++catPostions[cat];

                    // WMA score
                    var time = TimeSpan.FromMilliseconds(finisher.Time);
                    finisher.Wma = string.Format("{0:F2}%",
                        (WmaCalculator.CalcWma(AgeGroup.GetAgeOnDate(CurrentRace.Date, runner.DateOfBirth),
                            runner.Gender, CurrentRace.Distance, time.Hours, time.Minutes, time.Seconds) * 100));
                }
            }
            return finishers;
        }

        public Dictionary<string, IList<Finisher>> GetWinners()
        {
            var winners = new Dictionary<string, IList<Finisher>>();
            var alreadyGotAPrize = new List<Finisher>();
            var finishers = GetFinishers();

            // Top males - from all categories
            var allMales = new[]
            {
                AgeGroup.AgeGroupEnum.M, AgeGroup.AgeGroupEnum.MV40, AgeGroup.AgeGroupEnum.MV50,
                AgeGroup.AgeGroupEnum.MV60, AgeGroup.AgeGroupEnum.MV70
            };
            var catWinners = GetCategoryWinners( GenderEnum.Male, finishers, allMales, alreadyGotAPrize, 3 );
            winners.Add( AgeGroup.AgeGroupEnum.M.ToString(), catWinners );

            // Top females - from all categories
            var allFemales = new[]
            {
                AgeGroup.AgeGroupEnum.F, AgeGroup.AgeGroupEnum.FV40, AgeGroup.AgeGroupEnum.FV50,
                AgeGroup.AgeGroupEnum.FV60, AgeGroup.AgeGroupEnum.FV70
            };
            catWinners = GetCategoryWinners( GenderEnum.Female, finishers, allFemales, alreadyGotAPrize, 3 );
            winners.Add( AgeGroup.AgeGroupEnum.F.ToString(), catWinners );

            // Male V40
            catWinners = GetCategoryWinners( GenderEnum.Male, finishers, new[] {AgeGroup.AgeGroupEnum.MV40}, alreadyGotAPrize, 1 );
            winners.Add( AgeGroup.AgeGroupEnum.MV40.ToString(), catWinners );

            // Female V40
            catWinners = GetCategoryWinners( GenderEnum.Female, finishers, new[] {AgeGroup.AgeGroupEnum.FV40}, alreadyGotAPrize, 1 );
            winners.Add( AgeGroup.AgeGroupEnum.FV40.ToString(), catWinners );

            // Male V50
            catWinners = GetCategoryWinners( GenderEnum.Male, finishers, new[] {AgeGroup.AgeGroupEnum.MV50}, alreadyGotAPrize, 1 );
            winners.Add( AgeGroup.AgeGroupEnum.MV50.ToString(), catWinners );

            // Female V50
            catWinners = GetCategoryWinners( GenderEnum.Female, finishers, new[] {AgeGroup.AgeGroupEnum.FV50}, alreadyGotAPrize, 1 );
            winners.Add( AgeGroup.AgeGroupEnum.FV50.ToString(), catWinners );

            // Male V60
            catWinners = GetCategoryWinners( GenderEnum.Male, finishers, 
                new[] {AgeGroup.AgeGroupEnum.MV60, AgeGroup.AgeGroupEnum.MV70}, alreadyGotAPrize, 1 );
            winners.Add( AgeGroup.AgeGroupEnum.MV60.ToString(), catWinners );

            // Female V60
            catWinners = GetCategoryWinners( GenderEnum.Female, finishers, 
                new[] {AgeGroup.AgeGroupEnum.FV60, AgeGroup.AgeGroupEnum.FV70}, alreadyGotAPrize, 1 );
            winners.Add( AgeGroup.AgeGroupEnum.FV60.ToString(), catWinners );

            // ToDo: Add team result.

            var teams = GetTeams();

            return winners;
        }

        /// <summary>
        /// Gets the winners for a given category.
        /// </summary>
        /// <param name="gender">Gender of this category.</param>
        /// <param name="finishers">The full list of race finishers</param>
        /// <param name="categories">The age categories to be included.</param>
        /// <param name="alreadyGotAPrize">If a runner has won a prize in another category, don't give them one in this.</param>
        /// <param name="numberPrizesInCategory">How many prizes in this category</param>
        private List<Finisher> GetCategoryWinners(
            GenderEnum gender,
            IEnumerable<Finisher> finishers,
            IList<AgeGroup.AgeGroupEnum> categories,
            List<Finisher> alreadyGotAPrize, int numberPrizesInCategory )
        {
            var catWinners = finishers
                .Where( f => f.Gender == gender )
                .Where( f => categories.Contains( f.CategoryEnum ) )
                .Where( f => !alreadyGotAPrize.Contains( f ) ) // Has this person already got a prize?
                .OrderBy( f => f.Position ).Take( numberPrizesInCategory ).ToList();

            alreadyGotAPrize.AddRange( catWinners );
            return catWinners;
        }

        /// <summary>
        /// Extracts teams from the entry list.
        /// </summary>
        private List<string> GetTeams()
        {
            // 1. Find teams - needs 4 or more results
            var teamList = db.GetRunners().Select( r => r.Team ).Distinct();

            // 2. For runners not already in valid teams, find clubs with 4 or more runners

            return teamList.ToList();
        }
    }
}
