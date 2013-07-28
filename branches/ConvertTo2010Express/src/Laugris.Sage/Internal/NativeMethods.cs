//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace Laugris.Sage
{

    /// <summary>
    /// Extracting Low and High order words from integer parameter 
    /// </summary>
    internal static class ParamConvertor
    {
        internal static int LoWord(int param)
        {
            return (param & 0xffff);
        }
        /// <summary>
        /// Extracts low order word from integer
        /// Uses for extracting the coordinates from Windows message 
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns>Low order word from integer</returns>
        internal static int LoWord(IntPtr param)
        {
            return LoWord((int)((long)param));
        }

        internal static int HiWord(int param)
        {
            return ((param >> 0x10) & 0xffff);
        }

        /// <summary>
        /// Extracts high order word from integer 
        /// Uses for extracting the coordinates from Windows message
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns>High order word from integer</returns>
        internal static int HiWord(IntPtr param)
        {
            return HiWord((int)((long)param));
        }

        internal static int SignedHIWORD(int n)
        {
            return (short)((n >> 0x10) & 0xffff);
        }

        internal static int SignedHIWORD(IntPtr n)
        {
            return SignedHIWORD((int)((long)n));
        }

        internal static int SignedLOWORD(int n)
        {
            return (short)(n & 0xffff);
        }

        internal static int SignedLOWORD(IntPtr n)
        {
            return SignedLOWORD((int)((long)n));
        }

    }

    [SuppressUnmanagedCodeSecurity]
    internal static partial class NativeMethods
    {

        public static readonly IntPtr SC_CLOSE = (IntPtr)61536;

        public const int ILD_MASK = 0x10;
        public const int ILD_NORMAL = 0;
        public const int ILD_ROP = 0x40;
        public const int ILD_TRANSPARENT = 1;
        public const int ILP_DOWNLEVEL = 1;
        public const int ILP_NORMAL = 0;

        public static IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static IntPtr HWND_BOTTOM = new IntPtr(1);


        public const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
        public const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
        public const int SPIF_SENDCHANGE = 0x2;
        public const long PRF_CLIENT = 0x00000004L;
        public const long PRF_ERASEBKGND = 0x00000008L;

        public const int TOKEN_QUERY = 0X00000008;

        public const int WHEEL_DELTA = 120;

        // API constant identifier for the WM_COPYDATA message
        public const int WM_COPYDATA = 0x4A;

        public const int CM_BASE = 0xB000;
        public const int CM_MOUSEMOVE = CM_BASE + 1;
        public const int CM_LBUTTONDOWN = CM_BASE + 2;
        public const int CM_MBUTTONDOWN = CM_BASE + 3;
        public const int CM_RBUTTONDOWN = CM_BASE + 4;
        public const int CM_LBUTTONUP = CM_BASE + 5;
        public const int CM_MBUTTONUP = CM_BASE + 6;
        public const int CM_RBUTTONUP = CM_BASE + 7;
        public const int CM_LBUTTONDBLCLK = CM_BASE + 8;
        public const int CM_MBUTTONDBLCLK = CM_BASE + 9;
        public const int CM_RBUTTONDBLCLK = CM_BASE + 10;
        public const int CM_MOUSEWHEEL = CM_BASE + 11;
        public const int CM_XBUTTONUP = CM_BASE + 12;
        public const int CM_XBUTTONDOWN = CM_BASE + 13;
        public const int CM_XBUTTONDBLCLK = CM_BASE + 14;
        public const int CM_STARTENGINE = CM_BASE + 15;
        public const int CM_DESKTOPCLICK = CM_BASE + 20;

        public const int CM_SPLASHCLOSE = CM_BASE + 53;
        public const int CM_CHECKUPDATE = CM_BASE + 56;

        public const int CM_STONE_BASE = CM_BASE + 100;
        public const int CM_CHANGE_STONE = CM_STONE_BASE + 1;
        public const int CM_UPDATE_ALL = CM_STONE_BASE + 2;
        public const int CM_UPDATE_REDRAW = CM_STONE_BASE + 3;
        public const int CM_UPDATE_FRAMES = CM_STONE_BASE + 4;
        public const int CM_REDRAW_MANAGER = CM_STONE_BASE + 5;
        public const int CM_REMOVE_STONE = CM_STONE_BASE + 6;
        public const int CM_ROTATE = CM_STONE_BASE + 7;
        public const int CM_REDRAW_BUTTONS = CM_STONE_BASE + 8;

        public const int CN_BASE = 0xBC00;
        public const int CN_PAINT = CN_BASE + 01; //Frame animation
        public const int CN_CLOSE = CN_BASE + 02;
        public const int CN_BEGININVOKE = CN_BASE + 03;
        public const int CN_INVOKE = CN_BASE + 04;
        public const int CN_VISIBLECHANGED = CN_BASE + 11;
        public const int CN_UPDATEPARENT = CN_BASE + 13;
        public const int CN_REPAINTCONTROLS = CN_BASE + 14;
        public const int CN_UPDATE = CN_BASE + 15;
        public const int CN_DRAW_BACKGROUND = CN_BASE + 16;

        /* XButton values are WORD flags */
        public const int XBUTTON1 = 0x0001;
        public const int XBUTTON2 = 0x0002;

        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        public const int BI_RGB = 0;
        public const int DIB_RGB_COLORS = 0;
        public const int CS_DBLCLKS = 8;

        public const int BCM_SETSHIELD = 0x0000160C;
        public const int EM_POSFROMCHAR = 0x00D6;

        private static int wmMouseEnterMessage = -1;
        private static int wmUnSubclass = -1;

        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_HOTKEY = 0x312;
        public const int WM_CREATE = 0x0001;
        public const int WM_DESTROY = 0x0002;
        public const int WM_PAINT = 0x000F;
        public const int WM_CLOSE = 0x0010;
        public const int WM_QUERYENDSESSION = 0x0011;
        public const int WM_QUIT = 0x0012;
        public const int WM_ENDSESSION = 0x0016;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_MOVE = 0x0003;
        public const int WM_SIZE = 0x0005;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHOVER = 0x02A1;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        public const int WM_NCRBUTTONUP = 0x00A5;
        public const int WM_NCRBUTTONDBLCLK = 0x00A6;
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        public const int WM_NCMBUTTONUP = 0x00A8;
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
        public const int WM_NCXBUTTONDOWN = 0x00AB;
        public const int WM_NCXBUTTONUP = 0x00AC;
        public const int WM_GETDLGCODE = 0x0087;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_KILLTIMER = 0x402;
        public const int WM_TIMER = 0x113;
        public const int WM_NCPAINT = 0x85;
        public const int WM_ERASEBKGND = 20;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_KILLFOCUS = 8;
        public const int WM_GETOBJECT = 0x003D;
        public const int WM_ACTIVATE = 0x0006;
        public const int WA_INACTIVE = 0;
        public const int WM_POWERBROADCAST = 536;
        public const int WM_CANCELMODE = 0x001F;

        public const int MA_NOACTIVATE = 3;
        public const int WA_CLICKACTIVE = 2;

        public const int PBT_APMRESUMEAUTOMATIC = 0x0012;

        public const int HTTRANSPARENT = -1;

        public const uint TME_HOVER = 0x00000001;
        public const uint TME_LEAVE = 0x00000002;
        public const uint TME_NONCLIENT = 0x00000010;
        public const uint TME_QUERY = 0x40000000;
        public const uint TME_CANCEL = 0x80000000;
        public const uint HOVER_DEFAULT = 0xFFFFFFFF;

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetMessageExtraInfo();

        [DllImport("kernel32.dll", EntryPoint = "GlobalAddAtomA", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll", EntryPoint = "GlobalDeleteAtom", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ushort GlobalDeleteAtom(ushort atom);

        [DllImport("kernel32.dll", EntryPoint = "GlobalFindAtomA", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ushort GlobalFindAtom(string lpString);

        public static bool GlobalCheckAtom(string lpString)
        {
            return (GlobalFindAtom(lpString) != 0);
        }

        public static void GlobalKillAtom(string lpString)
        {
            while (GlobalFindAtom(lpString) != 0)
            {
                GlobalDeleteAtom(GlobalFindAtom(lpString));
            }
        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32")]
        public extern static int SetProp(IntPtr hwnd, string lpString, IntPtr hData);

        [DllImport("user32")]
        public extern static IntPtr RemoveProp(IntPtr hwnd, string lpString);

        [DllImport("user32.dll")]
        public extern static IntPtr GetProp(IntPtr hwnd, string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr hwnd);

        [DllImport("oleacc.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, HandleRef pAcc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PathUnExpandEnvStrings(string pszPath, [Out] StringBuilder pszBuf, int cchBuf);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PathIsURL(string pszPath);

        [DllImport("Shell32", CharSet = CharSet.Unicode)]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string fileName, int iconIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr windowHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ReleaseDC(IntPtr windowHandle, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr windowHandle, int cmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr windowHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr windowHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll", CharSet = CharSet.Auto), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StretchBlt(IntPtr DestDC, int X, int Y, int Width, int Height, IntPtr SrcDC, int XSrc, int YSrc, int SrcWidth, int SrcHeight, uint Rop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern int SetStretchBltMode(IntPtr DC, StretchBltMode StretchMode);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
        int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc,
        int dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetObject(IntPtr hObject, int nSize, ref BITMAP bm);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetObject(IntPtr hObject, int nSize, ref LOGFONT lf);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string modName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void Sleep(int milliSeconds);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetTickCount();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags);

        [DllImport("comctl32.dll", EntryPoint = "_TrackMouseEvent"), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool TrackMouseEvent(TRACKMOUSEEVENT tme);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hwnd, WindowMessage msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, ref Rectangle wparam, ref Rectangle lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, ref Rectangle wparam, bool lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, object lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);

        [DllImport("kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr dest, IntPtr source, int dwcount);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In, MarshalAs(UnmanagedType.LPStruct)]BITMAPINFOHEADER pbmi, int iUsage, out IntPtr ppvBits, IntPtr hSection, int dwOffset);

        [DllImport("user32.dll", EntryPoint = "SetCapture", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool ReleaseCapture();

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CreateMutex(IntPtr lpSecurityAttributes, [MarshalAs(UnmanagedType.Bool)] bool initialOwner, string name);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr OpenMutex(int desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool KillTimer(HandleRef hwnd, HandleRef idEvent);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool KillTimer(IntPtr hwnd, IntPtr idEvent);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetTimer(HandleRef hWnd, HandleRef nIDEvent, int uElapse, HandleRef lpTimerProc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, int uElapse, IntPtr lpTimerProc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RedrawWindow(HandleRef hwnd, IntPtr rcUpdate, HandleRef hrgnUpdate, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetCursor(IntPtr hcursor);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowText(IntPtr hWnd, StringBuilder text);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder lpszFile, int cch);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern void DragFinish(IntPtr hDrop);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern void DragAcceptFiles(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool fAccept);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int RegisterDragDrop(IntPtr hwnd, IOleDropTarget target);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int RevokeDragDrop(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int CoCreateGuid(ref Guid pguid);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadWindowsCallback lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc callback, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool enable);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage([In, Out] ref MSG msg, IntPtr hwnd, int msgMin, int msgMax, int remove);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowUnicode(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMessageA([In, Out] ref MSG msg, IntPtr hWnd, int uMsgFilterMin, int uMsgFilterMax);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMessageW([In, Out] ref MSG msg, IntPtr hWnd, int uMsgFilterMin, int uMsgFilterMax);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage([In, Out] ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr DispatchMessageA([In] ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr DispatchMessageW([In] ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern void WaitMessage();

        [DllImport("kernel32.dll", SetLastError = true,
            ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc);

        [DllImport("gdiplus.dll")]
        public static extern int GdipCreateCachedBitmap(IntPtr pBitmap, IntPtr pGraphics, ref IntPtr pCachedBitmap);

        [DllImport("gdiplus.dll")]
        public static extern int GdipDrawCachedBitmap(IntPtr pGraphics, IntPtr pCachedBitmap, int x, int y);

        [DllImport("gdiplus.dll")]
        public static extern int GdipDeleteCachedBitmap(IntPtr pCachedBitmap);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GdipGetImageType(IntPtr image, out int type);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GdiplusShutdown(IntPtr token);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr BeginPaint(IntPtr hWnd, [In, Out] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsProc callback, IntPtr lParam);

        public static Encoding GetFileEncoding(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Encoding.Unicode;

            if (!FileOperations.FileExists(filePath))
                return Encoding.Unicode;

            if (IsUnicodeFile(filePath))
                return Encoding.Unicode;
            else
                return Encoding.ASCII;
        }


        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root
        /// </summary>
        /// <param name="pszPath">[in] A pointer to a null-terminated string of maximum length MAX_PATH
        /// that contains the path and file name. Both absolute and relative paths are valid.
        /// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter must be the address
        /// of an ITEMIDLIST (PIDL) structure that contains the list of item identifiers that uniquely
        /// identifies the file within the Shell's namespace. The pointer to an item identifier
        /// list (PIDL) must be a fully qualified PIDL. Relative PIDLs are not allowed.
        /// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag,
        /// this parameter does not have to be a valid file name.
        /// The function will proceed as if the file exists with the specified name and with the file
        /// attributes passed in the dwFileAttributes parameter. This allows you to obtain information
        /// about a file type by passing just the extension for pszPath and passing FILE_ATTRIBUTE_NORMAL
        /// in dwFileAttributes.
        /// This string can use either short (the 8.3 form) or long file names.</param>
        /// <param name="dwFileAttributes">A combination of one or more file attribute flags</param>
        /// <param name="psfi">The address of a SHFILEINFO structure to receive the file information</param>
        /// <param name="cbSizeFileInfo">The size, in bytes, of the SHFILEINFO structure pointed to by the psfi parameter</param>
        /// <param name="uFlags">The flags that specify the file information to retrieve.</param>
        /// <returns>
        /// Returns a value whose meaning depends on the uFlags parameter
        /// </returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, IconFlags uFlags);

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(IntPtr pidl, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, IconFlags uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern void NotifyWinEvent(int accEvent, IntPtr hwnd, uint objType, int objID);

        /// <summary>
        ///Retrieves ppidl of special folder
        /// </summary>
        /// <param name="hwndOwner">The HWND owner.</param>
        /// <param name="nFolder">The n folder.</param>
        /// <param name="ppidl">The ppidl.</param>
        /// <returns></returns>
        [DllImport("shell32.dll", EntryPoint = "SHGetSpecialFolderLocation", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern Int32 SHGetSpecialFolderLocation(
            IntPtr hwndOwner,
            int nFolder,
            out IntPtr ppidl);

        /// <summary>
        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        /// </summary>
        [DllImport("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageList(
            int iImageList,
            ref Guid riid,
            ref IImageList ppv
            );

        [DllImport("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageListHandle(
            int iImageList,
            ref Guid riid,
            ref IntPtr handle
            );


        [DllImport("Shell32.dll")]
        public static extern IntPtr ILCombine(IntPtr intPtr, IntPtr intPtr_2);

        [DllImport("Shell32.dll")]
        public static extern void ILFree(IntPtr intPtr);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ILCreateFromPath(String pszPath);

        [DllImport("shell32.dll", CharSet = CharSet.None)]
        public static extern uint ILGetSize(IntPtr pidl);

        [DllImport("comctl32.dll", SetLastError = true)]
        public static extern IntPtr ImageList_GetIcon(IntPtr imageListHandle, int iconIndex, int flags);

        /// <summary>
        /// The LoadLibrary function maps the specified executable module into the 
        /// address space of the calling process
        /// </summary>
        /// <param name="lpFileName">Pointer to a null-terminated string that names 
        /// the executable module (either a .dll or .exe file). The name specified 
        /// is the file name of the module and is not related to the name stored in 
        /// the library module itself, as specified by the LIBRARY keyword in the 
        /// module-definition (.def) file.  
        /// If the string specifies a path but the file does not exist in the specified 
        /// directory, the function fails. When specifying a path, be sure to use 
        /// backslashes (\), not forward slashes (/).  
        /// If the string does not specify a path, the function uses a standard search 
        /// strategy to find the file.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module. 
        /// If the function fails, the return value is NULL</returns>
        [DllImport("kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        /// <summary>
        /// The LoadLibraryEx function maps the specified executable module into the 
        /// address space of the calling process. The executable module can be a .dll 
        /// or an .exe file. The specified module may cause other modules to be mapped 
        /// into the address space
        /// </summary>
        /// <param name="lpfFileName">Pointer to a null-terminated string that names 
        /// the executable module (either a .dll or an .exe file). The name specified 
        /// is the file name of the executable module. This name is not related to the 
        /// name stored in a library module itself, as specified by the LIBRARY keyword 
        /// in the module-definition (.def) file. If the string specifies a path, but 
        /// the file does not exist in the specified directory, the function fails. When 
        /// specifying a path, be sure to use backslashes (\), not forward slashes (/). 
        /// If the string does not specify a path, and the file name extension is omitted, 
        /// the function appends the default library extension .dll to the file name. 
        /// However, the file name string can include a trailing point character (.) to 
        /// indicate that the module name has no extension. If the string does not specify 
        /// a path, the function uses a standard search strategy to find the file. If 
        /// mapping the specified module into the address space causes the system to map 
        /// in other, associated executable modules, the function can use either the 
        /// standard search strategy or an alternate search strategy to find those modules.</param>
        /// <param name="flags">Action to take when loading the module. If no flags are 
        /// specified, the behavior of this function is identical to that of the LoadLibrary 
        /// function. This parameter can be one of the LoadLibraryExFlags values</param>
        /// <returns>If the function succeeds, the return value is a handle to the mapped 
        /// executable module. If the function fails, the return value is NULL.</returns>
        public static IntPtr LoadLibraryEx(string lpfFileName, LoadLibraryExFlags flags)
        {
            return NativeMethods.InternalLoadLibraryEx(lpfFileName, IntPtr.Zero, (int)flags);
        }

        [DllImport("Kernel32.dll", EntryPoint = "LoadLibraryEx")]
        private static extern IntPtr InternalLoadLibraryEx(string lpfFileName, IntPtr hFile, int dwFlags);

        /// <summary>
        /// The FreeLibrary function decrements the reference count of the loaded 
        /// dynamic-link library (DLL). When the reference count reaches zero, the 
        /// module is unmapped from the address space of the calling process and the 
        /// handle is no longer valid
        /// </summary>
        /// <param name="hModule">Handle to the loaded DLL module. The LoadLibrary 
        /// function returns this handle</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the 
        /// function fails, the return value is zero</returns>
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// The FindResource function determines the location of a resource with the 
        /// specified type and name in the specified module
        /// </summary>
        /// <param name="hModule">Handle to the module whose executable file contains 
        /// the resource. A value of NULL specifies the module handle associated with 
        /// the image file that the operating system used to create the current process</param>
        /// <param name="lpName">Specifies the name of the resource</param>
        /// <param name="lpType">Specifies the resource type</param>
        /// <returns>If the function succeeds, the return value is a handle to the 
        /// specified resource's information block. To obtain a handle to the resource, 
        /// pass this handle to the LoadResource function. If the function fails, the 
        /// return value is NULL</returns>
        [DllImport("Kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, ResourceTypes lpType);

        [DllImport("Kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, ResourceTypes lpType);

        /// <summary>
        /// The FindResource function determines the location of a resource with the 
        /// specified type and name in the specified module
        /// </summary>
        /// <param name="hModule">Handle to the module whose executable file contains 
        /// the resource. A value of NULL specifies the module handle associated with 
        /// the image file that the operating system used to create the current process</param>
        /// <param name="lpName">Specifies the name of the resource</param>
        /// <param name="lpType">Specifies the resource type</param>
        /// <returns>If the function succeeds, the return value is a handle to the 
        /// specified resource's information block. To obtain a handle to the resource, 
        /// pass this handle to the LoadResource function. If the function fails, the 
        /// return value is NULL</returns>
        [DllImport("Kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

        /// <summary>
        /// The SizeofResource function returns the size, in bytes, of the specified 
        /// resource
        /// </summary>
        /// <param name="hModule">Handle to the module whose executable file contains 
        /// the resource</param>
        /// <param name="hResInfo">Handle to the resource. This handle must be created 
        /// by using the FindResource or FindResourceEx function</param>
        /// <returns>If the function succeeds, the return value is the number of bytes 
        /// in the resource. If the function fails, the return value is zero</returns>
        [DllImport("Kernel32.dll")]
        public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);


        /// <summary>
        /// The LoadResource function loads the specified resource into global memory
        /// </summary>
        /// <param name="hModule">Handle to the module whose executable file contains 
        /// the resource. If hModule is NULL, the system loads the resource from the 
        /// module that was used to create the current process</param>
        /// <param name="hResInfo">Handle to the resource to be loaded. This handle is 
        /// returned by the FindResource or FindResourceEx function</param>
        /// <returns>If the function succeeds, the return value is a handle to the data 
        /// associated with the resource. If the function fails, the return value is NULL</returns>
        [DllImport("Kernel32.dll")]
        public static extern System.IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        /// The FreeResource function decrements (decreases by one) the reference count 
        /// of a loaded resource. When the reference count reaches zero, the memory occupied 
        /// by the resource is freed
        /// </summary>
        /// <param name="hglbResource">Handle of the resource. It is assumed that hglbResource 
        /// was created by LoadResource</param>
        /// <returns>If the function succeeds, the return value is zero. If the function fails, 
        /// the return value is non-zero, which indicates that the resource has not been freed</returns>
        [DllImport("Kernel32.dll")]
        public static extern int FreeResource(IntPtr hglbResource);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, EnumResNameProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern int LookupIconIdFromDirectoryEx(IntPtr presbits, [MarshalAs(UnmanagedType.Bool)] bool fIcon, int cxDesired, int cyDesired, LoadIconFlags Flags);

        /// <summary>
        /// The CreateIconFromResourceEx function creates an icon or cursor from resource 
        /// bits describing the icon
        /// </summary>
        /// <param name="pbIconBits">Pointer to a buffer containing the icon or cursor 
        /// resource bits. These bits are typically loaded by calls to the 
        /// LookupIconIdFromDirectoryEx and LoadResource functions</param>
        /// <param name="cbIconBits">Specifies the size, in bytes, of the set of bits 
        /// pointed to by the pbIconBits parameter</param>
        /// <param name="fIcon">Specifies whether an icon or a cursor is to be created. 
        /// If this parameter is TRUE, an icon is to be created. If it is FALSE, a cursor 
        /// is to be created</param>
        /// <param name="dwVersion">Specifies the version number of the icon or cursor 
        /// format for the resource bits pointed to by the pbIconBits parameter. This 
        /// parameter can be 0x00030000</param>
        /// <param name="csDesired">Specifies the desired width, in pixels, of the icon 
        /// or cursor. If this parameter is zero, the function uses the SM_CXICON or 
        /// SM_CXCURSOR system metric value to set the width</param>
        /// <param name="cyDesired">Specifies the desired height, in pixels, of the icon 
        /// or cursor. If this parameter is zero, the function uses the SM_CYICON or 
        /// SM_CYCURSOR system metric value to set the height</param>
        /// <param name="flags"></param>
        /// <returns>If the function succeeds, the return value is a handle to the icon 
        /// or cursor. If the function fails, the return value is NULL</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateIconFromResourceEx(IntPtr pbIconBits, int cbIconBits, [MarshalAs(UnmanagedType.Bool)] bool fIcon, int dwVersion, int csDesired, int cyDesired, LoadIconFlags flags);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyWidth, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, IconDrawingFlags diFlags);

        [DllImport("gdi32.dll", EntryPoint = "CreateFontIndirect", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFontIndirect([In, Out, MarshalAs(UnmanagedType.AsAny)] object lf);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int DoDragDrop(System.Runtime.InteropServices.ComTypes.IDataObject dataObject, IOleDropSource dropSource, int allowedEffects, int[] finalEffect);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileStringW([MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpKeyName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpString,
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        public static void WriteString(string fileName, string section, string ident, string value)
        {
            WritePrivateProfileStringW(section, ident, value, fileName);
        }

        public static void WriteInterger(string fileName, string section, string ident, int value)
        {
            WritePrivateProfileStringW(section, ident, value.ToString(CultureInfo.InvariantCulture), fileName);
        }

        public static string ReadString(string fileName, string section, string ident, string defaultValue)
        {
            string result;
            StringBuilder sb = new StringBuilder(255);
            NativeMethods.GetPrivateProfileString(section, ident, defaultValue, sb, 255, fileName);
            result = sb.ToString().Trim('\0');
            return result;
        }

        /// <summary>
        /// The BeginDeferWindowPos function allocates memory for a multiple-window- position structure and 
        /// returns the handle to the structure
        /// </summary>
        /// <param name="nNumWindows">Specifies the initial number of windows for which to store position information. 
        /// The DeferWindowPos function increases the size of the structure, if necessary.</param>
        /// <returns>If the function succeeds, the return value identifies the multiple-window-position structure</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        /// <summary>
        /// The DeferWindowPos function updates the specified multiple-window – position structure for the 
        /// specified window. The function then returns a handle to the updated structure. 
        /// The EndDeferWindowPos function uses the information in this structure to change the position and 
        /// size of a number of windows simultaneously. The BeginDeferWindowPos function creates the structure. 
        /// </summary>
        /// <param name="hWinPosInfo">Handle to a multiple-window – position structure that contains size and 
        /// position information for one or more windows. This structure is returned by BeginDeferWindowPos or 
        /// by the most recent call to DeferWindowPos.</param>
        /// <param name="hWnd">Handle to the window for which update information is stored in the structure. 
        /// All windows in a multiple-window – position structure must have the same parent. </param>
        /// <param name="hWndInsertAfter">Handle to the window that precedes the positioned window in the Z order.</param>
        /// <param name="X">Specifies the x-coordinate of the window's upper-left corner.</param>
        /// <param name="Y">Specifies the y-coordinate of the window's upper-left corner.</param>
        /// <param name="cx">Specifies the window's new width, in pixels</param>
        /// <param name="cy">the window's new height, in pixels</param>
        /// <param name="uFlags">Specifies a combination of the following values that affect the size and position of the window</param>
        /// <returns>The return value identifies the updated multiple-window – position structure. </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr DeferWindowPos(
            IntPtr hWinPosInfo,
            IntPtr hWnd,               // window handle
            IntPtr hWndInsertAfter,    // placement-order handle
            int X,                     // horizontal position
            int Y,                     // vertical position
            int cx,                    // width
            int cy,                    // height
            int uFlags);               // window positioning flags

        /// <summary>
        /// The EndDeferWindowPos function simultaneously updates the position and size of one or more 
        /// windows in a single screen-refreshing cycle. 
        /// </summary>
        /// <param name="hWinPosInfo">Handle to a multiple-window – position structure that contains size and 
        /// position information for one or more windows. This internal structure is returned by the 
        /// BeginDeferWindowPos function or by the most recent call to the DeferWindowPos function.</param>
        /// <returns>If the function succeeds, the return value is nonzero. 
        /// If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int GdipBeginContainer2(IntPtr nativeGraphics, out int state);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int GdipEndContainer(IntPtr nativeGraphics, int state);

        public static int WM_MOUSEENTER
        {
            get
            {
                if (wmMouseEnterMessage == -1)
                {
                    wmMouseEnterMessage = RegisterWindowMessage("LaugrisMouseEnter");
                }
                return wmMouseEnterMessage;
            }
        }

        public static int WM_UIUNSUBCLASS
        {
            get
            {
                if (wmUnSubclass == -1)
                {
                    wmUnSubclass = RegisterWindowMessage("LaugrisUnSubclass");
                }
                return wmUnSubclass;
            }
        }


        [DllImport("mongoose.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr kr_start(string document_root, string server_port);

        [DllImport("mongoose.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mg_stop(IntPtr ctx);

        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

        public static string GetFolderPath(CSIDL folder)
        {
            StringBuilder lpszPath = new StringBuilder(260);
            NativeMethods.SHGetFolderPath(IntPtr.Zero, (int)folder, IntPtr.Zero, 0, lpszPath);
            string path = lpszPath.ToString();
            return path;
        }


    }
}
