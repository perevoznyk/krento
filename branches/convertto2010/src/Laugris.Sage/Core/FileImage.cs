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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;

namespace Laugris.Sage
{

    // In order to draw the images with best possible quality FileImage class
    // should try to extract the biggest/best icon for the file type or the file name

    /// <summary>
    /// FileImage class can be used to extract an image that represents the file type or
    /// the image, associated with an executable
    /// </summary>
    public static class FileImage
    {
        private static int imageSize = 64;

        public static string CustomFileIcon(string fileName)
        {
            return CentralPoint.CustomFileIcon(fileName);
        }


        private static IntPtr GetProcessHandle()
        {
            return NativeMethods.GetModuleHandle(null);
        }

        public static int ImageSize
        {
            get { return imageSize; }
            set { imageSize = value; }
        }

        //Replacement for Icon.ToBitmap()
        public static Image ConvertIconToBitmap(Icon icon)
        {
            if (icon == null)
                return null;

            if (icon.Size.IsEmpty)
                return null;

            Bitmap bitmap = new Bitmap(icon.Width, icon.Height, PixelFormat.Format32bppPArgb);
            Graphics canvas = Graphics.FromImage(bitmap);
            try
            {
                canvas.InterpolationMode = InterpolationMode.NearestNeighbor;
                canvas.SmoothingMode = SmoothingMode.HighQuality;
                Bitmap iconBitmap = icon.ToBitmap();
                try
                {
                    canvas.DrawImage(iconBitmap, new Rectangle(0, 0, icon.Width, icon.Height));
                }
                finally
                {
                    iconBitmap.Dispose();
                }
            }
            finally
            {
                canvas.Dispose();
            }

            return bitmap;
        }

        /// <summary>
        /// Get the image for the specified file type. 
        /// You can provide the full file name, for example doc1.docx
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Image or null. The size of the image = size of the icon</returns>
        private static Image FileTypeImage(string fileName)
        {
            Image result = null;
            Icon icon = null;
            IconFlags flags;
            IntPtr hIcon = IntPtr.Zero;
            IntPtr shellInfo;
            IconSize size;
            IImageList iImageList = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                SHFILEINFO fileInfo = new SHFILEINFO();
                fileName = FileSearch.FullPath(fileName);

                if (FileOperations.FileOrFolderExists(fileName))
                    fileInfo.dwAttributes = (uint)File.GetAttributes(fileName);

                //first try to query the icon location and get the icon from there
                flags = IconFlags.LargeIcon | IconFlags.IconLocation | IconFlags.UseFileAttributes;
                shellInfo = NativeMethods.SHGetFileInfo(fileName, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), flags);
                if (shellInfo == (IntPtr)1)
                {
                    //we have the result, try to parse it
                    if (FileOperations.FileIsIcon(fileInfo.szDisplayName))
                    {
                        icon = new Icon(fileInfo.szDisplayName);
                        result = ConvertIconToBitmap(icon);
                        icon.Dispose();
                        return result;
                    }
                    else
                        if (FileOperations.FileIsExe(fileName))
                        {
                            result = ExtractIconFromExecutable(fileInfo.szDisplayName, (int)fileInfo.iIcon);
                        }
                }

                if (result != null)
                    return result;

                //if we have no result: continue to search using system image list
                flags = IconFlags.LargeIcon | IconFlags.SysIconIndex;
                if (!FileOperations.FileOrFolderExists(fileName))
                    flags = flags | IconFlags.UseFileAttributes;
                shellInfo = NativeMethods.SHGetFileInfo(fileName, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), flags);

                if (shellInfo == IntPtr.Zero)
                    return null;


                size = IconSize.ExtraLarge;

                if (InteropHelper.RunningOnXP)
                {
                    Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                    NativeMethods.SHGetImageList(
                        (int)size,
                        ref iidImageList,
                        ref iImageList
                        );
                    // the image list handle is the IUnknown pointer, but
                    // using Marshal.GetIUnknownForObject doesn't return
                    // the right value.  It really doesn't hurt to make
                    // a second call to get the handle:
                    NativeMethods.SHGetImageListHandle((int)size, ref iidImageList, ref shellInfo);
                }

                if (iImageList == null)
                    hIcon = NativeMethods.ImageList_GetIcon(shellInfo, (int)fileInfo.iIcon, NativeMethods.ILD_TRANSPARENT);
                else
                    iImageList.GetIcon((int)fileInfo.iIcon, (int)NativeMethods.ILD_TRANSPARENT, ref hIcon);

                if (hIcon == IntPtr.Zero)
                    return null;

