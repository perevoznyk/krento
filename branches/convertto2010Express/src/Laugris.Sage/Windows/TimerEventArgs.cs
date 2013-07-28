using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public class TimerEventArgs : EventArgs, IDisposable
    {
        private readonly int timerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerEventArgs"/> class.
        /// </summary>
        /// <param name="timerId">The timer id.</param>
        public TimerEventArgs(int timerId)
        {
            this.timerId = timerId;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TimerEventArgs"/> is reclaimed by garbage collection.
        /// </summary>
        ~TimerEventArgs()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the timer id.
        /// </summary>
        /// <value>The timer id.</value>
        public int TimerId
        {
            get { return timerId; }
        }

        #region IDisposable Members

        internal void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
