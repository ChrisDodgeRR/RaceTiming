
using System;
using Volante;

namespace RedRat.RaceTiming.Data.Model
{
    public class Result : Persistent
    {
        public TimeSpan Time { get; set; }

        public int RaceId { get; set; }

        public int RunnerId { get; set; }
    }
}
