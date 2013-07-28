using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    internal class BackgroundHandler : IDisposable
    {
        private IntPtr handle;
        private WndProcDelegate QWindow;
        private IntPtr windowHandle;
        private const int drawTimer = 1;

        public BackgroundHandler()
        {
            AllocateHandle();
        }

        public BackgroundHandler(IntPtr windowHandle)
        {
            AllocateHandle();
            this.windowHandle = windowHandle;
        }

        ~BackgroundHandler()
        {
            Dispose(false);
        }

        private void AllocateHandle()
        {
            QWindow = new WndProcDelegate(WndProc);
            handle = NativeMethods.AllocateHWND(QWindow);
        }

        public IntPtr Handle
        {
            get { return this.handle; }
        }

        public int SetInterval(int frequency)
        {
            StartTimer(drawTimer, frequency);
            return drawTimer;
        }

        public void ResetInterval(int frequency)
        {
            StopTimer(drawTimer);
            StartTimer(drawTimer, frequency);
        }

        public void StopInterval()
        {
            StopTimer(drawTimer);
        }

        public void StartTimer(int timerId, int interval)
        {
            try
            {
                NativeMethods.SetTimer(handle, (IntPtr)timerId, interval, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("StartTimer: " + ex.Message);
            }
        }

        public void StopTimer(int timerId)
        {
            try
            {
                NativeMethods.KillTimer(handle, (IntPtr)timerId);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("StopTimer: " + ex.Message);
            }
        }

        protected virtual void HandleTimer()
        {
            NativeMethods.PostMessage(windowHandle, NativeMethods.CN_DRAW_BACKGROUND, IntPtr.Zero, IntPtr.Zero);
        }

        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case (int)WindowMessage.WM_TIMER:
                    HandleTimer();
                    break;
                default:
                    break;
            }
            return NativeMethods.LayeredWndProc(hWnd, msg, wParam, lParam);
        }


        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            StopInterval();
            NativeMethods.SendMessage(handle, (int)WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            NativeMethods.DeallocateHWND(handle);
            GC.KeepAlive(QWindow);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
