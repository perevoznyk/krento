//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

/*
 * Important history:
 * 
 * 26.02.2009 - Added new property TopMostWindow
 * 17.08.2009 - Fixed Cursor property
 * 29.09.2009 - Added Modal dialogs support
 * 
 */

using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;

namespace Laugris.Sage
{
    /// <summary>
    /// Lightweight wrapper for native Windows window
    /// </summary>
    public class LightWindow : IDisposable, IWin32Window
    {
        #region Private fields
        private int tag;
        private Cursor cursor = Cursors.Default;
        private bool mostBottomWindow;
        private IntPtr handle;
        #endregion

        WndProcDelegate InternalWndProcDelegate;

        internal static ThreadWindows DisableTaskWindows()
        {
            ThreadWindows threadWindows = new ThreadWindows();
            threadWindows.Enable(false);
            return threadWindows;
        }

        internal static void EnableTaskWindows(ThreadWindows threadWindows)
        {
            threadWindows.Enable(true);
            threadWindows.Dispose();
            threadWindows = null;
        }

        internal bool LocalModalMessageLoop(LightWindow window)
        {
            try
            {
                MSG msg = new MSG();
                bool flag = false;
                bool flag2 = true;
                while (flag2)
                {
                    if (!NativeMethods.PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0))
                    {
                        goto Label_Wait;
                    }
                    if ((msg.hwnd != IntPtr.Zero) && NativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd)))
                    {
                        flag = true;
                        if (NativeMethods.GetMessageW(ref msg, IntPtr.Zero, 0, 0))
                        {
                            goto Label_Translate;
                        }
                        continue;
                    }
                    flag = false;
                    if (!NativeMethods.GetMessageA(ref msg, IntPtr.Zero, 0, 0))
                    {
                        continue;
                    }
                Label_Translate:
                    NativeMethods.TranslateMessage(ref msg);
                    if (flag)
                    {
                        NativeMethods.DispatchMessageW(ref msg);
                    }
                    else
                    {
                        NativeMethods.DispatchMessageA(ref msg);
                    }
                    if (window != null)
                    {
                        flag2 = NativeMethods.IsWindowVisible(window.handle);
                    }

