﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
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

            // Initial check of results
            if ( db.IsDbOpen )
            {
                db.CheckResults();
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

        public void SaveClockTime()
        {
            if ( db.IsDbOpen )
            {
                db.SaveClockTime( clockTime.CurrentTime, CurrentRace.Oid );
            }
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
            clockTime.CurrentTime = CurrentRace.ClockTime;
            options.LastFile = dbFilename;
        }

        public string BackupDb()
        {
            return !db.IsDbOpen ? null : db.BackupDb();
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
                        var runnerInfo = SplitCSV( line ).ToArray();

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

        /// <summary>
        /// Splits the CVS file line into fields, delimited on commas. The subtelty is that a cell may contain
        /// commas, and so it is surrounded/escaped by quotes, e.g. "some, thing".
        /// </summary>
        private IEnumerable<string> SplitCSV(string line)
        {
            var s = new StringBuilder();
            bool escaped = false, inQuotes = false;
            foreach (char c in line)
            {
                if (c == ',' && !inQuotes)
                {
                    yield return s.ToString();
                    s.Clear();
                }
                else if (c == '\\' && !escaped)
                {
                    escaped = true;
                }
                else if (c == '"' && !escaped)
                {
                    inQuotes = !inQuotes;
                }
                else
                {
                    escaped = false;
                    s.Append(c);
                }
            }
            yield return s.ToString();
        }

        private string RemoveUnwantedAttributes(string field)
        {
            var attrs = new[] {"none", "n/a"};
            return attrs.Any( attr => field.Equals( attr, StringComparison.InvariantCultureIgnoreCase ) ) ? null : field;
        }

        /// <summary>
        /// Deletes the result time at the given position, even if it has a finishing position.
        /// </summary>
        public void DeleteResultAtPosition( int pos )
        {
            db.DeleteResultAtPosition( pos );
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
        /// Deletes the result number at the given position. This has the effect of moving all following result 
        /// numbers up a position, i.e. giving them one better time.
        /// </summary>
        public void DeleteRunnerNumberShiftDown(int position)
        {
            db.DeleteRunnerNumberShiftDown( position );
            OnResultDataChange();
        }

        /// <summary>
        /// Deletes the time at the given position. This has the effect of moving all following result times down
        /// i.e. giving runners the next slower time.
        /// </summary>
        public void DeleteTimeShiftDown(int position)
        {
            db.DeleteTimeShiftDown(position);
            OnResultDataChange();
        }

        /// <summary>
        /// Inserts a new result. It contains the position at which it should be inserted, so all
        /// the remaining results are shuffled down.
        /// </summary>
        public void InsertResult( Result result )
        {
            db.InsertResult( result );
            OnResultDataChange();
        }

        /// <summary>
        /// Updates a result. The ID of the result to update is given in its position.
        /// </summary>
        public void UpdateResult( Result result )
        {
            db.UpdateResult( result );
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
            try
            {
                db.AddResultNumber( number );
            }
            catch ( NoMoreResultsException )
            {
                // Create new result and add it with estimated time - assume 5 seconds to travel up the funnel.
                var newResult = new Result
                {
                    Position = db.GetNextPosition(),
                    RaceId = db.GetNextPosition(),
                    RaceNumber = number,
                    Time = clockTime.CurrentTime.Add( new TimeSpan( 0, 0, -5 ) )
                };
                Result.AddDubiousReason( newResult, Result.DubiousResultEnum.EstimatedTime );
                db.AddResultTime( newResult );  // This adds the full result.
                db.CheckResults();
            }
            OnResultDataChange();
        }

        /// <summary>
        /// Gets all finishers and associated data
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

        public IList<TeamResult> GetTeamResults()
        {
            var finishers = GetFinishers();
            var teamResults = TeamCalc.GetTeamResults( finishers ).OrderBy( tr => tr.Score );
            return teamResults.ToList();
        }

        public RaceStats GetRaceStats()
        {
            var raceStats = new RaceStats
            {
                NumberEntrants = new RaceStats.RunnerBreakdown
                {
                    Total = GetRunners().Count,
                    Male = GetRunners().Count( r => r.Gender == GenderEnum.Male ),
                    Female = GetRunners().Count( r => r.Gender == GenderEnum.Female )
                },

                NumberFinishers = new RaceStats.RunnerBreakdown
                {
                    Total = GetFinishers().Count,
                    Male = GetFinishers().Count( f => f.Gender == GenderEnum.Male ),
                    Female = GetFinishers().Count( f => f.Gender == GenderEnum.Female )
                },

                NumberAffiliatedEntrants = new RaceStats.RunnerBreakdown
                {
                    Total = GetRunners().Count( r => !string.IsNullOrEmpty( r.Club.Trim() ) ),
                    Male = GetRunners().Count( r => !string.IsNullOrEmpty( r.Club.Trim() ) && r.Gender == GenderEnum.Male ),
                    Female = GetRunners().Count( r => !string.IsNullOrEmpty( r.Club.Trim() ) && r.Gender == GenderEnum.Female ),
                },

                NumberUnaffiliatedEntrants = new RaceStats.RunnerBreakdown
                {
                    Total = GetRunners().Count( r => string.IsNullOrEmpty( r.Club.Trim() ) ),
                    Male = GetRunners().Count( r => string.IsNullOrEmpty( r.Club.Trim() ) && r.Gender == GenderEnum.Male ),
                    Female = GetRunners().Count( r => string.IsNullOrEmpty( r.Club.Trim() ) && r.Gender == GenderEnum.Female ),
                }
            };
            return raceStats;
        }
    }
}
