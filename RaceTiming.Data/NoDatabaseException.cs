
using System;

namespace RedRat.RaceTiming.Data
{
    /// <summary>
    /// Thrown if we don't yet have a database.
    /// </summary>
    public class NoDatabaseException : Exception
    {
        public NoDatabaseException( string message ) : base( message )
        { }
    }
}
