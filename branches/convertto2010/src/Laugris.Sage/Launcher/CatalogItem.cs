using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage.Launcher
{
    public sealed class CatalogItem : IDisposable
    {
        private Bitmap icon;

        public string FullPath { get; set; }
        public string ShortName { get; set; }
        public int Usage { get; set; }
        public ItemType ItemType;
        public string Description { get; set; }

        public CatalogItem(string fullPath)
        {
            this.FullPath = fullPath;
            this.ShortName = FileOperations.GetFileDescription(fullPath);
            this.ItemType = ItemType.FileSystem;
        }

        public CatalogItem(string fullPath, string shortName)
        {
            this.FullPath = fullPath;
            this.ShortName = shortName;
            this.ItemType = ItemType.FileSystem;
        }

        public CatalogItem(string fullPath, string shortName, string iconPath, ItemType itemType)
        {
            this.FullPath = fullPath;
            this.ShortName = shortName;
            this.ItemType = itemType;
            this.icon = FastBitmap.FromFile(iconPath);
        }

        public CatalogItem(string fullPath, string shortName, Bitmap icon, ItemType itemType)
        {
            this.FullPath = fullPath;
            this.ShortName = shortName;
            this.ItemType = itemType;
            this.icon = icon;
        }

        ~CatalogItem()
        {
            Dispose(false);
        }

        public void AssignIcon()
        {
            if (icon != null)
            {
                icon.Dispose();
                icon = null;
            }

            string fileCustomIcon = FileImage.CustomFileIcon(FileOperations.ExtractFileNameFromShellLink(FullPath));

            if (string.IsNullOrEmpty(fileCustomIcon))
            {
                icon = (Bitmap)FileImage.FileNameImage(FullPath);

                if (icon == null)
                {
                    if (FileOperations.DirectoryExists(FullPath))
                    {
                        icon = NativeThemeManager.LoadBitmap("Folder.png");
                    }
                    else
                    {
                        icon = NativeThemeManager.LoadBitmap("UnknownFile.png");
                    }
                }

            }
            else
                icon = FastBitmap.FromFile(fileCustomIcon);


        }

        public Bitmap Icon
        {
            get
            {
                if (icon == null)
                    AssignIcon();
                return icon;
            }
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (icon != null)
            {
                icon.Dispose();
                icon = null;
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
