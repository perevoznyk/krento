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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace Laugris.Sage
{
    /// <summary>
    /// This is a helper class to be used with FileImage class for extracting the icon from executable
    /// IconHelper loads the best suitable icon that is available inside executable according to the icon size
    /// </summary>
    internal class IconHelper : IDisposable
    {
        private IntPtr moduleHandle;
        private Image internalImage;
        private int desiredSize;
        private int desiredIndex;

        private List<string> resources;

        public IconHelper(string fileName, int desiredSize)
        {
            this.desiredSize = desiredSize;
            resources = new List<string>();
            fileName = FileOperations.StripFileName(fileName);
            moduleHandle = NativeMethods.LoadLibraryEx(fileName, LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
            desiredIndex = 0;
            if (moduleHandle != IntPtr.Zero)
                ProcessResources();
        }

        public IconHelper(string fileName, int desiredSize, int iconIndex)
        {
            this.desiredSize = desiredSize;
            resources = new List<string>();
            fileName = FileOperations.StripFileName(fileName);
            desiredIndex = iconIndex;
            moduleHandle = NativeMethods.LoadLibraryEx(fileName, LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
            if (moduleHandle != IntPtr.Zero)
                ProcessResources();
        }

        ~IconHelper()
        {
            Dispose(false);
        }

        public Image Icon
        {
            get
            {
                if (internalImage == null)
                    internalImage = GetIcon();
                return internalImage;
            }
        }

        private bool EnumResourcesCallBack(IntPtr hModule, ResourceTypes lpszType, IntPtr lpszName, IntPtr lParam)
        {
            if (lpszType == ResourceTypes.RT_GROUP_ICON)
            {
                if (((uint)lpszName >> 16) == 0)
                    resources.Add(lpszName.ToString());
                else
                    resources.Add(Marshal.PtrToStringAuto(lpszName));
            }
            return true;
        }

        private void ProcessResources()
        {
            if (moduleHandle != IntPtr.Zero)
            {
                NativeMethods.EnumResourceNames(this.moduleHandle, (IntPtr)ResourceTypes.RT_GROUP_ICON, new EnumResNameProc(EnumResourcesCallBack), IntPtr.Zero);
            }
        }

        private Image GetIcon()
        {
            Image icon = null;

            if (resources.Count == 0)
                return null;

            icon = GetResourceData();
            return icon;
        }

        private static bool IsIntResource(string value)
        {
            int iResult;
            return int.TryParse(value, out iResult);
        }


        private Image GetResourceData()
        {

            string resourceName;

            if (resources.Count < 1)
                return null;

            if (desiredIndex >= resources.Count)
                desiredIndex = 0;

            if (desiredIndex >= 0)
                resourceName = resources[desiredIndex];
            else
            {
                desiredIndex = Math.Abs(desiredIndex);
                resourceName = desiredIndex.ToString(CultureInfo.InvariantCulture);
            }

            IntPtr resourceInfo = IntPtr.Zero;
            IntPtr resourceData = IntPtr.Zero;
            IntPtr resourceLock = IntPtr.Zero;
            IntPtr ico = IntPtr.Zero;
            Bitmap bitmap = null;

            int resourceSize = 0;

            if (IsIntResource(resourceName))
            {
                int resourceNumber = int.Parse(resourceName, CultureInfo.InvariantCulture);
                if (resourceNumber == 0)
                    resourceInfo = NativeMethods.FindResource(moduleHandle,
                        Marshal.StringToHGlobalAuto("0"),
                        ResourceTypes.RT_GROUP_ICON);
                else
                    resourceInfo = NativeMethods.FindResource(moduleHandle,
                        (IntPtr)resourceNumber,
                        ResourceTypes.RT_GROUP_ICON);
            }
            else
                resourceInfo = NativeMethods.FindResource(moduleHandle, resourceName, ResourceTypes.RT_GROUP_ICON);

            if (resourceInfo == IntPtr.Zero)
                return null;

            resourceData = NativeMethods.LoadResource(moduleHandle, resourceInfo);
            if (resourceData == IntPtr.Zero)
                return null;

            resourceLock = NativeMethods.LockResource(resourceData);
            if (resourceLock == IntPtr.Zero)
                return null;


            //number od desired icon
            int nID = NativeMethods.LookupIconIdFromDirectoryEx(resourceLock, true, desiredSize, desiredSize, LoadIconFlags.LR_DEFAULTCOLOR);
            if (nID <= 0)
                return null;

            resourceInfo = NativeMethods.FindResource(moduleHandle,
                (IntPtr)nID, ResourceTypes.RT_ICON);

            if (resourceInfo == IntPtr.Zero)
                return null;

            resourceData = NativeMethods.LoadResource(moduleHandle, resourceInfo);
            if (resourceData == IntPtr.Zero)
                return null;

            resourceLock = NativeMethods.LockResource(resourceData);
            if (resourceLock == IntPtr.Zero)
                return null;

            resourceSize = NativeMethods.SizeofResource(moduleHandle, resourceInfo);
            if (resourceSize > 0)
            {

                ico = NativeMethods.CreateIconFromResourceEx(resourceLock, resourceSize, true,
                    0x00030000, 48, 48, LoadIconFlags.LR_DEFAULTCOLOR);
                if (ico == IntPtr.Zero)
                {
                    byte[] buf = new byte[resourceSize];
                    Marshal.Copy(resourceLock, buf, 0, buf.Length);

                    //The resource contains icon, but getting icon failed for some reason
                    if (buf[0] == 40)
                        return null;

                    //The resource contains PNG (check by signature). This can happens
                    //on Vista. Getting PNG directly from resource without conversion
                    if ((buf[0] == 0x89) && (buf[1] == 0x50) && (buf[2] == 0x4E))
                    {
                        MemoryStream stream = new MemoryStream(buf);
                        try
                        {
                            bitmap = new Bitmap(stream);
                        }
                        finally
                        {
                            stream.Dispose();
                        }
                        return bitmap;
                    }
                    else
                        return null;

                }
                else
                {
                    Icon icon = (Icon)System.Drawing.Icon.FromHandle(ico);
                    bitmap = (Bitmap)FileImage.ConvertIconToBitmap(icon);
                    NativeMethods.DestroyIcon(ico);
                    icon.Dispose();
                    return bitmap;
                }
            }
            else
                return null;

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (internalImage != null)
                    internalImage.Dispose();
            }

            if (moduleHandle != IntPtr.Zero)
                NativeMethods.FreeLibrary(moduleHandle);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
