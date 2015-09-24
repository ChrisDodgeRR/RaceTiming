
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                db.AddResultTime(new Result
                {
                    Position = i + 1, 
                    Time = new TimeSpan(0, 0, i + 1, 0), 
                    RaceNumber = i + 1, 
                    WmaScore = i + 1
                });
            }

            return db;
        }

        [Fact]
        public void CanInsertResult()
        {
            foreach ( var insertPos in new[] {1, 4, 10, 11} )
            {
                using ( var db = CreateTestDb() )
                {
                    // Test first, middle and end
                    db.InsertResult( new Result
                    {
                        Position = insertPos,
                        RaceNumber = 20,
                        Time = new TimeSpan( 0, 20, 0 ),
                        WmaScore = 20,
                    } );

                    var results = db.GetResults();
                    Assert.Equal( 11, results.Count ); // One less result

                    // Ensure only one with this position
                    var newResults = results.Where( r => r.Position == insertPos ).ToList();
                    Assert.Equal( 1, newResults.Count );

                    for ( var i = 0; i < results.Count; i++ )
                    {
                        var result = results[i];
                        Assert.Equal( i + 1, result.Position );
                        if ( result.Position < insertPos )
                        {
                            Assert.Equal( i + 1, result.Time.Minutes );
                            Assert.Equal( i + 1, result.RaceNumber );
                            Assert.Equal( i + 1, result.WmaScore );
                        }
                        else if ( result.Position == insertPos )
                        {
                            Assert.Equal( 20, result.Time.Minutes );
                            Assert.Equal( 20, result.RaceNumber );
                            Assert.Equal( 20, result.WmaScore );
                        }
                        else if ( result.Position > insertPos )
                        {
                            // Results at pos 3 and above have a time one min larger.
                            Assert.Equal( i, result.Time.Minutes );
                            Assert.Equal( i, result.RaceNumber );
                            Assert.Equal( i, result.WmaScore );
                        }
                    }
                }
            }
        }

        [Fact]
        public void CanDeleteResultPosition()
        {
            using ( var db = CreateTestDb() )
            {
                db.DeleteResultAtPosition( 3 );

                var results = db.GetResults();
                Assert.Equal(9, results.Count);     // One less result

                for (var i = 0; i < results.Count; i++)
                {
                    var result = results[i];
                    Assert.Equal( i + 1, result.Position );
                    if ( result.Position < 3 )
                    {
                        Assert.Equal( i + 1, result.Time.Minutes );
                        Assert.Equal( i + 1, result.RaceNumber );
                        Assert.Equal( i + 1, result.WmaScore );
                    }
                    else
                    {
                        // Results at pos 3 and above have a time one min larger.
                        Assert.Equal( i + 2, result.Time.Minutes );
                        Assert.Equal( i + 2, result.RaceNumber );
                        Assert.Equal( i + 2, result.WmaScore );
                    }
                }
            }
        }

        [Fact]
        public void CanDeleteLastResultPosition()
        {
            using ( var db = CreateTestDb() )
            {
                db.DeleteResultAtPosition( 10 );

                var results = db.GetResults();
                Assert.Equal( 9, results.Count ); // One less result

                for ( var i = 0; i < results.Count; i++ )
                {
                    var result = results[i];
                    Assert.Equal( i + 1, result.Position );
                    Assert.Equal( i + 1, result.Time.Minutes );
                    Assert.Equal( i + 1, result.RaceNumber );
                    Assert.Equal( i + 1, result.WmaScore );
                }
            }
        }

        [Fact]
        public void CanDeleteFirstResultPosition()
        {
            using ( var db = CreateTestDb() )
            {
                db.DeleteResultAtPosition( 1 );

                var results = db.GetResults();
                Assert.Equal( 9, results.Count ); // One less result

                for ( var i = 0; i < results.Count; i++ )
                {
                    var result = results[i];
                    Assert.Equal( i + 1, result.Position );
                    Assert.Equal( i + 2, result.Time.Minutes );
                    Assert.Equal( i + 2, result.RaceNumber );
                    Assert.Equal( i + 2, result.WmaScore );
                }
            }
        }

        [Fact]
        public void DeletingInvalidPositionThrowsEx()
        {
            using ( var db = CreateTestDb() )
            {
                Assert.Throws<Exception>( () => db.DeleteResultAtPosition( 0 ) );
                Assert.Throws<Exception>( () => db.DeleteResultAtPosition( 20 ) );
            }
        }

        [Fact]
        public void CanFindDuplicateResults()
        {
            var results = new List<Result>
            {
                new Result {RaceNumber = 12},
                new Result {RaceNumber = 1},
                new Result {RaceNumber = 2},
                new Result {RaceNumber = 3},
                new Result {RaceNumber = 12},
                new Result {RaceNumber = 3},
                new Result {RaceNumber = 4},
                new Result {RaceNumber = 12},
                new Result {RaceNumber = 0},    // Should not count
                new Result {RaceNumber = 0},
                new Result {RaceNumber = 0},
            };

            var duplicates = DbService.FindDuplicateResults( results );
            Assert.Equal( 5, duplicates.Count );

            Assert.Equal( 2, duplicates.Count( r => r.RaceNumber == 3 ) );
            Assert.Equal( 3, duplicates.Count( r => r.RaceNumber == 12 ) );
        }
    }
}
