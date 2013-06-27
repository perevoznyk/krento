//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Laugris.Sage;

namespace Krento
{
    internal sealed class SplashScreen : IDisposable
    {
        private SplashWindow window;


        public SplashScreen()
        {
            window = new SplashWindow();
            window.TopMostWindow = true;
            window.CanDrag = false;
            window.FadeFinished += new EventHandler<FadeEventArgs>(window_FadeFinished);
            window.Cursor = Cursors.AppStarting;
        }


        void window_FadeFinished(object sender, FadeEventArgs e)
        {
            window.Hide();
                NativeMethods.PostMessage(InteropHelper.MainWindow, NativeMethods.CM_SPLASHCLOSE, IntPtr.Zero, IntPtr.Zero); 
        }

        ~SplashScreen()
        {
            Dispose(false);
        }

        public void Show()
        {
            window.Left = (int)(PrimaryScreen.Center.X - (window.Width / 2.0));
            window.Top = (int)(PrimaryScreen.Center.Y - (window.Height / 2.0));
            window.Show(true);
        }

        public void Hide()
        {
            window.Hide();
        }

        public void Close(int fadeDuration)
        {
            window.Shutdown(fadeDuration);
        }

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (window != null)
                {
                    window.FadeFinished -= window_FadeFinished;
                    window.Dispose();
                    window = null;
                }
            }
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
