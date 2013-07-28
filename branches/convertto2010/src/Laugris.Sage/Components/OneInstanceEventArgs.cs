using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{

    public class OneInstanceEventArgs : EventArgs
    {
        private readonly string fileName;

        public OneInstanceEventArgs(string fileName)
        {
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
        }
    }

}
