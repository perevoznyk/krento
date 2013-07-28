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
using System.IO;
using Laugris.Sage;

namespace Krento.Toys
{
    /// <summary>
    /// Krento About Box
    /// </summary>
    internal sealed class ToyAboutBox : IDisposable
    {
        private AboutWindow window;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToyAboutBox"/> class.
        /// </summary>
        public ToyAboutBox(string configFileName)
        {
            MemIniFile iniFile = new MemIniFile(configFileName);
            iniFile.Load();

            window = new AboutWindow();
            window.TopMostWindow = true;
            window.KeyDown += new System.Windows.Forms.KeyEventHandler(window_KeyDown);
            window.MouseClick += new System.Windows.Forms.MouseEventHandler(window_MouseClick);
            window.BigIcon = Path.Combine(Path.GetDirectoryName(configFileName) ,  iniFile.ReadString("Toy", "Icon"));
            window.Copyright = iniFile.ReadString("Toy", "Copyright");
            window.Author = "Author: " + iniFile.ReadString("Toy", "Author");
            window.Version = "Version: " + iniFile.ReadString("Toy", "Version");
            window.Description = iniFile.ReadString("Toy", "Description");
            window.AboutText = iniFile.ReadString("Toy", "AboutText");
            iniFile.Dispose();
        }

        void window_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            window.Hide();
        }

        void window_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            window.Hide();
        }

        ~ToyAboutBox()
        {
            Dispose(false);
        }

        public void Show()
        {

            window.Update(true);
            window.Left = (int)(PrimaryScreen.Center.X - (window.Width / 2.0));
            window.Top = (int)(PrimaryScreen.Center.Y - (window.Height / 2.0));
            window.ShowDialog();
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
