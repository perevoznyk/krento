using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    /// <summary>
    /// Associate file extension with application. All credit goes to cristiscu
    /// If user is administrator then global settings are used, otherwise this method works
    /// for the current user only
    /// </summary>
    public static class FileAssociation
    {
        // Associate file extension with progID, description, icon and application
        public static void Associate(string extension,
               string progID, string description, string icon, string application, int iconIndex)
        {
            string classesKey = @"SOFTWARE\Classes";
            string iconKey;

            using (RegistryKey rootKey = Registry.CurrentUser.OpenSubKey(classesKey, true))
            {
                using (RegistryKey key = rootKey.CreateSubKey(extension))
                {
                    key.SetValue("", progID);
                }

                if (!string.IsNullOrEmpty(progID))
                    using (RegistryKey key = rootKey.CreateSubKey(progID))
                    {
                        if (description != null)
                            key.SetValue("", description);
                        if (icon != null)
                        {
                            iconKey = ToShortPathName(icon);
                            if (iconIndex > -1)
                                iconKey = iconKey + "," + iconIndex.ToString();

                            using (RegistryKey subKey = key.CreateSubKey("DefaultIcon"))
                            {
                                subKey.SetValue("", iconKey);
                            }
                        }
                        if (application != null)
                        {
                            using (RegistryKey subKey = key.CreateSubKey(@"Shell\Open\Command"))
                            {
                                subKey.SetValue("",
                                            ToShortPathName(application) + " \"%1\"");
                            }
                        }
                    }
            }

            NativeMethods.NotifyOfChange();
        }

        // Return true if extension already associated in registry
        public static bool IsAssociated(string extension)
        {
            bool result;
            string classesKey = @"SOFTWARE\Classes";
            using (RegistryKey rootKey = Registry.CurrentUser.OpenSubKey(classesKey, false))
            {
                using (RegistryKey subKey = rootKey.OpenSubKey(extension, false))
                {
                    result = (subKey != null);
                }
            }
            return result;
        }


        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(260);
            uint iSize = (uint)s.Capacity;
            NativeMethods.GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}