                icon = Icon.FromHandle(hIcon);
                //do not destroy icon from the system image list
                //NativeMethods.DestroyIcon(hIcon);
                result = ConvertIconToBitmap(icon);
            }


            if (icon != null)
                icon.Dispose();

            if (result != null)
                result = BitmapPainter.ResizeBitmap(result, imageSize, imageSize, true);

            return result;
        }


        [SecurityPermission(SecurityAction.LinkDemand)]
        public static Image VirtualFolderImage(int folderId)
        {
            IntPtr pidl;
            Image result = null;
            IntPtr shellInfo;
            IconFlags flags;
            SHFILEINFO fileInfo = new SHFILEINFO();
            Icon icon = null;
            IntPtr hIcon = IntPtr.Zero;
            IconSize size;
            IImageList iImageList = null;

            if (NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, folderId, out pidl) != 0)
                return null;

            flags = IconFlags.PIDL | IconFlags.LargeIcon | IconFlags.SysIconIndex | IconFlags.UseFileAttributes;
            shellInfo = NativeMethods.SHGetFileInfo(pidl, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), flags);
            if (shellInfo != IntPtr.Zero)
            {


                size = IconSize.ExtraLarge;

                if (InteropHelper.RunningOnXP)
                {
                    Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                    NativeMethods.SHGetImageList(
                        (int)size,
                        ref iidImageList,
                        ref iImageList
                        );
                    // the image list handle is the IUnknown pointer, but
                    // using Marshal.GetIUnknownForObject doesn't return
                    // the right value.  It really doesn't hurt to make
                    // a second call to get the handle:
                    NativeMethods.SHGetImageListHandle((int)size, ref iidImageList, ref shellInfo);
                }

                if (iImageList == null)
                    hIcon = NativeMethods.ImageList_GetIcon(shellInfo, (int)fileInfo.iIcon, NativeMethods.ILD_TRANSPARENT);
                else
                    iImageList.GetIcon((int)fileInfo.iIcon, (int)NativeMethods.ILD_TRANSPARENT, ref hIcon);


                if (hIcon != IntPtr.Zero)
                {
                    icon = (Icon)Icon.FromHandle(hIcon);
                    //NativeMethods.DestroyIcon(hIcon);
                    result = ConvertIconToBitmap(icon);
                }

            }
            Marshal.FreeCoTaskMem(pidl);

            if (icon != null)
            {
                icon.Dispose();
                icon = null;
            }

            if (result != null)
                result = BitmapPainter.ResizeBitmap(result, imageSize, imageSize, true);
            return result;
        }

        private static Image ExtractIconFromExecutable(string fileName)
        {
            Image result = null;
            Icon icon = null;

            using (IconHelper iconHelper = new IconHelper(fileName, imageSize))
            {
                if (iconHelper.Icon != null)
                    result = (Bitmap)iconHelper.Icon.Clone();
            }

            if (result == null)
            {
                IntPtr iconHandle = NativeMethods.ExtractIcon(GetProcessHandle(), fileName, 0);
                if (iconHandle != IntPtr.Zero)
                {
                    icon = Icon.FromHandle(iconHandle);
                    NativeMethods.DestroyIcon(iconHandle);
                    result = ConvertIconToBitmap(icon);
                    icon.Dispose();
                    icon = null;
                    return result;
                }
                else
                    return null;
            }
            else
                return result;

        }

        private static Image ExtractIconFromExecutable(string fileName, int iconIndex)
        {
            Image result = null;
            Icon icon = null;

            using (IconHelper iconHelper = new IconHelper(fileName, imageSize, iconIndex))
            {
                result = iconHelper.Icon;
                if (result != null)
                    result = (Bitmap)result.Clone();
            }

            if (result == null)
            {
                IntPtr iconHandle = NativeMethods.ExtractIcon(GetProcessHandle(), fileName, iconIndex);
                if (iconHandle != IntPtr.Zero)
                {
                    icon = Icon.FromHandle(iconHandle);
                    NativeMethods.DestroyIcon(iconHandle);
                    result = ConvertIconToBitmap(icon);
                    icon.Dispose();
                    return result;
                }
                else
                    return null;
            }
            else
                return result;

        }


        private static Image TryExtractImageFromShellLink(string fileName)
        {
            Bitmap result = null;

            string targetName = FileOperations.ExtractFileNameFromShellLink(fileName);
            if (!string.IsNullOrEmpty(targetName))
            {
                //link uses original target icon
                if (FileOperations.FileIsExe(targetName))
                {
                    result = (Bitmap)ExtractIconFromExecutable(targetName);
                }
            }
            return result;
        }


        
        /// <summary>
        /// Get the image for specified file name. If the file has no embedded icon,
        /// this method will return default image for the file extension
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Image or null. The size of the image = size of the icon</returns>
        public static Image FileNameImage(string fileName)
        {
            
            Image result;

            try
            {
                result = FileNameImageInternal(fileName);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
                result = NativeThemeManager.LoadBitmap("UnknownFile.png");
            }

            return result;
        }
        
        
        private static Image FileNameImageInternal(string fileName)
        {
            Image result = null;
            Icon icon = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = FileOperations.StripFileName(fileName);
                //if we have only name of the file without path
                //try to find it in the search path of Windows
                fileName = FileSearch.FullPath(fileName);

                if (!FileOperations.FileOrFolderExists(fileName))
                {
                    result = FileTypeImage(fileName);
                    return result;
                }

                //if this is a link to executable with original
                //executable icon
                if (FileOperations.FileIsLink(fileName))
                {
                    result = TryExtractImageFromShellLink(fileName);
                    if (result != null)
                        return result;
                }

                if (FileOperations.FileIsImage(fileName))
                {
                    try
                    {
                        result = Image.FromFile(fileName);
                        result = BitmapPainter.ResizeBitmap(result, imageSize, imageSize, true);
                    }
                    catch
                    {
                        result = null;
                    }
                    if (result != null)
                        return result;
                }

                // 1. The file is icon 
                // 2. The file is exe or dll
                // 3. The file is a shell link .lnk
                // 4. Other file types
                if (FileOperations.FileIsIcon(fileName))
                    //Simply load icon from file
                    icon = new Icon(fileName);
                else
                    //Extract icon from exe or dll directly
                    if (FileOperations.FileIsExe(fileName))
                    {
                        return ExtractIconFromExecutable(fileName);
                    }
                    else
                    {
                        result = FileTypeImage(fileName);
                        return result;
                    }

                result = ConvertIconToBitmap(icon);
            }

            if (icon != null)
                icon.Dispose();

            return result;
        }
    }
}
