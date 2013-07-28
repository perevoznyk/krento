//-----------------------------------------------------------------------------
//
//  DwmApi
//
//  Managed wrapper of DWM API functions.
//
//-----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Laugris.Sage
{
    [SuppressUnmanagedCodeSecurity]
    internal static partial class NativeMethods
    {
        //
        //  Composition
        //

        [DllImport("DwmApi.dll")]
        public static extern int DwmEnableComposition([MarshalAs(UnmanagedType.Bool)] bool enabled);

        [DllImport("DwmApi.dll")]
        public static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] ref bool enabled);


        [DllImport("DwmApi", EntryPoint = "#106")]
        public static extern int DwmpIsCompositionCapable([MarshalAs(UnmanagedType.Bool)] ref bool fCapable1, [MarshalAs(UnmanagedType.Bool)] ref bool fCapable2);
 

        //
        //  Thumbnails
        //

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            public int dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        };


        //
        //  Window Attributes
        //
        public const int DWMNCRP_USEWINDOWSTYLE = 0;  // Enable/disable non-client rendering based on window style
        public const int DWMNCRP_DISABLED = 1;        // Disabled non-client rendering; window style is ignored
        public const int DWMNCRP_ENABLED = 2;         // Enabled non-client rendering; window style is ignored

        public const int DWMWA_NCRENDERING_ENABLED = 1;       // Enable/disable non-client rendering Use DWMNCRP_* values
        public const int DWMWA_NCRENDERING_POLICY = 2;        // Non-client rendering policy
        public const int DWMWA_TRANSITIONS_FORCEDISABLED = 3; // Potentially enable/forcibly disable transitions 0 or 1

        public const int WM_THEMECHANGED = 0x031A;
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        public const int WM_DWMNCRENDERINGCHANGED = 0x031F;
        public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
        public const int WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321;


        //
        //  Multi-Media
        //

        [StructLayout(LayoutKind.Sequential)]
        public struct UNSIGNED_RATIO
        {
            public UInt32 uiNumerator;
            public UInt32 uiDenominator;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_PRESENT_PARAMETERS
        {
            public int cbSize;
            public bool fQueue;
            public UInt64 cRefreshStart;
            public uint cBuffer;
            public bool fUseSourceRate;
            public UNSIGNED_RATIO uiNumerator;
        };




        //
        //  Client Area Blur
        //

        public const int DWM_BB_ENABLE = 0x00000001;  // fEnable has been specified
        public const int DWM_BB_BLURREGION = 0x00000002;  // hRgnBlur has been specified
        public const int DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;  // fTransitionOnMaximized has been specified

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public int dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };


        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref MARGINS m);



    }

}
