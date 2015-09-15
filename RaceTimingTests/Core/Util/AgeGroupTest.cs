
using System;
using RedRat.RaceTiming.Core.Util;
using RedRat.RaceTiming.Data;
using Xunit;

namespace RaceTimingTests.Core.Util
{
    public class AgeGroupTest
    {
        private DateTime raceDate = DateTime.Parse( "27/09/2015" );

        [Fact]
        public void CanCalculateAge()
        {
            Assert.Equal( 50, AgeGroup.GetAgeOnDate( raceDate, DateTime.Parse( "25/01/1965" ) ) );
            Assert.Equal( 20, AgeGroup.GetAgeOnDate( raceDate, DateTime.Parse( "27/09/1995" ) ) );
            Assert.Equal( 19, AgeGroup.GetAgeOnDate( raceDate, DateTime.Parse( "28/09/1995" ) ) );
            Assert.Equal( 20, AgeGroup.GetAgeOnDate( raceDate, DateTime.Parse( "26/09/1995" ) ) );
            Assert.Equal( 29, AgeGroup.GetAgeOnDate( raceDate, DateTime.Parse( "12/12/1985" ) ) );
            Assert.Equal( 29, AgeGroup.GetAgeOnDate( raceDate, DateTime.Parse( "12/02/1986" ) ) );
        }

        [Fact]
        public void CanFindAgeGroup()
        {
            Assert.Equal(AgeGroup.AgeGroupEnum.M, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("12/02/1976"), GenderEnum.Male));
            Assert.Equal(AgeGroup.AgeGroupEnum.MV40, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("12/02/1966"), GenderEnum.Male));
            Assert.Equal(AgeGroup.AgeGroupEnum.MV50, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("25/01/1965"), GenderEnum.Male));
            Assert.Equal(AgeGroup.AgeGroupEnum.MV60, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("25/01/1953"), GenderEnum.Male));
            Assert.Equal(AgeGroup.AgeGroupEnum.MV70, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("25/01/1903"), GenderEnum.Male));

            Assert.Equal(AgeGroup.AgeGroupEnum.F, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("12/02/1976"), GenderEnum.Female));
            Assert.Equal(AgeGroup.AgeGroupEnum.FV40, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("12/02/1966"), GenderEnum.Female));
            Assert.Equal(AgeGroup.AgeGroupEnum.FV50, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("25/01/1965"), GenderEnum.Female));
            Assert.Equal(AgeGroup.AgeGroupEnum.FV60, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("25/01/1953"), GenderEnum.Female));
            Assert.Equal(AgeGroup.AgeGroupEnum.FV70, AgeGroup.GetAgeGroup(raceDate, DateTime.Parse("25/01/1903"), GenderEnum.Female));
        
        }
    }
}
