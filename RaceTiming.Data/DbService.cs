
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using RedRat.RaceTiming.Data.Model;
using Volante;

namespace RedRat.RaceTiming.Data
{
    /// <summary>
    /// Manages the race data, interacting with the database.
    /// We use the Volante embedded DB - http://blog.kowalczyk.info/software/volante/database.html.
    /// Note: For Mono, Volante.dll has been built with the "MONO" flag as it otherwise uses the
    ///       "FlushFileBuffers " system call, which isn't support on the Mac.
    /// </summary>
    public class DbService : IDisposable
    {
        public static TraceSwitch dbTraceSwitch = new TraceSwitch("RTDB", "Race Timing DB");

        private IDatabase db;
        private string dbFilename;
        private RtDatabaseRoot dbRoot;
        private bool isDbOpen;
        private object dbLock = new object();

        public DbService()
        {
            isDbOpen = false;
        }

        public void Dispose()
        {
            Close();
        }

        public void Open(string filename)
        {
            if ( db != null )
            {
                throw new Exception( string.Format( "The database file '{0}' is already open", dbFilename ) );
            }

            lock ( dbLock )
            {
                db = DatabaseFactory.CreateDatabase();
                db.Open( filename );

                if ( db.Root == null )
                {
                    // Only create root the first time
                    dbRoot = new RtDatabaseRoot();
                }
                else
                {
                    dbRoot = (RtDatabaseRoot)db.Root;
                }
                CheckAndCreateIndexes();
                db.Root = dbRoot;
                db.Commit();

                dbFilename = filename;
                isDbOpen = true;
                Trace.WriteLineIf(dbTraceSwitch.TraceInfo, "DB file opened: " + dbFilename);
            }
        }

        /// <summary>
        /// Indexes should already be in place, but if not then this will create them. It means we can update
        /// existing DB files if more are added.
        /// </summary>
        private void CheckAndCreateIndexes()
        {
            lock ( dbLock )
            {
                if ( dbRoot.raceNameIndex == null )
                {
                    dbRoot.raceNameIndex = db.CreateIndex<string, Race>( IndexType.NonUnique );
                }
                if ( dbRoot.runnerFirstNameIndex == null )
                {
                    dbRoot.runnerFirstNameIndex = db.CreateIndex<string, Runner>( IndexType.NonUnique );
                }
                if ( dbRoot.runnerLastNameIndex == null )
                {
                    dbRoot.runnerLastNameIndex = db.CreateIndex<string, Runner>( IndexType.NonUnique );
                }
                if (dbRoot.runnerNumberIndex == null)
                {
                    dbRoot.runnerNumberIndex = db.CreateIndex<int, Runner>(IndexType.NonUnique);
                }
                if ( dbRoot.resultPositionIndex == null )
                {
                    dbRoot.resultPositionIndex = db.CreateIndex<int, Result>( IndexType.NonUnique );
                }
            }
        }

        private void CheckHaveDb()
        {
            if ( !IsDbOpen )
            {
                throw new NoDatabaseException( "No database." );
            }
        }

        public string BackupDb()
        {
            if (!IsDbOpen) return null;

            var fileInfo = new FileInfo( dbFilename );
            var timestamp = DateTime.Now.ToString( "dd-MM-yyyy HH.mm.ss" );
            var backupFilename = fileInfo.FullName.Replace( 
                fileInfo.Extension, 
                string.Format(" Bck [{0}]{1}", timestamp, fileInfo.Extension) );

            using ( var stream = new FileStream( backupFilename, FileMode.Create ) )
            {
                db.Backup( stream );
                stream.Flush();
                stream.Close();
            }

            Trace.WriteLineIf( dbTraceSwitch.TraceWarning, "Database backed up to: " + backupFilename );
            return backupFilename;
        }

        public void Close()
        {
            if ( db == null ) return;

			lock (dbLock)
			{
				db.Close();
				isDbOpen = false;
				db = null;
			}
        }

        /// <summary>
        /// Gets whether we have a DB open and in use.
        /// </summary>
        public bool IsDbOpen
        {
            get { return isDbOpen; }
        }

        #region Race Methods

