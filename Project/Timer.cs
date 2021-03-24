// Beginning C# Game Programming
// (C)2004 Ron Penton
// Advanced Framework Timer

namespace AdvancedFramework
{

    // --------------------------------------------------------------------
    // Independent timer class
    // --------------------------------------------------------------------
    public class Timer
	{

        // --------------------------------------------------------------------
        // variables
        // --------------------------------------------------------------------
        float starttime;    // absolute time when timer was last reset
        float pausetime;    // absolute time when timer was paused
        float elapsedtime;  // timer time when last Elapsed() call was made
        bool paused;        // is timer paused?


        // --------------------------------------------------------------------
        // Returns absolute system time; static; requires no timer objects
        // --------------------------------------------------------------------
        public static float Now()
        {
            return DXUtil.Timer( DirectXTimer.GetAbsoluteTime );
        }

        // --------------------------------------------------------------------
        // Constructor
        // --------------------------------------------------------------------
        public Timer()
		{
            // reset the start time to current absolute time
			this.starttime = Now();
            paused = false;

            // reset elapsed time to 0
            this.elapsedtime = 0.0f;
        }


        // --------------------------------------------------------------------
        // Reset the timer to a specific time value
        // --------------------------------------------------------------------
        public void Reset( float p_time )
        {
            this.starttime = Now() - p_time;
        }


        // --------------------------------------------------------------------
        // Reset the timer to 0
        // --------------------------------------------------------------------
        public void Reset()
        {
            this.Reset( 0.0f );
        }


        // --------------------------------------------------------------------
        // get current timer time
        // --------------------------------------------------------------------
        public float Time()
        {
            // if paused, return time difference from when timer was paused
            if( this.paused )
                return this.pausetime - this.starttime;

            // if not, then return time normally
            else
                return Now() - this.starttime;
        }


        // --------------------------------------------------------------------
        // get amount of time elapsed since last call to Elapsed()
        // --------------------------------------------------------------------
        public float Elapsed()
        {
            float now = Time();
            float elapsed = now - this.elapsedtime;
            this.elapsedtime = now;
            return elapsed;
        }


        // --------------------------------------------------------------------
        // Pause the timer
        // --------------------------------------------------------------------
        public void Pause()
        {
            // make sure timer isn't already paused
            if( !this.paused )
            {
                this.paused = true;
                this.pausetime = Now();
            }
        }


        // --------------------------------------------------------------------
        // Unpause the timer
        // --------------------------------------------------------------------
        public void Unpause()
        {
            // make sure timer isn't already unpaused
            if( this.paused )
            {
                this.paused = false;
                this.starttime += Now() - this.pausetime;
            }
        }
    }
}
