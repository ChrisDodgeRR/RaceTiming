
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            const string dupResultMsg = "Duplicate runner number";

            CheckHaveDb();
            lock ( dbLock )
            {
                // First check to see if this number is a duplicate
                var resultsWithNumber = dbRoot.resultPositionIndex.Where( r => r.RaceNumber == number ).ToList();
                var haveDuplicate = false;
                foreach (var result in resultsWithNumber)
                {
                    haveDuplicate = true;
                    result.DubiousResult = true;
                    result.AppendReason(dupResultMsg);
                    result.Modify();
                }

                var nextResult = dbRoot.resultPositionIndex.OrderBy( r => r.Time ).FirstOrDefault( r => r.RaceNumber == 0 );
                if ( nextResult == null )
                {
                    throw new Exception("No more results to add numbers to.");
                }
                nextResult.RaceNumber = number;
                if ( haveDuplicate )
                {
                    nextResult.DubiousResult = true;
                    nextResult.AppendReason( dupResultMsg );
                }
                nextResult.Modify();

                db.Commit();
            }
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

        /// <summary>
        /// Deletes the result at the given position.
        /// </summary>
        /// <param name="pos">Finishing position of result to delete</param>
        /// <param name="deleteNumber">Should the finishing race be deleted as well? If not, then the race numbers are shuffled up.</param>
        public void DeleteResultAtPosition( int pos, bool deleteNumber )
        {
            CheckHaveDb();
            lock ( dbLock )
            {
                var result = dbRoot.resultPositionIndex.FirstOrDefault( r => r.Position == pos );
                if ( result == null )
                {
                    var msg = string.Format( "No result found for position {0}", pos );
                    Trace.WriteLineIf( dbTraceSwitch.TraceWarning, msg );
                    throw new Exception( msg );
                }
                if ( result.RaceNumber == 0 )
                {
                    // No associated result race number, so just delete the time.
                    DeleteResultTimeAtPosition(dbRoot.resultPositionIndex, pos);
                    db.Commit();
                }
            }
        }

        public static void DeleteResultTimeAtPosition( IIndex<int, Result> resultIndex, int position )
        {
            var orderedResults = resultIndex.ToList().OrderBy( r => r.Position ).Where( r => r.Position >= position ).ToList();
            for ( var i = 0; i < orderedResults.Count - 1; i++ )
            {
                // ToDo: Also copy race number???
                orderedResults[i].Time = orderedResults[i + 1].Time;
                orderedResults[i].Modify();
            }
            // Remove last result
            var lastResult = resultIndex[resultIndex.Max( r => r.Position )];
            resultIndex.Remove( lastResult.Position, lastResult );
        }
        #endregion
    }
}
