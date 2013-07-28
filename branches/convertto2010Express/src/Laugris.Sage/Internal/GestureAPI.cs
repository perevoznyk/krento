using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{

    [SuppressUnmanagedCodeSecurity]
    internal static partial class NativeMethods
    {
        public const uint GC_ALLGESTURES = 1;
        public const uint GF_BEGIN = 1;
        public const uint GF_END = 4;
        public const uint GF_INERTIA = 2;
        public const uint GID_BEGIN = 1;
        public const uint GID_END = 2;
        public const uint GID_PAN = 4;
        public const uint GID_ROLLOVER = 7;
        public const uint GID_ROTATE = 5;
        public const uint GID_TWOFINGERTAP = 6;
        public const uint GID_ZOOM = 3;

        public const uint WM_GESTURE = 0x119;
        public const uint WM_GESTURENOTIFY = 0x11a;

        [DllImport("user32.dll")]
        public static extern bool CloseGestureInfoHandle(IntPtr hGestureInfo);

        [DllImport("user32.dll")]
        public static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        [DllImport("user32.dll")]
        public static extern bool SetGestureConfig(IntPtr hwnd, uint dwReserved, uint cIDs, GESTURECONFIG[] pGestureConfig, uint cbSize);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        public static double GID_ROTATE_ANGLE_FROM_ARGUMENT(ushort arg)
        {
            return ((((((double)arg) / 65535.0) * 4.0) * 3.14159265) - 6.2831853);
        }

        public static ushort GID_ROTATE_ANGLE_TO_ARGUMENT(ushort arg)
        {
            return (ushort)(((arg + 6.2831853) / 12.5663706) * 65535.0);
        }

    }
}