        public void AddRace( Race race )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                dbRoot.raceNameIndex.Put( race.Name, race );
                db.Commit();
            }
        }

        public IList<Race> GetRaces()
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                return dbRoot.raceNameIndex.ToList();
            }
        }

        public void UpdateRace( int oid, Race newDetails )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var race = GetRaces().FirstOrDefault( r => r.Oid == oid );
                if ( race == null )
                {
                    throw new Exception( "No race object with oid " + oid );
                }
                race.Name = newDetails.Name;
                race.Description = newDetails.Description;
                race.Distance = newDetails.Distance;
                race.Date = newDetails.Date;
                race.Modify();  // Inform DB that it's been modified
                db.Commit();    // Push changes back to DB
            }
        }

        public void SaveClockTime( TimeSpan clockTime, int raceOid )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var race = GetRaces().FirstOrDefault( r => r.Oid == raceOid );
                if ( race == null )
                {
                    throw new Exception( "No race object with oid " + raceOid );
                }
                race.ClockTime = clockTime;
                race.Modify();
                db.Commit();
            }

        }

        #endregion

        #region Runner Methods

        public void AddRunner(Runner runner)
        {
            CheckHaveDb();
            lock (dbLock)
            {
                dbRoot.runnerFirstNameIndex.Put(runner.FirstName, runner);
                dbRoot.runnerLastNameIndex.Put(runner.LastName, runner);
                dbRoot.runnerNumberIndex.Put( runner.Number, runner );
                db.Commit();
            }
        }

        public void UpdateRunner( Runner updatedRunner )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var runner = dbRoot.runnerNumberIndex.FirstOrDefault( r => r.Number == updatedRunner.Number );
                if ( runner == null )
                {
                    throw new Exception( "Unable to find runner with number: " + updatedRunner.Number );
                }
                runner.FirstName = updatedRunner.FirstName;
                runner.LastName = updatedRunner.LastName;
                runner.Gender = updatedRunner.Gender;
                runner.DateOfBirth = updatedRunner.DateOfBirth;
                runner.Email = updatedRunner.Email;
                runner.Club = updatedRunner.Club;
                runner.Team = updatedRunner.Team;
                runner.Urn = updatedRunner.Urn;
                // ToDo: Affiliated flag

                runner.Modify();
                db.Commit();
            }
        }

        public void DeleteRunner( int number )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var runnerToDelete = dbRoot.runnerNumberIndex.FirstOrDefault( r => r.Number == number );
                if ( runnerToDelete == null )
                {
                    throw new Exception( "Unable to find a runner with this number." );
                }
                dbRoot.runnerNumberIndex.Remove( runnerToDelete.Number, runnerToDelete );
                dbRoot.runnerFirstNameIndex.Remove( runnerToDelete.FirstName, runnerToDelete );
                dbRoot.runnerLastNameIndex.Remove( runnerToDelete.LastName, runnerToDelete );
                db.Commit();
            }
        }

        public IList<Runner> GetRunners()
        {
            CheckHaveDb();
            lock (dbLock)
            {
                return dbRoot.runnerFirstNameIndex.ToList();
            }            
        }

		// Tests whether runner already exists in db using name & DoB
		public bool TestDuplicate(Runner runner)
		{
			CheckHaveDb ();
			var runners = GetRunners ();
			for (var i = 0; i < runners.Count(); i++) {
				if (runner.FirstName.Equals( runners [i].FirstName, StringComparison.CurrentCultureIgnoreCase ) &&
				    runner.LastName.Equals( runners [i].LastName, StringComparison.CurrentCultureIgnoreCase ) &&
				    runner.DateOfBirth == runners [i].DateOfBirth) {
					return true;
				}
			}
			return false;
		}

        /// <summary>
        /// Returns the next bib number.
        /// </summary>
        public int GetNextNumber()
        {
            return GetRunners().Count + 1;
        }

        #endregion

        #region Result Methods

        public void AddResultTime( Result result )
        {
            CheckHaveDb();
            lock (dbLock)
            {
                dbRoot.resultPositionIndex.Put(result.Position, result);
                db.Commit();
            }
        }

        /// <summary>
        /// Adds a result number to the first result without a number.
        /// </summary>
        public void AddResultNumber( int number )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var nextResult = dbRoot.resultPositionIndex.OrderBy( r => r.Time ).FirstOrDefault( r => r.RaceNumber == 0 );
                if ( nextResult == null )
                {
                    throw new NoMoreResultsException( "No more results to add a number to." );
                }
                nextResult.RaceNumber = number;
                nextResult.Modify();
                db.Commit();
            }
            CheckResults();
        }

        public void CheckResults()
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var results = dbRoot.resultPositionIndex.ToList();
                var runners = dbRoot.runnerNumberIndex.ToList();

                // == Duplicate numbers ==
                // Clear
                var currentDups = results.Where( r => r.DubiousResult.HasFlag( Result.DubiousResultEnum.DuplicateNumber ) );
                foreach ( var duplicate in currentDups.Where( duplicate => Result.RemoveDubiousReason( duplicate, Result.DubiousResultEnum.DuplicateNumber ) ) ) {
                    duplicate.Modify();
                }

                // Find duplicates
                var duplicates = FindDuplicateResults( results );
                foreach ( var duplicate in duplicates.Where( duplicate => Result.AddDubiousReason( duplicate, Result.DubiousResultEnum.DuplicateNumber ) ) ) {
                    duplicate.Modify();
                }

                // == Unknown numbers ==
                // Clear
                var currentUnknownNums = results.Where(r => r.DubiousResult.HasFlag(Result.DubiousResultEnum.UnknownNumber));
                foreach (var unknwonNum in currentUnknownNums.Where(unknwonNum => Result.RemoveDubiousReason(unknwonNum, Result.DubiousResultEnum.UnknownNumber)))
                {
                    unknwonNum.Modify();
                }

                // Flag unknowns
                var unknownNums = FindUnknownNumbers(results, runners);
                foreach (var unknownNum in unknownNums.Where(unknownNum => Result.AddDubiousReason(unknownNum, Result.DubiousResultEnum.UnknownNumber)))
                {
                    unknownNum.Modify();
                }

                db.Commit();
            }
        }

        public static IList<Result> FindDuplicateResults(IList<Result> results)
        {
            // Selects any results which have a duplicate race number
            var duplicates = results
                .Where( r => r.RaceNumber != 0 )        // Ignore race number of 0 (no runner number added yet)
                .GroupBy( r => r.RaceNumber )
                .Where( g => g.Count() > 1 ).SelectMany( r => r );
            return duplicates.ToList();
        }

        public static IList<Result> FindUnknownNumbers(IList<Result> results, IList<Runner> runners)
        {
            var knownNumbers = runners.Select( r => r.Number );
            var unknownNums = results
                .Where(r => r.RaceNumber != 0)        // Ignore race number of 0 (no runner number added yet)
                .Where(r => !knownNumbers.Contains(r.RaceNumber));
            return unknownNums.ToList();
        }

        public IList<Result> GetResults()
        {
            CheckHaveDb();
            lock (dbLock)
            {
                return dbRoot.resultPositionIndex.ToList();
            }
        }

        public int GetNextPosition()
        {
            return GetResults().Count + 1;
        }

        public void DeleteResultData()
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                dbRoot.resultPositionIndex.Clear();
                db.Commit();
            }
        }

        public void UpdateResult( Result result )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var dbResult = dbRoot.resultPositionIndex.FirstOrDefault( r => r.Position == result.Position );
                if ( dbResult == null )
                {
                    throw new Exception("Unable to find a result for position " + result.Position );
                }

                // Check the times - this result can't be set to have a time before or after surrounding results.
                CheckBeforeAfterResult(dbRoot.resultPositionIndex.ToList(), result, insert: false);

                // If the result has an estimated time, and the time is edited, we then assume that it's no
                // longer as estimate
                if ( ( dbResult.Time != result.Time ) && dbResult.DubiousResult.HasFlag( Result.DubiousResultEnum.EstimatedTime ) )
                {
                    Result.RemoveDubiousReason( dbResult, Result.DubiousResultEnum.EstimatedTime );
                }

                dbResult.RaceNumber = result.RaceNumber;
                dbResult.Time = result.Time;
                dbResult.Modify();
                db.Commit();
            }

            CheckResults();
        }

        /// <summary>
        /// Deletes the result at the given position.
        /// </summary>
        public void DeleteResultAtPosition( int pos )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var resultIndex = dbRoot.resultPositionIndex;
                var result = resultIndex.FirstOrDefault( r => r.Position == pos );
                if ( result == null )
                {
                    var msg = string.Format( "No result found for position {0}", pos );
                    Trace.WriteLineIf( dbTraceSwitch.TraceWarning, msg );
                    throw new Exception( msg );
                }

                // Shuffle all the results down...
                var orderedResults = resultIndex.OrderBy( r => r.Position ).Where( r => r.Position >= pos ).ToList();
                for ( var i = 0; i < orderedResults.Count - 1; i++ )
                {
                    Result.TransferState( orderedResults[i], orderedResults[i + 1] );
                    orderedResults[i].Modify();
                }
                // Remove last result
                var lastResult = resultIndex[resultIndex.Max( r => r.Position )];
                resultIndex.Remove( lastResult.Position, lastResult );
                db.Commit();
            }

            CheckResults();
        }

        public void DeleteRunnerNumberShiftDown( int position )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var resultIndex = dbRoot.resultPositionIndex;
                var orderedResults = resultIndex.OrderBy(r => r.Position).ToList();

                for ( var i = position - 1; i < orderedResults.Count() - 1; i++ )
                {
                    orderedResults[i].RaceNumber = orderedResults[i + 1].RaceNumber;
                    orderedResults[i].Modify();
                }

                // The last result needs a 0 inserted
                orderedResults[orderedResults.Count() - 1].RaceNumber = 0;
            }
            CheckResults();
        }

        public void DeleteTimeShiftDown(int position)
        {
            CheckHaveDb();
            lock (dbLock)
            {
                var resultIndex = dbRoot.resultPositionIndex;
                var orderedResults = resultIndex.OrderBy(r => r.Position).ToList();

                for (var i = position - 1; i < orderedResults.Count() - 1; i++)
                {
                    orderedResults[i].Time = orderedResults[i + 1].Time;
                    orderedResults[i].Modify();
                }

                // The last result needs a time inserted, so what time do we give?
                // Just leave it as it was, but flag as "estimated"
                Result.AddDubiousReason( orderedResults[orderedResults.Count() - 1], Result.DubiousResultEnum.EstimatedTime );
            }
            CheckResults();
        }

        /// <summary>
        /// Insert result at the given position contained in the result.
        /// </summary>
        public void InsertResult(Result newResult)
        {
            CheckHaveDb();

            if (newResult == null)
            {
                throw new ArgumentNullException("result");
            }

            if (newResult.Position == 0)
            {
                throw new Exception("Result position can't be zero.");
            }

            lock (dbLock)
            {
                var resultIndex = dbRoot.resultPositionIndex;

                // Check the times - this result needs to be inserted between times.
                CheckBeforeAfterResult( resultIndex.ToList(), newResult, insert: true);

                // Add empty result at end
                var pos = GetNextPosition();

                if (newResult.Position > pos )
                {
                    throw new Exception("Cannot insert past end of result set.");
                }

                // Add empty result to end of list
                resultIndex.Put(pos, new Result { Position = pos });

                // Shuffle all the results up...
                var orderedResults = resultIndex.OrderBy(r => r.Position).Where(r => r.Position >= newResult.Position).ToList();
                for (var i = orderedResults.Count - 2; i >= 0; i-- )
                {
                    Result.TransferState(orderedResults[i + 1], orderedResults[i]);
                    orderedResults[i + 1].Modify();
                }

                // Update new result
                var resultToUpdate = resultIndex.First( r => r.Position == newResult.Position);
                Result.TransferState( resultToUpdate, newResult );
                resultToUpdate.Modify();

                db.Commit();
            }

            CheckResults();
        }

        /// <summary>
        /// When a result is inserted or updated, we check that it's in order. If being inserted, then
        /// check "after" result against current position. If editing, then check again next position.
        /// </summary>
        private void CheckBeforeAfterResult( IList<Result> results, Result resultToCheck, bool insert )
        {
            // Check the times - this result needs to be inserted between times.
            var insertPos = resultToCheck.Position;
            var beforeResult = results.FirstOrDefault(r => r.Position == insertPos - 1);
            if (beforeResult != null)
            {
                if (resultToCheck.Time < beforeResult.Time)
                {
                    throw new Exception(string.Format("Result can't have a time less than the previous result ({0:hh\\:mm\\:ss}).",
                        beforeResult.Time));
                }
            }
            var afterPos = ( insert ) ? insertPos : insertPos + 1;
            var afterResult = results.FirstOrDefault(r => r.Position == afterPos);
            if (afterResult != null)
            {
                if (resultToCheck.Time > afterResult.Time)
                {
                    throw new Exception(string.Format("Result can't have a time greater than the next result ({0:hh\\:mm\\:ss}).",
                        afterResult.Time));
                }
            }            
        }
        #endregion
    }
}
