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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    public static class WebsiteImage
    {

        /// <summary>
        /// Customs the site icon name.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <returns></returns>
        public static string CustomSiteIcon(string siteAddress)
        {
            if (File.Exists(GlobalConfig.SiteImagesFileName))
            {
                Uri siteUri = new Uri(siteAddress);
                string imageName = null;
                string siteHost = siteUri.Host;
                string[] parts = siteHost.Split('.');
                MemIniFile iniFile = new MemIniFile(GlobalConfig.SiteImagesFileName);
                try
                {
                    iniFile.Load();
                    foreach (var part in parts)
                    {
                        imageName = iniFile.ReadString("site", part);
                        if (!string.IsNullOrEmpty(imageName))
                        {
                            if (FileOperations.FileExists(imageName))
                            {
                                //return FileOperations.StripFileName(imageName);
                                return imageName;
                            }
                        }
                    }

                    //no image for url part, return default image
                    if (iniFile.SectionExists("default"))
                    {
                        imageName = iniFile.ReadString("default", "url");
                        if (!string.IsNullOrEmpty(imageName))
                        {
                            if (FileOperations.FileExists(imageName))
                            {
                                //return FileOperations.StripFileName(imageName);
                                return imageName;
                            }
                        }
                    }

                }
                finally
                {
                    iniFile.Dispose();
                }
            }

            return null;
        }

        public static Image DownloadSiteIcon(string siteAddress)
        {
            if (File.Exists(GlobalConfig.SiteImagesFileName))
            {
                Uri siteUri = new Uri(siteAddress);
                string imageName = null;
                string siteHost = siteUri.Host;
                string[] parts = siteHost.Split('.');
                MemIniFile iniFile = new MemIniFile(GlobalConfig.SiteImagesFileName);
                try
                {
                    iniFile.Load();
                    foreach (var part in parts)
                    {
                        imageName = iniFile.ReadString("site", part);
                        if (!string.IsNullOrEmpty(imageName))
                        {
                            if (FileOperations.FileExists(imageName))
                            {
                                return BitmapPainter.ResizeBitmap(FastBitmap.FromFile(FileOperations.StripFileName(imageName)), FileImage.ImageSize, FileImage.ImageSize, true);
                            }
                        }
                    }

                    //no image for url part, return default image
                    if (iniFile.SectionExists("default"))
                    {
                        imageName = iniFile.ReadString("default", "url");
                        if (!string.IsNullOrEmpty(imageName))
                        {
                            if (FileOperations.FileExists(imageName))
                            {
                                return BitmapPainter.ResizeBitmap(FastBitmap.FromFile(FileOperations.StripFileName(imageName)), FileImage.ImageSize, FileImage.ImageSize, true);
                            }
                        }
                    }

                }
                finally
                {
                    iniFile.Dispose();
                }
            }

            return null;
        }

    }
}
