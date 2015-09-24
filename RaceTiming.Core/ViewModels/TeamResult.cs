
using System.Collections.Generic;
using System.Linq;

namespace RedRat.RaceTiming.Core.ViewModels
{
    public class TeamResult
    {
        public string Name { get; set; }
        public IList<Finisher> Members { get; set; }
        public int Score { get; set; }

        public void CalcScore()
        {
            if ( Members == null ) return;
            Score = Members.Sum( m => m.Position );
        }
    }
}
