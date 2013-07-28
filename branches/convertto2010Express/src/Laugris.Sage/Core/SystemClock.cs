using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public static class SystemClock
    {
        public static void Sleep(int milliseconds)
        {
            NativeMethods.Sleep(milliseconds);
        }

        public static int UptimeMillis()
        {
            return NativeMethods.GetTickCount();
        }


        public static int ElapsedRealtime()
        {
            return NativeMethods.GetTickCount();
        }

        public static DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
