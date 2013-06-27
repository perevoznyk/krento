using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Laugris.Sage
{
    /// <summary>
    /// The central entry point for the application
    /// This class holds the references for all important classes
    /// to save memory and avoid GC too much
    /// </summary>
    public static class CentralPoint
    {
        private static MemIniFile appImages;

        public static void Startup()
        {
            appImages = new MemIniFile(GlobalConfig.AppImagesFileName, true);
        }


        public static void Shutdown()
        {
            if (appImages != null)
            {
                appImages.Dispose();
                appImages = null;
            }
        }


        public static string CustomFileIcon(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || (appImages == null))
                return null;

            string imageName = null;

            if (FileOperations.FileIsLink(fileName))
            {
                string tempName = FileOperations.ExtractFileNameFromShellLink(fileName);
                if (!string.IsNullOrEmpty(tempName))
                    fileName = tempName;
            }

            string shortName = Path.GetFileName(fileName);
            imageName = appImages.ReadString("Program", shortName);
            if (!string.IsNullOrEmpty(imageName))
            {
                if (FileOperations.FileExists(imageName))
                {
                    return imageName;
                }
            }
            return null;
        }
    }
}
