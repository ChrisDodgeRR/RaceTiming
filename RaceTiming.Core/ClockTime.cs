

using System;
using System.Timers;

namespace RedRat.RaceTiming.Core
{
    /// <summary>
    /// Manages the clock time.
    /// </summary>
    public class ClockTime
    {
        public event EventHandler<TimeSpan> ClockChangeHandler;

        private Timer timer;
        private TimeSpan time;
        private readonly TimeSpan oneSecond = TimeSpan.FromSeconds( 1 );

        public ClockTime()
        {
            time = TimeSpan.FromSeconds( 0 );
            timer = new Timer(1000)
            {
                AutoReset = true
            };
            timer.Elapsed += TimerOnElapsed;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            time = time.Add( oneSecond );
            NotifyClockChange();
        }

        private void NotifyClockChange()
        {
            if ( ClockChangeHandler != null )
            {
                ClockChangeHandler( this, time );
            }
        }
    }
}
