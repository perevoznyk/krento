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
using System.Threading;

namespace Laugris.Sage
{
    /// <summary>
    /// Called FileExcutor Execute method asynchronously from the different thread
    /// </summary>
    public sealed class AsyncShellExecute : IDisposable
    {
        private readonly string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncShellExecute"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public AsyncShellExecute(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="AsyncShellExecute"/> is reclaimed by garbage collection.
        /// </summary>
        ~AsyncShellExecute()
        {
            Dispose(false);
        }

        private void ExecuteAsync()
        {
            FileExecutor.Execute(fileName);
        }

        public void Run()
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Thread t = new Thread(new ThreadStart(ExecuteAsync));
                t.IsBackground = true;
                t.Start();
                NativeMethods.Sleep(0);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
        }

        #endregion
    }
}
