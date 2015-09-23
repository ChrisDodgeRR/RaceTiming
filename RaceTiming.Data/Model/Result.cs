
using System;
using Volante;

namespace RedRat.RaceTiming.Data.Model
{
    /// <summary>
    /// We store the race number here and link with the runner on demand.
    /// This is less constrained for data capture and editing.
    /// </summary>
    public class Result : Persistent
    {
        public int RaceId { get; set; }
        public int RaceNumber { get; set; }
        public TimeSpan Time { get; set; }
        public int Position { get; set; }
        public float WmaScore { get; set; }
        public bool DubiousResult { get; set; }
        public string Reason { get; set; }

        public void AppendReason( string reason )
        {
            if ( string.IsNullOrWhiteSpace( Reason ) )
            {
                Reason = reason;
            }
            else
            {
                Reason = Reason + "; " + reason;
            }
        }

        public void ClearDubiousFlag()
        {
            DubiousResult = false;
            Reason = "";
        }
    }
}