                    continue;
                Label_Wait:
                    if (window == null)
                    {
                        break;
                    }
                    if (!NativeMethods.PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0))
                    {
                        NativeMethods.WaitMessage();
                    }
                }
                return flag2;
            }
            catch
            {
                return false;
            }
        }

        protected virtual void DestroyWindow()
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    if (!NativeMethods.DeallocateHWND(handle))
                    {
                        this.UnregisterWindowProc();
                        NativeMethods.PostMessage(this.Handle, WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                }

            }

            finally
            {
                handle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window has a handle associated with it. 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance handle is created; otherwise, <c>false</c>.
        /// </value>
        public bool IsHandleCreated
        {
            get
            {
                return (this.Handle != IntPtr.Zero);
            }
        }

        ~LightWindow()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Code to dispose the unmanaged resources 
            // held by the class
            if (disposing)
            {
                DestroyWindow();
                this.InternalWndProcDelegate = null;
            }
            else
            {
                ForceExit();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void SetWindowText(string value)
        {
            if (string.IsNullOrEmpty(value))
                value = "";

            if (this.IsHandleCreated)
            {
                NativeMethods.SetWindowText(Handle, value);
            }
        }

        private string GetWindowText()
        {
            int windowTextLength = NativeMethods.GetWindowTextLength(Handle);
            if (SystemInformation.DbcsEnabled)
            {
                windowTextLength = (windowTextLength * 2) + 1;
            }
            StringBuilder lpString = new StringBuilder(windowTextLength + 1);
            int len = NativeMethods.GetWindowText(Handle, lpString, lpString.Capacity);
            if (len > 0)
                return lpString.ToString();
            else
                return string.Empty;
        }

        protected IntPtr SendMessageToSelf(int msg, IntPtr wparam, IntPtr lparam)
        {
            return NativeMethods.SendMessage(Handle, msg, wparam, lparam);
        }

        protected bool PostMessageToSelf(int msg, IntPtr wparam, IntPtr lparam)
        {
            return NativeMethods.PostMessage(this.Handle, msg, wparam, lparam);
        }

        protected void SetCursorInternal()
        {
            IntPtr cursorHandle = (this.cursor == null) ? IntPtr.Zero : this.cursor.Handle;
            NativeMethods.SetCursor(cursorHandle);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightWindow"/> class.
        /// </summary>
        public LightWindow()
        {
        }

        private void ReleaseHandle()
        {
            if (this.handle != IntPtr.Zero)
            {
                lock (this)
                {
                    if (this.handle != IntPtr.Zero)
                    {
                        this.handle = IntPtr.Zero;
                    }
                }
            }
        }

        internal void ForceExit()
        {
            IntPtr winHandle;
            lock (this)
            {
                winHandle = this.handle;
            }

            if (this.handle != IntPtr.Zero)
            {
                this.ReleaseHandle();
            }

            if (winHandle != IntPtr.Zero)
            {
                NativeMethods.PostMessage(winHandle, WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }

        }

        protected void CreateHandle()
        {
                handle = NativeMethods.AllocateLaugrisWindow();
                if (handle != IntPtr.Zero)
                {
                    this.InternalWndProcDelegate = new WndProcDelegate(WndProc);
                    NativeMethods.SetWindowLongPointer(this.Handle, WindowLong.GWL_WNDPROC, InternalWndProcDelegate);
                }
        }

        protected void UnregisterWindowProc()
        {
            NativeMethods.RestoreWindowSubclass(this.Handle);
        }

        protected virtual void WndProc(ref Message m)
        {
            DefWndProc(ref m);
        }

        internal virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            Message m = Message.Create(hWnd, msg, wParam, lParam);
            WndProc(ref m);
            return m.Result;
        }

        private bool GetTopMostWindow()
        {
            if (this.handle == IntPtr.Zero)
                return false;

            return NativeMethods.IsTopMostWindow(handle);
        }

        private void SetTopMostWindow(bool value)
        {
            if (this.handle == IntPtr.Zero)
                return;

            NativeMethods.SetTopMostWindow(handle, value);
            mostBottomWindow = !value;
        }

        private void SetBottomMostWindow(bool value)
        {
            if (this.handle == IntPtr.Zero)
                return;

            NativeMethods.SetBottomMostWindow(handle, value);
            mostBottomWindow = value;
        }

        protected static void DefWndProc(ref Message m)
        {
            m.Result = NativeMethods.LayeredWndProc(m.HWnd, m.Msg, m.WParam, m.LParam);
        }

        #region Public methods and properties

        public IntPtr Handle
        {
            get { return handle; }
        }

        /// <summary>
        /// Put this window to the bottom of Z order
        /// </summary>
        /// <value><c>true</c> if [bottom most window]; otherwise, <c>false</c>.</value>
        public bool BottomMostWindow
        {
            get { return mostBottomWindow; }
            set { SetBottomMostWindow(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [top most window].
        /// </summary>
        /// <value><c>true</c> if [top most window]; otherwise, <c>false</c>.</value>
        public bool TopMostWindow
        {
            get { return GetTopMostWindow(); }
            set { SetTopMostWindow(value); }
        }

        public void InsertAfter(LightWindow window)
        {
            if (window == null)
                return;

            if (this.Handle == IntPtr.Zero)
                return;

            if (window.Handle == IntPtr.Zero)
                return;

            NativeMethods.WindowInsertAfter(handle, window.Handle);
        }

        public string Text
        {
            get { return GetWindowText(); }
            set { SetWindowText(value); }
        }

        public virtual Cursor Cursor
        {
            get
            {
                if (cursor != null)
                    return cursor;
                else
                    return Cursors.Default;
            }
            set
            {
                this.cursor = value;
                if (this.IsHandleCreated)
                {
                    if (NativeMethods.CursorInsideWindow(this.Handle))
                        SetCursorInternal();
                }
            }
        }

        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        #endregion
    }
}
