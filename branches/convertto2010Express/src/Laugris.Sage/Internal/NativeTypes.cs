//===============================================================================
// Copyright (c)Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Laugris.Sage
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINTS
    {
        public short x;
        public short y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GESTURECONFIG
    {
        public uint dwID;
        public uint dwWant;
        public uint dwBlock;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GESTUREINFO
    {
        public uint cbSize;
        public uint dwFlags;
        public uint dwID;
        public IntPtr hwndTarget;
        public POINTS ptsLocation;
        public uint dwInstanceID;
        public uint dwSequenceID;
        public ulong ullArguments;
        public uint cbExtraArgs;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GESTURENOTIFYSTRUCT
    {
        public uint cbSize;
        public uint dwFlags;
        public IntPtr hwndTarget;
        public POINTS ptsLocation;
        public uint dwInstanceID;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupInput
    {
        public int GdiplusVersion;
        public IntPtr DebugEventCallback;
        public bool SuppressBackgroundThread;
        public bool SuppressExternalCodecs;

        public static StartupInput GetDefaultStartupInput()
        {
            StartupInput result = new StartupInput();
            result.GdiplusVersion = 1;
            result.SuppressBackgroundThread = false;
            result.SuppressExternalCodecs = false;
            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupOutput
    {
        public IntPtr Hook;
        public IntPtr Unhook;
    }


    internal enum ChangeWindowMessageFilterFlags : int
    {
        Add = 1, Remove = 2
    };

    internal enum IconDrawingFlags
    {
        DI_MASK = 1,
        DI_IMAGE = 2,
        DI_NORMAL = 3,
        DI_COMPAT = 4,
        DI_DEFAULTSIZE = 8
    }

    internal enum LoadIconFlags : int
    {
        LR_DEFAULTCOLOR = 0,
        LR_MONOCHROME = 1
    }

    /// <summary>
    /// The LoadLibraryExFlags enemeration contains flags that control 
    /// how a .dll file is loaded with the NativeMethods.LoadLibraryEx 
    /// function
    /// </summary>
    internal enum LoadLibraryExFlags
    {
        /// <summary>
        /// If this value is used, and the executable module is a DLL, 
        /// the system does not call DllMain for process and thread 
        /// initialization and termination. Also, the system does not 
        /// load additional executable modules that are referenced by 
        /// the specified module. If this value is not used, and the 
        /// executable module is a DLL, the system calls DllMain for 
        /// process and thread initialization and termination. The system 
        /// loads additional executable modules that are referenced by 
        /// the specified module
        /// </summary>
        DONT_RESOLVE_DLL_REFERENCES = 1,

        /// <summary>
        /// If this value is used, the system maps the file into the calling 
        /// process's virtual address space as if it were a data file. Nothing 
        /// is done to execute or prepare to execute the mapped file. Use 
        /// this flag when you want to load a DLL only to extract messages 
        /// or resources from it
        /// </summary>
        LOAD_LIBRARY_AS_DATAFILE = 2,

        /// <summary>
        /// If this value is used, and lpFileName specifies a path, the 
        /// system uses the alternate file search strategy to find associated 
        /// executable modules that the specified module causes to be loaded. 
        /// If this value is not used, or if lpFileName does not specify a 
        /// path, the system uses the standard search strategy to find 
        /// associated executable modules that the specified module causes 
        /// to be loaded
        /// </summary>
        LOAD_WITH_ALTERED_SEARCH_PATH = 8,

        /// <summary>
        /// If this value is used, the system does not perform automatic 
        /// trust comparisons on the DLL or its dependents when they are 
        /// loaded
        /// </summary>
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 16
    }

    internal enum ResourceTypes : int
    {
        RT_ICON = 3,
        RT_GROUP_ICON = 14
    }

    /// <summary>
    /// Contains information about a file object
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHFILEINFO
    {
        /// <summary>
        /// A handle to the icon that represents the file. You are responsible for destroying this 
        /// handle with DestroyIcon when you no longer need it.
        /// </summary>
        public IntPtr hIcon;
        /// <summary>
        /// The index of the icon image within the system image list.
        /// </summary>
        public IntPtr iIcon;
        /// <summary>
        /// An array of values that indicates the attributes of the file object.
        /// </summary>
        public uint dwAttributes;
        /// <summary>
        /// A string that contains the name of the file as it appears in the Microsoft 
        /// Windows Shell, or the path and file name of the file that contains the icon representing 
        /// the file
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        /// <summary>
        /// A string that describes the type of file
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [Flags]
    internal enum IconSize : uint
    {

        /// <summary>
        /// Specify large icon - 32 pixels by 32 pixels.
        /// </summary>
        Large = 0x0,
        /// <summary>
        /// Specify small icon - 16 pixels by 16 pixels.
        /// </summary>
        Small = 0x1,
        /// <summary>
        /// Specify extra large icon - 48 pixels by 48 pixels.
        /// Only available under XP and latter; other OS return the Large Icon ImageList.
        /// </summary>
        ExtraLarge = 0x2,
        /// <summary>
        /// These images are the size specified by GetSystemMetrics called with SM_CXSMICON and GetSystemMetrics called with SM_CYSMICON.
        /// </summary>
        SysSmall = 0x3,
        /// <summary>
        /// Windows Vista and later. The image is normally 256x256 pixels.
        /// </summary>
        Jumbo = 0x4
    }

    [Flags]
    internal enum IconFlags : int
    {
        Icon = 0x000000100,             // get icon
        DisplayName = 0x000000200,      // get display name
        TypeName = 0x000000400,         // get type name
        Attributes = 0x000000800,       // get attributes
        IconLocation = 0x000001000,     // get icon location
        ExeType = 0x000002000,          // return exe type
        SysIconIndex = 0x000004000,     // get system icon index
        LinkOverlay = 0x000008000,      // put a link overlay on icon
        Selected = 0x000010000,         // show icon in selected state
        AttrSpecified = 0x000020000,    // get only specified attributes
        LargeIcon = 0x000000000,        // get large icon
        SmallIcon = 0x000000001,        // get small icon
        OpenIcon = 0x000000002,         // get open icon
        ShellIconSize = 0x000000004,    // get shell size icon
        PIDL = 0x000000008,             // pszPath is a pidl
        UseFileAttributes = 0x000000010 // use passed dwFileAttribute
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;        // x offest from the upperleft of bitmap
        public int yBitmap;        // y offset from the upperleft of bitmap
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }

    [Flags]
    internal enum SLGP_FLAGS
    {
        SLGP_RAWPATH = 4,
        SLGP_SHORTPATH = 1,
        SLGP_UNCPRIORITY = 2
    }

    [Flags]
    internal enum SLR_FLAGS
    {
        SLR_ANY_MATCH = 2,
        SLR_INVOKE_MSI = 0x80,
        SLR_NO_UI = 1,
        SLR_NOLINKINFO = 0x40,
        SLR_NOSEARCH = 0x10,
        SLR_NOTRACK = 0x20,
        SLR_NOUPDATE = 8,
        SLR_UPDATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public int rcPaint_left;
        public int rcPaint_top;
        public int rcPaint_right;
        public int rcPaint_bottom;
        public bool fRestore;
        public bool fIncUpdate;
        public int reserved1;
        public int reserved2;
        public int reserved3;
        public int reserved4;
        public int reserved5;
        public int reserved6;
        public int reserved7;
        public int reserved8;
    }

    /// <summary>
    /// A Wrapper for a POINT struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        /// <summary>
        /// The X coordinate of the point
        /// </summary>
        public int x;

        /// <summary>
        /// The Y coordinate of the point
        /// </summary>
        public int y;

        /// <summary>
        /// Initialize the point
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// A Wrapper for a SIZE struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        /// <summary>
        /// Width
        /// </summary>
        public int cx;
        /// <summary>
        /// Height
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public int cy;
    };

    /// <summary>
    /// A Wrapper for a RECT struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        /// <summary>
        /// Position of left edge
        /// </summary>
        public int left;

        /// <summary>
        /// Position of top edge
        /// </summary>
        public int top;

        /// <summary>
        /// Position of right edge
        /// </summary>
        public int right;

        /// <summary>
        /// Position of bottom edge
        /// </summary>
        public int bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public RECT(int width, int height)
        {
            this.left = 0;
            this.top = 0;
            this.right = width;
            this.bottom = height;
        }

        public RECT(Rectangle r)
        {
            this.left = r.Left;
            this.top = r.Top;
            this.right = r.Right;
            this.bottom = r.Bottom;
        }

        public override bool Equals(object obj)
        {
            RECT r = (RECT)obj;
            return (r.left == left && r.right == right && r.top == top && r.bottom == bottom);
        }

        public override int GetHashCode()
        {
            // Attempting a minor degree of "hash-ness" here
            return ((left ^ top) ^ right) ^ bottom;
        }

        public static bool operator ==(RECT a, RECT b)
        {
            return (a.left == b.left && a.right == b.right && a.top == b.top && a.bottom == b.bottom);
        }

        public static bool operator !=(RECT a, RECT b)
        {
            return !(a == b);
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0)]
    internal sealed class INITCOMMONCONTROLSEX
    {
        public int dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
        public int dwICC;
    }

    /// <summary>
    /// COPYDATASTRUCT for holding the data passed using WM_COPYDATA
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }


    [StructLayout(LayoutKind.Sequential)]
    internal sealed class BITMAPINFOHEADER
    {
        public int biSize = 40;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public IntPtr bmBits = IntPtr.Zero;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
    internal delegate IntPtr WndProcDelegate(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
    internal delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
    internal delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
    internal delegate bool EnumResNameProc(IntPtr hModule, ResourceTypes lpszType, IntPtr lpszName, IntPtr lParam);


    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int flags;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShDragImage
    {
        public SIZE sizeDragImage;
        public POINT ptOffset;
        public IntPtr hbmpDragImage;
        public int crColorKey;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class TRACKMOUSEEVENT
    {
        public int cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
        public uint dwFlags;
        public IntPtr hwndTrack;
        public uint dwHoverTime = 0;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class LOGFONT
    {
        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string lfFaceName;
        public LOGFONT()
        {
        }

        public LOGFONT(LOGFONT lf)
        {
            this.lfHeight = lf.lfHeight;
            this.lfWidth = lf.lfWidth;
            this.lfEscapement = lf.lfEscapement;
            this.lfOrientation = lf.lfOrientation;
            this.lfWeight = lf.lfWeight;
            this.lfItalic = lf.lfItalic;
            this.lfUnderline = lf.lfUnderline;
            this.lfStrikeOut = lf.lfStrikeOut;
            this.lfCharSet = lf.lfCharSet;
            this.lfOutPrecision = lf.lfOutPrecision;
            this.lfClipPrecision = lf.lfClipPrecision;
            this.lfQuality = lf.lfQuality;
            this.lfPitchAndFamily = lf.lfPitchAndFamily;
            this.lfFaceName = lf.lfFaceName;
        }

        public override string ToString()
        {
            return string.Concat(new object[] { 
                    "lfHeight=", this.lfHeight, ", lfWidth=", this.lfWidth, ", lfEscapement=", this.lfEscapement, ", lfOrientation=", this.lfOrientation, ", lfWeight=", this.lfWeight, ", lfItalic=", this.lfItalic, ", lfUnderline=", this.lfUnderline, ", lfStrikeOut=", this.lfStrikeOut, 
                    ", lfCharSet=", this.lfCharSet, ", lfOutPrecision=", this.lfOutPrecision, ", lfClipPrecision=", this.lfClipPrecision, ", lfQuality=", this.lfQuality, ", lfPitchAndFamily=", this.lfPitchAndFamily, ", lfFaceName=", this.lfFaceName
                 });
        }
    }

}
