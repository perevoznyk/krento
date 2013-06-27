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
using Laugris.Sage;
using Krento.RollingStones;

namespace Krento
{
    /// <summary>
    /// Krento About Box
    /// </summary>
    internal sealed class AboutBox : IDisposable
    {
        private AboutWindow window;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox"/> class.
        /// </summary>
        public AboutBox()
        {
            window = new AboutWindow();
            window.TopMostWindow = true;
            window.KeyDown += new System.Windows.Forms.KeyEventHandler(window_KeyDown);
            window.MouseClick += new System.Windows.Forms.MouseEventHandler(window_MouseClick);
        }
        public AboutBox(RollingStoneBase stone)
        {
            window = new AboutWindow(stone);
            window.TopMostWindow = true;
            window.KeyDown += new System.Windows.Forms.KeyEventHandler(window_KeyDown);
            window.MouseClick += new System.Windows.Forms.MouseEventHandler(window_MouseClick);
        }

        private void window_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            window.Hide();
        }

        private void window_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            window.Hide();
        }

        ~AboutBox()
        {
            Dispose(false);
        }

        public void Show()
        {

            window.Left = (int)(PrimaryScreen.Center.X - (window.Width / 2.0));
            window.Top = (int)(PrimaryScreen.Center.Y - (window.Height / 2.0));
            window.ShowDialog();
        }

        public void Show(RollingStoneBase stone)
        {
            if (stone != null)
            {
            }
            Show();
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (window != null)
                {
                    window.KeyDown -= window_KeyDown;
                    window.MouseClick -= window_MouseClick;
                    window.Dispose();
                    window = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
