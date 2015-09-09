
using System;
using Volante;

namespace RedRat.RaceTiming.Data.Model
{
    public class Result : Persistent
    {
        public int RaceId { get; set; }
        public int RunnerId { get; set; }
        public TimeSpan Time { get; set; }
        public int Position { get; set; }
        public float WmaScore { get; set; }
    }
}
