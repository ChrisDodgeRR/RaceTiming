
using System;
using Volante;

namespace RedRat.RaceTiming.Data.Model
{
    public class Race : Persistent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Distance { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ClockTime { get; set; }
    }
}
