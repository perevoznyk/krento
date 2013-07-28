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
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Windows.Forms;

namespace Laugris.Sage
{
    /// <summary>
    /// Helper class for common file oprations
    /// </summary>
    public static class FileOperations
    {

        private static readonly string ApplicationPrefix = GlobalConfig.ProductName.ToUpperInvariant();
#if PORTABLE
        private static readonly string EnvironmentMainFolder = "%" + ApplicationPrefix + "_PORTABLE%";
#endif
        private static readonly string EnvironmentDrive = "%" + ApplicationPrefix + "_DRIVE%";
        private static readonly string EnvironmentData = "%" + ApplicationPrefix + "_DATA%";

        public static string ExtractFileNameFromShellLink(string fileName)
        {

            if (string.IsNullOrEmpty(fileName))
                return null;

            StringBuilder sb = new StringBuilder(262);
            if (NativeMethods.ResolveShellLink(fileName, sb, 262))
            {
                return sb.ToString();
            }
            else
                return null;
        }

        public static bool IsKrentoPackage(string fileName)
        {
            return NativeMethods.FileIsKrentoPackage(fileName);
        }

        public static Image GetSiteLogo(string siteName)
        {
            Image fileLogo = null;
            fileLogo = (Bitmap)WebsiteImage.DownloadSiteIcon(siteName);
            if (fileLogo == null)
                fileLogo = NativeThemeManager.LoadBitmap("url.png");
            return fileLogo;
        }

        public static int GetFilesCount(string folderName)
        {
            return NativeMethods.GetFilesCount(folderName);
        }

        public static string GetExtension(string path, bool includeDot)
        {
            if (path == null)
            {
                return string.Empty;
            }
            if (!IsValidPathName(path))
                return string.Empty;

            int length = path.Length;
            int startIndex = length;
            while (--startIndex >= 0)
            {
                char ch = path[startIndex];
                if (ch == '.')
                {
                    if (startIndex != (length - 1))
                    {
                        if (includeDot)
                            return path.Substring(startIndex, length - startIndex);
                        else
                            return path.Substring(startIndex + 1, length - startIndex - 1);
                    }
                    return string.Empty;
                }
                if (((ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar)) || (ch == Path.VolumeSeparatorChar))
                {
                    break;
                }
            }
            return string.Empty;
        }


        public static Bitmap GetExtensionLogo(string fileName)
        {
            string extension = GetExtension(fileName, false);
            string fullName = Path.Combine(GlobalConfig.ExtIconsFolder, Path.ChangeExtension(extension, ".png"));
            if (FileExists(fullName))
                return FastBitmap.FromFile(fullName);
            else
                return null;
        }

        public static Image GetFileLogo(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NativeThemeManager.LoadBitmap("UnknownFile.png");

            Image fileLogo = null;

            if (IsURL(fileName))
            {
                fileLogo = (Bitmap)WebsiteImage.DownloadSiteIcon(fileName);
                if (fileLogo == null)
                    fileLogo = NativeThemeManager.LoadBitmap("url.png");

            }
            else
            {
                fileLogo = (Bitmap)FileImage.FileNameImage(fileName);

                if (fileLogo == null)
                {
                    if (FileOperations.DirectoryExists(fileName))
                    {
                        fileLogo = NativeThemeManager.LoadBitmap("Folder.png");
                    }
                    else
                    {
                        fileLogo = GetExtensionLogo(fileName);
                        if (fileLogo == null)
                            fileLogo = NativeThemeManager.LoadBitmap("UnknownFile.png");
                    }
                }

            }

            return fileLogo;
        }

        /// <summary>
        /// Deletes file to recycle bin.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void DeleteToRecycleBin(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            RecycleBin.SendSilent(fileName);
        }

        public static void ClearFileAttributes(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            NativeMethods.ClearFileAttributes(fileName);
        }

