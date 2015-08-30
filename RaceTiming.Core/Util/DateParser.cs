
using System;
using System.Text.RegularExpressions;

namespace RedRat.RaceTiming.Core.Util
{
    public class DateParser
    {
        /// <summary>
        /// Parse the Runners World date format - 25/1/1965 or 25/12/2015.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ParseRwDate( string date )
        {
            var match = Regex.Match(date, @"(?<day>[0-9]{1,2})/(?<month>[0-9]{1,2})/(?<year>[0-9]{2,4})");

            var day = match.Groups["day"];
            var month = match.Groups["month"];
            var year = match.Groups["year"];

            return DateTime.Parse( string.Format( "{0}-{1}-{2}", day, month, year ) );
        }
    }
}
