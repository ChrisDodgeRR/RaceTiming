using System.Collections.Generic;
using System.Linq;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Core.ViewModels;
using Xunit;

namespace RaceTimingTests.Core.Util
{
    public class TeamCalcTest
    {
        [Fact]
        public void CanGetTeamList()
        {
            var finishers = new List<Finisher>
            {
                new Finisher {Team = "team1"},
                new Finisher {Team = "team1"},
                new Finisher {Team = "team1"}
            };
            Assert.Equal(0, TeamCalc.GetTeamNames(finishers).Count);

            finishers.Add( new Finisher {Team = "Team1"} );
            Assert.Equal(1, TeamCalc.GetTeamNames(finishers).Count);
            Assert.Equal("TEAM1", TeamCalc.GetTeamNames(finishers)[0]);

            finishers.AddRange( new[]
            {
                new Finisher {Team = "ddddd"},
                new Finisher {Team = "DDDDD"},
                new Finisher {Team = "ddddd    "},
                new Finisher {Team = "  DDDDD  "},
                new Finisher {Team = "  DDDDD  "},
            } );
            Assert.Equal(2, TeamCalc.GetTeamNames(finishers).Count);
            Assert.Contains("TEAM1", TeamCalc.GetTeamNames(finishers));
            Assert.Contains("DDDDD", TeamCalc.GetTeamNames(finishers));
        }

        [Fact]
        public void CanGetClubTeamList()
        {
            var finishers = new List<Finisher>
            {
                new Finisher {Club = "club1"},
                new Finisher {Club = "club1"},
                new Finisher {Club = "club1"}
            };
            Assert.Equal(0, TeamCalc.GetClubTeamNames(finishers).Count);

            finishers.Add(new Finisher { Club = "club1" });
            Assert.Equal(1, TeamCalc.GetClubTeamNames(finishers).Count);
            Assert.Equal("CLUB1", TeamCalc.GetClubTeamNames(finishers)[0]);

            finishers.AddRange(new[]
            {
                new Finisher {Club = "ddddd"},
                new Finisher {Club = "DDDDD"},
                new Finisher {Club = "ddddd    "},
                new Finisher {Club = "  DDDDD  "},
                new Finisher {Club = "  DDDDD  "},
            });
            Assert.Equal(2, TeamCalc.GetClubTeamNames(finishers).Count);
            Assert.Contains("CLUB1", TeamCalc.GetClubTeamNames(finishers));
            Assert.Contains("DDDDD", TeamCalc.GetClubTeamNames(finishers));
        }

        [Fact]
        public void CanCalcTeamResults()
        {
            var teamResults = TeamCalc.GetTeamResults( CreateTestSet() ).OrderBy( r => r.Score ).ToList();
            Assert.Equal( 3, teamResults.Count );

            Assert.Equal( "CLUB1", teamResults[0].Name );
            Assert.Equal( 17, teamResults[0].Score );

            Assert.Equal("TEAM1", teamResults[1].Name);
            Assert.Equal(19, teamResults[1].Score);

            Assert.Equal( "TEAM2", teamResults[2].Name );
            Assert.Equal( 57, teamResults[2].Score );
        }

        private IList<Finisher> CreateTestSet()
        {
            var finishers = new List<Finisher>
            {
                new Finisher {Position = 1, Team = "team1", Club = "club1"},    // Club should be ignored
                new Finisher {Position = 5, Team = "team1", Club = "club2"},    // Club should be ignored
                new Finisher {Position = 6, Team = "team1"},
                new Finisher {Position = 7, Team = "team1"},
                new Finisher {Position = 10, Team = "team1"},

                new Finisher {Position = 18, Team = "team2"},
                new Finisher {Position = 13, Team = "team2"},
                new Finisher {Position = 15, Team = "team2"},
                new Finisher {Position = 17, Team = "team2"},
                new Finisher {Position = 12, Team = "team2"},

                new Finisher {Position = 20, Club = "club1", Team = "team3"},  // Team should be ignored as not complete
                new Finisher {Position = 2, Club = "club1"},
                new Finisher {Position = 4, Club = "club1"},
                new Finisher {Position = 3, Club = "club1"},
                new Finisher {Position = 8, Club = "club1"},

                new Finisher {Position = 23, Club = "club2"},
                new Finisher {Position = 24, Club = "club2"},
                new Finisher {Position = 25, Club = "club2"},
            };

            return finishers;
        }
    }
}
