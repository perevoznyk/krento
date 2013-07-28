using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;
using System.IO;

namespace Krento.RollingStones
{
    public class LiveFolder : FolderView
    {
        private readonly string path;

        private static Bitmap GetLogoFromRing(string ringName)
        {
            if (!FileOperations.FileExists(ringName))
                return null;

            KrentoRing ring = new KrentoRing(ringName);
            Bitmap result = BitmapPainter.ConvertToRealColors(ring.Logo, false);
            ring.Dispose();
            return result;
        }

        public LiveFolder(IntPtr parentWindow, string path)
            : base(parentWindow)
        {
            this.path = path;
            this.Font = new Font("Tahoma", 8);
            this.Name = "LiveFolder";
        }

        protected override void BuildItems()
        {
            FolderItem item;
            string fullTarget;
            DisposeItems();
            string fullPath = FileOperations.StripFileName(path);
            string[] files = Directory.GetFileSystemEntries(fullPath);

            HeaderText = Path.GetFileNameWithoutExtension(fullPath);

            item = new FolderItem(fullPath);
            item.Logo = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("Folder.png"), FileImage.ImageSize, FileImage.ImageSize, true);
            Items.Add(item);

            int totalItems = 0;

            for (int i = 0; i < files.Length; i++)
            {
                fullTarget = files[i];
                if ((File.GetAttributes(fullTarget) & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;

                totalItems++;

                if (totalItems > Limit)
                {
                    totalItems = Limit;
                    break;
                }

                item = new FolderItem(fullTarget);

                if (TextHelper.SameText(Path.GetExtension(fullTarget), ".circle"))
                    item.Logo = GetLogoFromRing(fullTarget);

                if (item.Logo == null)
                    item.Logo = (Bitmap)FileImage.FileNameImage(fullTarget);

                if (item.Logo == null)
                {
                    if (FileOperations.DirectoryExists(fullTarget))
                    {
                        item.Logo = NativeThemeManager.LoadBitmap("Folder.png");
                    }
                    else
                    {
                        item.Logo = NativeThemeManager.LoadBitmap("UnknownFile.png");
                    }
                }

                Items.Add(item);
            }

            files = null;
        }

    }
}