        public static string ExcludeTrailingPathDelimiter(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;
            else
            {
                if (fullPath.EndsWith(@"\") || fullPath.EndsWith(@"/"))
                    return fullPath.Substring(0, fullPath.Length - 1);
                else
                    return fullPath;
            }
        }

        public static string IncludeTrailingPathDelimiter(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;
            else
            {
                if (fullPath.EndsWith(@"\") || fullPath.EndsWith(@"/"))
                    return fullPath;
                else
                    return fullPath + "\\";
            }
        }

        public static bool IsValidFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return NativeMethods.IsValidFileName(path);
        }

        public static bool IsValidPathName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return NativeMethods.IsValidPathName(path);
        }

        public static void ShellCopyFile(string oldName, string newName)
        {
            try
            {
                NativeMethods.ShellCopyFile(oldName, newName);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Determines whether the specified URL string is URL.
        /// </summary>
        /// <param name="urlString">The URL string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URL string is URL; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsURL(string urlString)
        {
            if (!string.IsNullOrEmpty(urlString))
            {
                return NativeMethods.PathIsURL(urlString);
            }
            else
                return false;

        }

        public static string RemoveURI(string urlString)
        {
            try
            {
                Uri url = null;
                if (!string.IsNullOrEmpty(urlString))
                {
                    if (!Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out url))
                    {
                        return urlString;
                    }
                    else
                    {
                        if (!url.IsAbsoluteUri)
                            return urlString;

                        if (url.IsFile)
                            return url.LocalPath;
                        else
                            return
                                urlString;
                    }
                }
                else
                    return urlString;
            }
            catch
            {
                return urlString;
            }
        }

        /// <summary>
        /// Expand environment values and relace it with the full value. This method removes
        /// environment varaibles from the file name. To insert environment variable to the 
        /// file name use <see cref="UnExpandPath"/> method
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>A string with each environment variable replaced by its value.</returns>
        public static string StripFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (IsURL(name))
                return name;

            StringBuilder sb = new StringBuilder(259);
            NativeMethods.StripFileName(name, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Replace known path by environment variables. This function does the opposite work to <see cref="StripFileName"/> function
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string UnExpandPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            string result;
            StringBuilder sb = new StringBuilder(259);
            bool b = NativeMethods.PathUnExpandEnvStrings(path, sb, sb.Capacity);
            if (b)
                result = sb.ToString();
            else
            {
                result = path;
#if PORTABLE
                result = result.Replace(GlobalConfig.MainFolder, EnvironmentMainFolder);
                result = result.Replace(GlobalConfig.ApplicationDrive, EnvironmentDrive);
#else
                result = result.Replace(GlobalConfig.MainFolder, EnvironmentData);
                result = result.Replace(GlobalConfig.ApplicationDrive, EnvironmentDrive);
#endif
            }
            return result;
        }

        public static bool FileExtensionIs(string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(extension))
                return false;

            return NativeMethods.FileExtensionIs(fileName, extension);

        }

        public static string FileNameFromPidl(IntPtr pidlItem)
        {
            StringBuilder sb = new StringBuilder(260);
            if (NativeMethods.GetFileNameFromPidl(pidlItem, sb, 260))
                return sb.ToString();
            else
                return null;
        }

        /// <summary>
        /// The file is an image (jpeg, bmp, gif, png or ico)
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static bool FileIsImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return NativeMethods.FileIsImage(fileName);

        }

        public static bool FileIsIcon(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return NativeMethods.FileIsIcon(fileName);
        }

        public static bool FileIsExe(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return NativeMethods.FileIsExe(fileName);
        }

        public static bool FileIsLink(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return NativeMethods.FileIsLink(fileName);
        }
        /// <summary>
        /// Gets the file or URL description.
        /// </summary>
        /// <param name="fileName">Name of the file or URL.</param>
        /// <returns>The description of the file</returns>
        public static string GetFileDescription(string fileName)
        {
            string targetDescription = null;

            try
            {
                if (IsURL(fileName))
                {
                    targetDescription = GetPageTitle(fileName);
                    if (string.IsNullOrEmpty(targetDescription))
                    {
                        targetDescription = fileName;
                    }
                }
                else
                {
                    //string fullName = StripFileName(fileName);
                    //targetDescription = Path.GetFileNameWithoutExtension(fullName);
                    StringBuilder sb = new StringBuilder(fileName, 262);
                    NativeMethods.GetFileDescription(sb);
                    targetDescription = sb.ToString();
                }
            }
            catch
            {
                targetDescription = fileName;
            }

            return targetDescription;
        }



        public static string GetPageTitle(string pageAddress)
        {
            if (pageAddress == null)
                return null;

            try
            {
                string result = null;
                Uri siteUri = new Uri(pageAddress);
                result = siteUri.Host;
                return result;
            }
            catch
            {
                return pageAddress;
            }

        }


        private static void FileNameCallBack(string fileName, IntPtr lParam)
        {
            GCHandle gch = (GCHandle)lParam;
            List<string> files = (List<string>)gch.Target;
            files.Add(fileName);
        }

        public static void GetAllFiles(List<string> files, string folderName, string searchMask)
        {
            GCHandle gch = GCHandle.Alloc(files);

            FolderEnumProc fep = new FolderEnumProc(FileNameCallBack);

            NativeMethods.GetAllFiles(FileNameCallBack, folderName, searchMask, (IntPtr)gch);

            gch.Free();
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void DeleteFile(string fileName)
        {
            try
            {
                NativeMethods.FileDelete(fileName);
            }
            catch
            {
                //error while deleting the file, ignore it
            }
        }


        public static void RenameFile(string oldName, string newName)
        {
            try
            {
                NativeMethods.FileRename(oldName, newName);
            }
            catch
            {
                //error while deleting the file, ignore it
            }

        }

        public static void CopyFile(string oldName, string newName)
        {
            try
            {
                NativeMethods.FileCopy(oldName, newName);
            }
            catch
            {
                //error while deleting the file, ignore it
            }

        }


        /// <summary>
        /// Checks if file exists
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <returns>True, if file exists, otherwise false</returns>
        public static bool FileExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return NativeMethods.FileExists(name);
        }

        public static bool DirectoryExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return NativeMethods.DirectoryExists(name
);
        }

