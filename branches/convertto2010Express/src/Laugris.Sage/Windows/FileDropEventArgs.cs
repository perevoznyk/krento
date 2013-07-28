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

namespace Laugris.Sage
{
    public class FileDropEventArgs : EventArgs
    {
        private readonly string fileName;

        public FileDropEventArgs(string fileName)
        {
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
        }
    }
}
