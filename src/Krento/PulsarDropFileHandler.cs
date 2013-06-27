using System;

namespace Krento
{
    /// <summary>
    /// Provides data for <see cref="PulsarDropFileHandler"/> delegate
    /// </summary>
    public sealed class PulsarEventArgs : EventArgs, IDisposable
    {
        private string fileName;
        private bool handled;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsarEventArgs"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public PulsarEventArgs(string fileName)
        {
            this.fileName = fileName;
            this.handled = true;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PulsarEventArgs"/> is reclaimed by garbage collection.
        /// </summary>
        ~PulsarEventArgs()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PulsarEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                fileName = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle an event when the file is dropped on Pulsar
    /// </summary>
    public delegate void PulsarDropFileHandler(PulsarEventArgs e);
}
