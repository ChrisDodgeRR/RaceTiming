
using System;
using System.IO;
using RedRat.RaceTiming.Data;
using RedRat.RaceTiming.Data.Model;

namespace RedRat.RaceTiming.Core
{
    /// <summary>
    /// This is the main application controller. The UI is a thin wrapper round this.
    /// </summary>
    public class AppController
    {
        private DbController db;

        public AppController()
        {
            db = new DbController();
        }

        public bool IsDbOpen
        {
            get { return db.IsDbOpen; }
        }

        public void CreateNewRace( Race race, string dbFilename )
        {
            if ( db.IsDbOpen )
            {
                db.Close();
            }
            // If we are overwriting, then delete the current file.
            if ( File.Exists( dbFilename ) )
            {
                File.Delete( dbFilename );
            }
            db.Open( dbFilename );
            db.AddRace( race );
        }

        public void OpenRace( string dbFilename )
        {
            if ( db.IsDbOpen )
            {
                db.Close();
            }
            db.Open( dbFilename );
        }

        /// <summary>
        /// Gets the current race.
        /// </summary>
        public Race CurrentRace
        {
            get
            {
                var races = db.GetRaces();
                // We should only have one race - current logic imposes this.
                // DB should in principle support having multiple races.
                if ( races.Count != 1 )
                {
                    throw new Exception( string.Format( "Should have only one race object. Currently have {0}.", races.Count ) );
                }
                return races[0];
            }
        }

        public void UpdateCurrentRace( Race newRaceDetails )
        {
            db.UpdateRace( CurrentRace.Oid, newRaceDetails );
        }
    }
}
