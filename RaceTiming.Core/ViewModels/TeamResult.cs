
using System.Collections.Generic;

namespace RedRat.RaceTiming.Core.ViewModels
{
    public class TeamResult
    {
        public string Name { get; set; }
        public IList<Finisher> Members { get; set; }
        public int Score { get; set; }
    }
}