        /// <summary>
        /// Check if file or folder with given name exists
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static bool FileOrFolderExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return NativeMethods.FileOrFolderExists(name);
        }


        private static object[] FileListToIDListArray(string[] files)
        {
            IntPtr pidl;
            int cbPidl;

            //
            // Allocate an array of objects (byte arrays) to store the
            // pidl of each item in the file list
            object[] apidl = new object[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                //
                // Obtain a pidl for the file
                pidl = NativeMethods.ILCreateFromPath(files[i]);
                cbPidl = (int)NativeMethods.ILGetSize(pidl);

                //
                // Convert the pidl to a byte array
                apidl[i] = (object)new byte[cbPidl];
                Marshal.Copy(pidl, (byte[])apidl[i], 0, cbPidl);

                //
                // Free the pidl
                NativeMethods.ILFree(pidl);
            }
            return apidl;
        }

        public static MemoryStream FileListToShellIDListArray(string[] files)
        {
            int cb = 0;
            int offset = 0;
            int i;
            uint pidlDesktop = 0;

            object[] apidl = null;
            MemoryStream stream = null;
            BinaryWriter streamWriter = null;

            //
            // Convert the array of paths to an array of pidls
            apidl = FileListToIDListArray(files);

            //
            // Determine the amount of memory required for the CIDA
            //
            // The 2 in the statement below is for the offset to the
            // folder pidl and the count field in the CIDA structure
            cb = offset = Marshal.SizeOf(typeof(uint)) * (apidl.Length + 2);
            for (i = 0; i < apidl.Length; i++)
            {
                cb += ((byte[])apidl[i]).Length; ;
            }

            //
            // Create a memory stream that we will write the CIDA into
            stream = new MemoryStream();

            //
            // Wrap the memory stream with a BinaryWriter object
            streamWriter = new BinaryWriter(stream);

            //
            // Write the cidl member of the CIDA structure
            streamWriter.Write(apidl.Length);

            //
            // Write the array of offsets for each pidl. Calculate each
            // offset as we go
            streamWriter.Write(offset);
            offset += Marshal.SizeOf(pidlDesktop);

            for (i = 0; i < apidl.Length; i++)
            {
                streamWriter.Write(offset);
                offset += ((byte[])apidl[i]).Length;
            }

            //
            // Write the parent folder pidl
            streamWriter.Write(pidlDesktop);

            //
            // Write the item pidls
            for (i = 0; i < apidl.Length; i++)
            {
                streamWriter.Write((byte[])apidl[i]);
            }

            //
            // Return the memory stream that contains the CIDA
            return stream;
        }


        public static bool IsDirectory(string folderName)
        {
            return NativeMethods.IsDirectory(folderName);
        }

        public static void CreateBackup(string fileName)
        {
            string DirectoryToZip = GlobalConfig.MainFolder;
            NativeMethods.CreateBackup(fileName, DirectoryToZip);
        }

        public static void ClearCacheFolder()
        {
            string[] files = Directory.GetFiles(GlobalConfig.RollingStonesCache, @"*.png");
            foreach (string fileName in files)
            {
                FileOperations.DeleteFile(fileName);
            }
        }

        /// <summary>
        /// Copies the files list to clipboard.
        /// </summary>
        /// <param name="files">The files.</param>
        public static void CopyFilesListToClipboard(string[] files)
        {
            DataObject data = new DataObject();
            MemoryStream cida = null;
            //
            // Obtain a CIDA for the give list of files
            cida = FileListToShellIDListArray(files);
            //
            // Create a data object that wraps the CIDA
            data.SetData("Shell IDList Array", true, cida);

            //
            // Copy the data object to the clipboard
            Clipboard.SetDataObject(data, false);
        }

    }
}
