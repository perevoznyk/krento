using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public class EmptyEventArgs : EventArgs, IDisposable
    {

        public EmptyEventArgs() : base() { }

        ~EmptyEventArgs()
        {
            Dispose(false);
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            //nothing to do
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
