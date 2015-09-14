
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RedRat.RaceTiming.Data;
using RedRat.RaceTiming.Data.Model;

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
            public TimeSpan datetime;
            public override string ToString()
            {
                return string.Format( "{0}", datetime );
            }
        }

        // Notifies listeners of new results
        public event EventHandler NewResult;

        private readonly AppController appController;
        private readonly DbService db;
        private bool processTaskRunning = false;
        private Task processTask = null;

        public ResultsQueue(AppController appController, DbService db)
        {
            this.appController = appController;
            this.db = db;
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
                    var update = false;
                    while ( Count > 0 )
                    {
                        var res = Dequeue();
                        db.AddResultTime( new Result
                        {
                            Position = db.GetNextPosition(),
                            RaceId = appController.CurrentRace.Oid,
                            Time = res.datetime,
                        });
                        Trace.WriteLineIf(AppController.traceSwitch.TraceInfo, "Race result: " + res);
                        update = true;
                    }

                    if (update && NewResult != null)
                    {
                        NewResult( this, null );
                    }

                    // Delay so we don't spin in a loop.
                    Thread.Sleep( 250 );
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
