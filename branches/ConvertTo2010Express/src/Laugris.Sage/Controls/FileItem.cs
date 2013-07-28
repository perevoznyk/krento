using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Laugris.Sage
{
    public class FileItem : IDisposable
    {
        private Bitmap logo;
        private string fileName;
        private string name;

        public FileItem()
        {
        }

        public FileItem(string fileName)
        {
            this.fileName = FileOperations.StripFileName(fileName);
            name = Path.GetFileNameWithoutExtension(FileOperations.StripFileName(fileName));
        }

        public Bitmap Logo
        {
            get { return logo; }
            set { logo = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                if (!string.IsNullOrEmpty(fileName))
                    name = Path.GetFileNameWithoutExtension(FileOperations.StripFileName(fileName));
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string ToString()
        {
            return Name;
        }

        public int Tag { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (logo != null)
                {
                    logo.Dispose();
                    logo = null;
                }
            }
        }

        #endregion
    }
}
