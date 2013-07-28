using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Laugris.Sage
{
        internal sealed class ThreadWindows : IDisposable
        {
            private IntPtr activeHwnd;
            private IntPtr focusedHwnd;
            private int windowCount;
            private IntPtr[] windows = new IntPtr[0x10];

            internal ThreadWindows()
            {
                NativeMethods.EnumThreadWindows(NativeMethods.GetCurrentThreadId(), new EnumThreadWindowsCallback(this.Callback), IntPtr.Zero);
            }

            ~ThreadWindows()
            {
                Dispose(false);
            }

            private bool Callback(IntPtr hWnd, IntPtr lparam)
            {
                if (NativeMethods.IsWindowVisible(hWnd) && NativeMethods.IsWindowEnabled(hWnd))
                {
                    if (this.windowCount == this.windows.Length)
                    {
                        IntPtr[] destinationArray = new IntPtr[this.windowCount * 2];
                        Array.Copy(this.windows, 0, destinationArray, 0, this.windowCount);
                        this.windows = destinationArray;
                    }
                    this.windows[this.windowCount++] = hWnd;
                }
                return true;
            }


            internal void Enable(bool state)
            {
                if (!state)
                {
                    this.activeHwnd = NativeMethods.GetActiveWindow();
                    this.focusedHwnd = NativeMethods.GetFocus();
                }
                for (int i = 0; i < this.windowCount; i++)
                {
                    IntPtr handle = this.windows[i];
                    if (NativeMethods.IsWindow(handle))
                    {
                        NativeMethods.EnableWindow(handle, state);
                    }
                }
                if (state)
                {
                    if ((this.activeHwnd != IntPtr.Zero) && NativeMethods.IsWindow(this.activeHwnd))
                    {
                        NativeMethods.SetActiveWindow(this.activeHwnd);
                    }
                    if ((this.focusedHwnd != IntPtr.Zero) && NativeMethods.IsWindow(this.focusedHwnd))
                    {
                        NativeMethods.SetFocus(this.focusedHwnd);
                    }
                }
            }


            private void Dispose(bool disposing)
            {
                if (disposing)
                    this.windows = null;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }
}
