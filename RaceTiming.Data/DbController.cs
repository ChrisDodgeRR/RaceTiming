
using System;
using System.Collections.Generic;
using System.Linq;
using RedRat.RaceTiming.Data.Model;
using Volante;

namespace RedRat.RaceTiming.Data
{
    /// <summary>
    /// Manages the race data, interacting with the database.
    /// We use the Volante embedded DB - http://blog.kowalczyk.info/software/volante/database.html.
    /// </summary>
    public class DbController : IDisposable
    {
        private IDatabase db;
        private string dbFilename;
        private DatabaseRoot dbRoot;
        private bool isDbOpen;
        private object dbLock = new object();

        public DbController()
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

                if ( db.Root != null )
                {
                    dbRoot = (DatabaseRoot) db.Root;
                }
                else
                {
                    // Only create root the first time
                    CreateDb();
                }
                dbFilename = filename;
                isDbOpen = true;
            }
        }

        private void CreateDb()
        {
            lock ( dbLock )
            {
                dbRoot = new DatabaseRoot
                {
                    raceNameIndex = db.CreateIndex<string, Race>( IndexType.NonUnique ),
                    runnerFirstNameIndex = db.CreateIndex<string, Runner>( IndexType.NonUnique ),
                };
                db.Root = dbRoot;
                // changing the root marks database as modified but it's
                // only modified in memory. Commit to persist changes to disk.
                db.Commit();
            }
        }

        public void Close()
        {
            if ( db == null ) return;

            db.Close();
            isDbOpen = false;
            db = null;
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
            lock ( dbLock )
            {
                dbRoot.raceNameIndex.Put( race.Name, race );
                db.Commit();
            }
        }

        public IList<Race> GetRaces()
        {
            lock ( dbLock )
            {
                return dbRoot.raceNameIndex.ToList();
            }
        }

        public void UpdateRace( int oid, Race newDetails )
        {
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
                race.Modify(); // Inform DB that it's been modified
                db.Commit(); // Push changes back to DB
            }
        }

        #endregion


        public void TestInsertData()
        {
            var runner = new Runner {FirstName = "Chris", LastName = "Dodge"};
            dbRoot.runnerFirstNameIndex.Put( runner.FirstName, runner );
            db.Commit();
        }

        public void PrintRunners()
        {
            foreach (var runner in dbRoot.runnerFirstNameIndex)
            {
                Console.WriteLine("{0}: {1}", runner.FirstName, runner.LastName);
            }
        }
    }
}
