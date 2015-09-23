using System.Collections.Generic;
using RedRat.RaceTiming.Core;
using RedRat.RaceTiming.Core.ViewModels;
using Xunit;

namespace RaceTimingTests.Core
{
    public class AppControllerTests
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
            Assert.Equal( 0, AppController.GetTeamNames( finishers ).Count );

            finishers.Add(new Finisher { Team = "Team1" });
            Assert.Equal(1, AppController.GetTeamNames(finishers).Count);
            Assert.Equal("TEAM1", AppController.GetTeamNames(finishers)[0]);

            finishers.AddRange( new[]
            {
                new Finisher{Club = "club1" }, 
                new Finisher{Club = "club1" }, 
                new Finisher{Club = "club1" }, 
                new Finisher{Club = "club1" }, 
            });
            Assert.Equal(2, AppController.GetTeamNames(finishers).Count);
            Assert.Contains( "TEAM1", AppController.GetTeamNames( finishers ) );
            Assert.Contains( "CLUB1", AppController.GetTeamNames( finishers ) );
        }
    }
}
