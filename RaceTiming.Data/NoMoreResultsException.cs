using System;

namespace RedRat.RaceTiming.Data
{
    public class NoMoreResultsException : Exception
    {
        public NoMoreResultsException() { }

        public NoMoreResultsException(string message) : base(message) { }
    }
}
