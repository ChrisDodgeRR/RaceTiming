

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
        public event EventHandler<bool> ClockRunningHandler;

        private Timer timer;
        private TimeSpan time;
        private readonly TimeSpan oneSecond = TimeSpan.FromSeconds( 1 );
        private bool clockRunning = false;
        private readonly TimeSpan zeroTime = TimeSpan.FromSeconds( 0 );

        public ClockTime()
        {
            time = zeroTime;
            timer = new Timer(1000)
            {
                AutoReset = true
            };
            timer.Elapsed += TimerOnElapsed;
        }

        public void Start()
        {
            timer.Start();
            clockRunning = true;
            NotifyClockRunning();
        }

        public void Stop()
        {
            timer.Stop();
            clockRunning = false;
            NotifyClockRunning();
        }

        public void Reset()
        {
            // Gives a cleaner visual reset to stop the timer and then restart.
            timer.Stop();
            time = zeroTime;
            NotifyClockChange();

            if ( clockRunning )
            {
                timer.Start();
            }
        }

        public TimeSpan CurrentTime
        {
            get { return time; }
            set
            {
                time = value;
                NotifyClockChange();
            }
        }

        public void AddTime( TimeSpan deltaTime, bool isSeconds )
        {
            // If we are changing the seconds, it looks better to stop them and restart.
            // If changing minutes or hours, then we don't want to stop the seconds.
            if ( isSeconds )
            {
                timer.Stop();
            }
            ChangeTime( deltaTime );
            if (clockRunning)
            {
                timer.Start();
            }
        }

        private void TimerOnElapsed( object sender, ElapsedEventArgs elapsedEventArgs )
        {
            ChangeTime( oneSecond );
        }

        private void ChangeTime( TimeSpan deltaTime )
        {
            time = time.Add(deltaTime);
            if ( time < zeroTime )
            {
                time = zeroTime;
            }
            NotifyClockChange();            
        }

        private void NotifyClockChange()
        {
            if ( ClockChangeHandler != null )
            {
                ClockChangeHandler( this, time );
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
