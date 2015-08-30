
using System.Diagnostics;
using System.IO;

namespace RedRat.RaceTiming.Core.Logging
{
    /// <summary>
    /// Handles logging to file etc.
    /// </summary>
    public class LogController : TraceListener
    {
        // Note: This probably isn't very efficient as it opens the log file for each log entry.
        //       It is somewhat safer but could be improved if performance is an issue.

        private string logFilename;
        private readonly object writeLock = new object();

        public void Open( string logFilename )
        {
            this.logFilename = logFilename;
            Trace.WriteLineIf( AppController.traceSwitch.TraceInfo, "\nLOGGING STARTED..." );
        }

        private void Log( string message )
        {
            lock ( writeLock )
            {
                if ( logFilename != null )
                {
                    File.AppendAllText( logFilename, message );
                }
            }
        }

        public override void Write(string message)
        {
            Log( message );
        }

        public override void WriteLine(string message)
        {
            Log( message + "\n" );
        }
    }
}
