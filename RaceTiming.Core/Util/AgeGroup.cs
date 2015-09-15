
using System;
using System.Collections.Generic;
using RedRat.RaceTiming.Data;

namespace RedRat.RaceTiming.Core.Util
{
    public class AgeGroup
    {
        public enum AgeGroupEnum
        {
            M,
            MV40,
            MV50,
            MV60,
            MV70,
            F,
            FV40,
            FV50,
            FV60,
            FV70,
        }

        public static IDictionary<AgeGroupEnum, int> ageGroupLookup = new Dictionary<AgeGroupEnum, int>()
        {
            {AgeGroupEnum.M, 16},
            {AgeGroupEnum.MV40, 40},
            {AgeGroupEnum.MV50, 50},
            {AgeGroupEnum.MV60, 60},
            {AgeGroupEnum.MV70, 70},
            {AgeGroupEnum.F, 16},
            {AgeGroupEnum.FV40, 40},
            {AgeGroupEnum.FV50, 50},
            {AgeGroupEnum.FV60, 60},
            {AgeGroupEnum.FV70, 70},
        };

        public static AgeGroupEnum GetAgeGroup( DateTime raceDate, DateTime dob, GenderEnum gender )
        {
            // Get age on race day
            var ageAtRace = GetAgeOnDate( raceDate, dob );

            if ( gender == GenderEnum.Male )
            {
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.MV70]) return AgeGroupEnum.MV70;
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.MV60]) return AgeGroupEnum.MV60;
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.MV50]) return AgeGroupEnum.MV50;
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.MV40]) return AgeGroupEnum.MV40;
                return AgeGroupEnum.M;
            }
            else
            {
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.FV70]) return AgeGroupEnum.FV70;
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.FV60]) return AgeGroupEnum.FV60;
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.FV50]) return AgeGroupEnum.FV50;
                if (ageAtRace >= ageGroupLookup[AgeGroupEnum.FV40]) return AgeGroupEnum.FV40;
                return AgeGroupEnum.F;
            }
        }

        public static int GetAgeOnDate( DateTime date, DateTime dob )
        {
            var years = date.Year - dob.Year;
            if ( dob.Month == date.Month && date.Day < dob.Day )
            {
                years--;
            }
            else if ( date.Month < dob.Month )
            {
                years--;
            }
            return years;
        }
    }
}
