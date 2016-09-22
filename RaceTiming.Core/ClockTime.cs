

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RedRat.RaceTiming.Core
{
    /// <summary>
    /// Manages the clock time.
    /// </summary>
    public class ClockTime
    {
        public event EventHandler<TimeSpan> ClockChangeHandler;
        public event EventHandler<bool> ClockRunningHandler;

        private readonly Timer timer;            // Used to create events to update time listeners.
        private readonly Stopwatch stopwatch;
        private TimeSpan stopwatchOffset;
        private readonly TimeSpan zeroTime = TimeSpan.FromSeconds( 0 );
        private bool clockRunning = false;

        public ClockTime()
        {
            stopwatch = new Stopwatch();
            stopwatchOffset = zeroTime;
            timer = new Timer(1000)
            {
                AutoReset = true
            };
            timer.Elapsed += TimerOnElapsed;
        }

        public void Start()
        {
            stopwatch.Start();
            timer.Start();
            clockRunning = true;
            NotifyClockRunning();
        }

        public void Stop()
        {
            stopwatch.Stop();
            timer.Stop();
            clockRunning = false;
            NotifyClockRunning();
        }

        public void Reset()
        {
            stopwatchOffset = zeroTime;
            stopwatch.Reset();
            NotifyClockChange();

            if ( clockRunning )
            {
                stopwatch.Start();
            }
        }

        public TimeSpan CurrentTime
        {
            get { return stopwatch.Elapsed + stopwatchOffset; }
            set
            {
                stopwatchOffset = value;
                stopwatch.Reset();
                NotifyClockChange();
            }
        }

        public void AddTime( TimeSpan deltaTime, bool isSeconds )
        {
            ChangeTime( deltaTime );
        }

        private void TimerOnElapsed( object sender, ElapsedEventArgs elapsedEventArgs )
        {
            NotifyClockChange();
        }

        private void ChangeTime( TimeSpan deltaTime )
        {
            var stopwatchOffsetSave = stopwatchOffset;
            stopwatchOffset = stopwatchOffset.Add(deltaTime);
            // Don't allow it to be set to less than 0.
            if ( stopwatchOffset < TimeSpan.Zero )
            {
                stopwatchOffset = stopwatchOffsetSave;
            }
            NotifyClockChange();
        }

        private void NotifyClockChange()
        {
            if ( ClockChangeHandler != null )
            {
                ClockChangeHandler( this, CurrentTime );
            }
        }

        private void NotifyClockRunning()
        {
            if (ClockRunningHandler != null)
            {
                ClockRunningHandler(this, clockRunning);
            }
        }
    }
}
