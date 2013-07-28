using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace Laugris.Sage
{
    public static class DesktopHelper
    {
        private static List<IntPtr> Result = new List<IntPtr>();

        private static bool WindowEnumProc(IntPtr hwnd, IntPtr lParam)
        {
            if (NativeMethods.IsGoodWindow(hwnd))
                Result.Add(hwnd);

            return true;
        }

        public static IntPtr[] GetDesktopWindows()
        {
            Result.Clear();
            EnumWindowsProc enumWindowsProc = new EnumWindowsProc(WindowEnumProc);
            NativeMethods.EnumDesktopWindows(IntPtr.Zero, enumWindowsProc, IntPtr.Zero);
            GC.KeepAlive(enumWindowsProc);
            return Result.ToArray();
        }

        public static string GetWindowText(IntPtr hwnd)
        {
            int windowTextLength = NativeMethods.GetWindowTextLength(hwnd);
            if (SystemInformation.DbcsEnabled)
            {
                windowTextLength = (windowTextLength * 2) + 1;
            }
            StringBuilder lpString = new StringBuilder(windowTextLength + 1);
            int len = NativeMethods.GetWindowText(hwnd, lpString, lpString.Capacity);
            if (len > 0)
                return lpString.ToString();
            else
                return string.Empty;
        }

        public static string GetApplicationFromWindow(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(262);
            if (NativeMethods.GetApplicationFromWindow(hwnd, sb))
                return sb.ToString();
            else
                return null;
        }
    }
}
