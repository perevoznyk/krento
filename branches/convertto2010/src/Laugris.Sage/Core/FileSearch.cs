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
using Microsoft.Win32;

namespace Laugris.Sage
{
    /// <summary>
    /// Searh file in system search path or in registry
    /// </summary>
    public static class FileSearch
    {
        /// <summary>
        /// If file exists in the current folder then this method simply returns the file name.
        /// if not, it will search the file with provided name in every folder, specified in the system
        /// search path. 
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string FullPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            // search machine
            try
            {
                StringBuilder sb = new StringBuilder(260);
                if (NativeMethods.FullPath(fileName, sb))
                    return sb.ToString();
                else
                    return fileName;
            }
            catch (Exception ex)
            {
                //oops!
                TraceDebug.Trace("FileSearch.FullPath: " + ex.Message);
                return fileName;
            }
        }
    }
}
