
using System;
using RedRat.RaceTiming.Core.Util;
using Xunit;

namespace RaceTimingTests.Core.Util
{
    public class DateParserTests
    {
        [Fact]
        public void CanParseRwDoB()
        {
            Assert.Equal( DateTime.Parse( "25-Jan-65" ), DateParser.ParseRwDate( @"25/1/1965" ) );
            Assert.Equal( DateTime.Parse( "25-Jan-65" ), DateParser.ParseRwDate( @"25/01/1965" ) );
            Assert.Equal( DateTime.Parse( "25-Jan-65" ), DateParser.ParseRwDate( @"25/1/65" ) );
            Assert.Equal( DateTime.Parse( "25-Jan-65" ), DateParser.ParseRwDate( @"25/01/65" ) );
            Assert.Equal( DateTime.Parse( "5-Jan-65" ), DateParser.ParseRwDate( @"05/01/65" ) );
        }
    }
}
