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
using System.IO;
using System.Globalization;

namespace Laugris.Sage
{
    public static class FoldersInfo
    {
        private static string productName = Application.ProductName;

        public static string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        public static string UserAppDataPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        public static string UserLocalAppDataPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
        }

        public static string CommonAppDataPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
        }

        /// <summary>
        /// Gets the data path for the current application based on common or user data folder name
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <returns></returns>
        public static string GetDataPath(string basePath)
        {
            //The @ symbol tells the string constructor to ignore escape characters and line breaks.
            string format = @"{0}\{1}";
            
            string path = string.Format(CultureInfo.CurrentCulture, format, new object[] { basePath, productName });
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {

                }
            }
            return path;
        }

        public static string ConcatenatePath(string basePath, string relatedPath)
        {
            string result = Path.Combine(basePath, relatedPath);
            if (!Directory.Exists(result))
            {
                try
                {
                    Directory.CreateDirectory(result);
                }
                catch
                {
                }

            }

            return result;
        }

        public static string UserSettingsFileName
        {
            get
            {
                string settingsFolder = GetDataPath(UserAppDataPath);
                string format = @"{0}\{1}";
                string fileName = string.Format(CultureInfo.CurrentCulture, format, new object[] { settingsFolder, "settings.ini" });
                return fileName;
            }
        }

        public static string ConstructUserSettingsFileName(string settingsFileName)
        {
            string settingsFolder = GetDataPath(UserAppDataPath);
            string format = @"{0}\{1}";
            string fileName = string.Format(CultureInfo.CurrentCulture, format, new object[] { settingsFolder, settingsFileName });
            return fileName;
        }

        public static string UserDocumentsPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

    }
}
