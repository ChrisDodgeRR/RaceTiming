
using System;
using System.Text;
using Volante;

namespace RedRat.RaceTiming.Data.Model
{
    /// <summary>
    /// We store the race number here and link with the runner on demand.
    /// This is less constrained for data capture and editing.
    /// </summary>
    public class Result : Persistent
    {
        [Flags]
        public enum DubiousResultEnum
        {
            None = 0x00,
            DuplicateNumber = 0x01,     // This race number appears more than once in the results.
            UnknownNumber = 0x02,        // This race number does not have a corresponding entry/runner.
            EstimatedTime = 0x04,       // The result time has been estimated (not captured at finish).
        };

        public int RaceId { get; set; }
        public int RaceNumber { get; set; }
        public TimeSpan Time { get; set; }
        public int Position { get; set; }
        public float WmaScore { get; set; }
        public DubiousResultEnum DubiousResult { get; set; }

        public static bool AddDubiousReason( Result result, DubiousResultEnum reason )
        {
            if ( result.DubiousResult.HasFlag( reason )) return false;

            result.DubiousResult = result.DubiousResult | reason;
            return true;
        }

        public static bool RemoveDubiousReason(Result result, DubiousResultEnum reason)
        {
            if (!result.DubiousResult.HasFlag(reason)) return false;

            result.DubiousResult &= ~reason;
            return true;
        }


        public string GetDubiousResultReason()
        {
            var txt = new StringBuilder();
            Action<string> addText = ( s ) => txt.Append( string.IsNullOrEmpty( txt.ToString() ) ? s : "; " + s );

            if ( DubiousResult.HasFlag( DubiousResultEnum.DuplicateNumber ) )
            {
                addText( "Duplicate runner number" );
            }
            if ( DubiousResult.HasFlag( DubiousResultEnum.UnknownNumber ) )
            {
                addText( "No runner with this number" );
            }
            if ( DubiousResult.HasFlag( DubiousResultEnum.EstimatedTime ) )
            {
                addText( "This result has an estimated time" );
            }
            return txt.ToString();
        }

        /// <summary>
        /// Transfers all state, apart from position.
        /// </summary>
        public static void TransferState( Result to, Result from )
        {
            to.Time = from.Time;
            to.RaceNumber = from.RaceNumber;
            to.WmaScore = from.WmaScore;
            to.DubiousResult = from.DubiousResult;
        }
    }
}
