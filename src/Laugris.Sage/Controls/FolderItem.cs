using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    public class FolderItem : FileItem
    {
        private Rectangle position;

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public FolderItem(string fileName)
            : base(fileName)
        {
        }
    }

}
