using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    public class LiveBackground : IDisposable
    {
        private Window window;
        private int handler;
        private BackgroundHandler helperWindow;

        public LiveBackground(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            this.window = window;
            helperWindow = new BackgroundHandler(window.Handle);
        }

        ~LiveBackground()
        {
            Dispose(false);
        }

        internal protected virtual void Draw(Graphics canvas, bool reschedule)
        {
        }

        public virtual void Run()
        {
            window.RedrawBackground();
        }

        public virtual void Reschedule(int delay)
        {
            if (handler == 0)
                handler = helperWindow.SetInterval(delay);
            else
                helperWindow.ResetInterval(delay);
        }

        public virtual void Stop()
        {
            if (handler > 0)
            {
                helperWindow.StopInterval();
            }
        }

        public int Handler
        {
            get { return handler; }
        }

        public virtual void OnDestroy()
        {
            Stop();
        }

        public virtual void OnVisibilityChanged(bool value)
        {
        }

        public virtual void OnCanvasCreate()
        {
        }

        public virtual void OnCanvasDestroyed()
        {
            Stop();
        }

        public virtual void OnOffsetChanged(int offsetX, int offsetY)
        {
        }

        public virtual void OnSizeChanged()
        {
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            helperWindow.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
