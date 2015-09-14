
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

        public IDictionary<AgeGroupEnum, int> ageGroupLookup = new Dictionary<AgeGroupEnum, int>()
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
            return AgeGroupEnum.M;
        }
    }
}
