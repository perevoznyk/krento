using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Laugris.Sage
{

    public class ThreadMethodEntry
    {
        public string ErrorMessage { get; set; }
        public MethodInvoker Entry { get; set; }

        public ThreadMethodEntry(MethodInvoker entry, string message)
        {
            ErrorMessage = message;
            Entry = entry;
        }

        public ThreadMethodEntry(MethodInvoker entry)
        {
            Entry = entry;
        }
    }

    public class Invocator : IDisposable
    {
        private IntPtr handle;
        Queue<ThreadMethodEntry> queue;
        private WndProcDelegate QWindow;

        public Invocator()
        {
            queue = new Queue<ThreadMethodEntry>();
            QWindow = new WndProcDelegate(WndProc);
            handle = NativeMethods.AllocateHWND(QWindow);
        }

        ~Invocator()
        {
            Dispose(false);
        }

        protected void ExecuteRequest()
        {
            ThreadMethodEntry method = null;

            lock (this.queue)
            {
                if (queue.Count > 0)
                {
                    method = queue.Dequeue();
                }
            }

            if (method != null)
            {
                try
                {
                    if (method.Entry != null)
                    method.Entry();
                }
                catch (Exception ex)
                {
                    InvocationException ie = new InvocationException(method.ErrorMessage, ex);
                    Application.OnThreadException(ie);
                }
            }

        }

        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case NativeMethods.CN_BEGININVOKE:
                case NativeMethods.CN_INVOKE:
                    ExecuteRequest();
                    return IntPtr.Zero;
                case (int)WindowMessage.WM_CLOSE:
                    queue.Clear();
                    break;
                default:
                    break;
            }

            return NativeMethods.LayeredWndProc(hWnd, msg, wParam, lParam);
        }

        public void BeginInvoke(MethodInvoker method)
        {
            ThreadMethodEntry entry = new ThreadMethodEntry(method);
            queue.Enqueue(entry);
            NativeMethods.PostMessage(handle, NativeMethods.CN_BEGININVOKE, IntPtr.Zero, IntPtr.Zero);
        }

        public void BeginInvoke(MethodInvoker method, string errorMessage)
        {
            ThreadMethodEntry entry = new ThreadMethodEntry(method, errorMessage);
            queue.Enqueue(entry);
            NativeMethods.PostMessage(handle, NativeMethods.CN_BEGININVOKE, IntPtr.Zero, IntPtr.Zero);
        }

        public void Invoke(MethodInvoker method)
        {
            ThreadMethodEntry entry = new ThreadMethodEntry(method);
            queue.Enqueue(entry);
            NativeMethods.SendMessage(new HandleRef(null, handle), NativeMethods.CN_INVOKE, IntPtr.Zero, IntPtr.Zero);
        }

        public void Invoke(MethodInvoker method, string errorMessage)
        {
            ThreadMethodEntry entry = new ThreadMethodEntry(method, errorMessage);
            queue.Enqueue(entry);
            NativeMethods.SendMessage(new HandleRef(null, handle), NativeMethods.CN_INVOKE, IntPtr.Zero, IntPtr.Zero);
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (disposing)
                queue.Clear();
            else
                NativeMethods.SendMessage(new HandleRef(null, handle), (int)WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
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
