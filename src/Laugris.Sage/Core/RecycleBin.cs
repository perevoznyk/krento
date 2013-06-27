using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    /// <summary>
    /// Send files directly to the recycle bin.
    /// </summary>
    public static class RecycleBin
    {

        /// <summary>
        /// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static void Send(string path) 
        {
            NativeMethods.DeleteToRecycleBin(path, false);
        }

        /// <summary>
        /// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static void SendSilent(string path)
        {
            NativeMethods.DeleteToRecycleBin(path, true);
        }

        public static bool IsEmpty
        {
            get { return NativeMethods.RecycleBinEmpty(); }
        }

        public static void Clear()
        {
            NativeMethods.EmptyRecycleBin();
        }
    }
}
