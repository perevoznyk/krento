using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public class KrentoSkinInfo
    {
        public string FileName { get; set; }
        public string Caption { get; set; }

        public KrentoSkinInfo(string fileName, string caption)
        {
            this.FileName = fileName;
            this.Caption = caption;
        }
    }
}
