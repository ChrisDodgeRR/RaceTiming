
using System;
using System.IO;
using RedRat.RaceTiming.Data;
using RedRat.RaceTiming.Data.Model;
using Xunit;

namespace RaceTimingTests.Data
{
    public class DbServiceTests
    {
        private DbService CreateTestDb()
        {
            const string filename = "UnitTestDb.dbs";
            if ( File.Exists( filename ) )
            {
                File.Delete( filename );
            }

            var db = new DbService();
            db.Open(filename);

            // Add data
            for (var i = 0; i < 10; i++)
            {
                db.AddResultTime(new Result { Position = i + 1, Time = new TimeSpan(0, 0, i + 1, 0) });
            }

            return db;
        }

        [Fact]
        public void CanDeleteResultPosition()
        {
            using ( var db = CreateTestDb() )
            {
                db.DeleteResultAtPosition( 3, false );

                var results = db.GetResults();
                Assert.Equal(9, results.Count);     // One less result

                for (var i = 0; i < results.Count; i++)
                {
                    var result = results[i];
                    Assert.Equal( i + 1, result.Position );
                    if ( result.Position < 3 )
                    {
                        Assert.Equal( i + 1, result.Time.Minutes );
                    }
                    else
                    {
                        // Results at pos 3 and above have a time one min larger.
                        Assert.Equal( i + 2, result.Time.Minutes );
                    }
                }
            }
        }

        [Fact]
        public void CanDeleteLastResultPosition()
        {
            using ( var db = CreateTestDb() )
            {
                db.DeleteResultAtPosition( 10, false );

                var results = db.GetResults();
                Assert.Equal( 9, results.Count ); // One less result

                for ( var i = 0; i < results.Count; i++ )
                {
                    var result = results[i];
                    Assert.Equal( i + 1, result.Position );
                    Assert.Equal( i + 1, result.Time.Minutes );
                }
            }
        }

        [Fact]
        public void CanDeleteFirstResultPosition()
        {
            using (var db = CreateTestDb())
            {
                db.DeleteResultAtPosition(1, false);

                var results = db.GetResults();
                Assert.Equal(9, results.Count);     // One less result

                for (var i = 0; i < results.Count; i++)
                {
                    var result = results[i];
                    Assert.Equal(i + 1, result.Position);
                    Assert.Equal(i + 2, result.Time.Minutes);
                }
            }            
        }

        [Fact]
        public void DeletingInvalidPositionThrowsEx()
        {
            using ( var db = CreateTestDb() )
            {
                Assert.Throws<Exception>( () => db.DeleteResultAtPosition( 0, false ) );
                Assert.Throws<Exception>( () => db.DeleteResultAtPosition( 20, false ) );
            }
        }
    }
}
