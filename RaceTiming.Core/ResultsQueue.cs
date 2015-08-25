
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RedRat.RaceTiming.Core
{
    /// <summary>
    /// In case the computer is busy (slow filesystem etc), we want to separate capturing race times 
    /// from processing them. So results are added to this queue for later insertion into the DB.
    /// </summary>
    public class ResultsQueue : Queue<ResultsQueue.ResultSlot>
    {
        public struct ResultSlot
        {
            public bool female;
            public TimeSpan datetime;
            public override string ToString()
            {
                return string.Format( "{0} - {1}", datetime, female ? "F" : "M" );
            }
        }

        private readonly AppController appController;
        private bool processTaskRunning = false;
        private Task processTask = null;

        public ResultsQueue( AppController appController )
        {
            this.appController = appController;
            appController.ClockTime.ClockRunningHandler += ClockRunningHandler;
        }

        public void ClockRunningHandler(object sender, bool clockRunning)
        {
            if (clockRunning)
            {
                if ( processTask == null )
                {
                    processTask = Task.Run( () => ProcessResultQueue() );
                }
            }
            else
            {
                processTaskRunning = false;
                processTask = null;
            }
        }

        private void ProcessResultQueue()
        {
            Trace.WriteLineIf(AppController.traceSwitch.TraceInfo, "Result processing queue started...");
            processTaskRunning = true;
            while (processTaskRunning)
            {
                try
                {

                    if ( this.Count > 0 )
                    {
                        var res = this.Dequeue();
                        Trace.WriteLineIf(AppController.traceSwitch.TraceInfo, "Race result: " + res);
                    }
                    // Delay so we don't spin in a loop.
                    Thread.Sleep( 500 );
                }
                catch ( Exception ex )
                {
                    Trace.WriteLineIf(AppController.traceSwitch.TraceError, "Error processing result queue: " + ex.Message);
                }
            }
            Trace.WriteLineIf(AppController.traceSwitch.TraceInfo, "Result processing queue terminating...");
        }
    }
}
