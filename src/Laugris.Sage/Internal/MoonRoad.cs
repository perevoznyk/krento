using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace Laugris.Sage
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
    internal delegate void FolderEnumProc([MarshalAs(UnmanagedType.LPWStr)] string fileName, IntPtr lParam);

    [SuppressUnmanagedCodeSecurity]
    internal static partial class NativeMethods
    {
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartEngine(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string applicationName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void DrawLayeredWindow(IntPtr handle, int left, int top, int width, int height, IntPtr buffer, int colorKey, byte alpha, [MarshalAs(UnmanagedType.Bool)] bool redrawOnly);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void SetStartup([MarshalAs(UnmanagedType.LPWStr)] string appName,
                                             [MarshalAs(UnmanagedType.LPWStr)] string appPath);


        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void RemoveStartup([MarshalAs(UnmanagedType.LPWStr)] string appName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetStartup([MarshalAs(UnmanagedType.LPWStr)] string appName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Matches([MarshalAs(UnmanagedType.LPWStr)] string itemText, [MarshalAs(UnmanagedType.LPWStr)] string searchText);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void GetFileDescription(StringBuilder fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetApplicationFromWindow(IntPtr window, StringBuilder appName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void CurrentIPAddress(StringBuilder address, int len);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void CreateShellLink(IntPtr PathObj, [MarshalAs(UnmanagedType.LPWStr)] string lpszPathLink);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ResolveShellLink([MarshalAs(UnmanagedType.LPWStr)] string lpszLinkFile, StringBuilder lpszPath, int iPathBufferSize);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetFileNameFromPidl(IntPtr pidlRelative, StringBuilder lpszPath, int iPathBufferSize);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PortAvailable(int port);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr LayeredWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GetFilesCount([MarshalAs(UnmanagedType.LPWStr)] string folderName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GetAllFiles(FolderEnumProc callback ,[MarshalAs(UnmanagedType.LPWStr)] string folderName, [MarshalAs(UnmanagedType.LPWStr)] string searchMask, IntPtr lParam);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void RemoveSystemMenu(IntPtr handle);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileExists([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIsExe([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIsIcon([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsValidFileName([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsValidPathName([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsAssembly([MarshalAs(UnmanagedType.LPWStr)] string FileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsAnimatedGIF([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileOrFolderExists([MarshalAs(UnmanagedType.LPWStr)] string name);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DirectoryExists([MarshalAs(UnmanagedType.LPWStr)] string name);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsDirectory([MarshalAs(UnmanagedType.LPWStr)] string name);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool AddRemoveMessageFilter(int msg, ChangeWindowMessageFilterFlags flags);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawTextDirect(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int color, int left, int top);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawTextDirectEx(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int color, int background, int left, int top);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void GetFullScreenSize(ref RECT rect);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsMultiTouchReady();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindows8();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsMetroActive();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsNativeWin64();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindows7();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowsVista();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowsXP();

        [DllImport("MoonRoad.dll", EntryPoint = "IsWindowsXPSP2", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowsXPServicePack2();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTabletPC();

        [DllImport("Moonroad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsMediaCenter();

        [DllImport("Moonroad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsUserPlayingFullscreen();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsConnectedToInternet();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void CreateUnicodeFile([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ClearFileAttributes([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsUnicodeFile([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FileCreateRewrite([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void FileClose(IntPtr handle);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void FileWriteNewLine(IntPtr handle);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void FileWrite(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string text);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void FileWriteChar(IntPtr handle, char text);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileRename([MarshalAs(UnmanagedType.LPWStr)] string oldName, [MarshalAs(UnmanagedType.LPWStr)] string newName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsGoodWindow(IntPtr hwnd);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShutdownWindows(int flags);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SuspendWindows();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool HibernateWindows();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr AllocateDefaultHWND();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr AllocateLayeredWindow([MarshalAs(UnmanagedType.LPWStr)] string className);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr AllocateHWND(WndProcDelegate method);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeallocateHWND(IntPtr hwnd);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr AllocateLaugrisWindow();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr AllocateKarnaWindow();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void RestoreWindowSubclass(IntPtr hwnd);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetAppicationIcon(IntPtr wnd);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWorkingArea(ref RECT area);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void UpdateWindowPosition(IntPtr handle, int x, int y);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ShellCopyFile([MarshalAs(UnmanagedType.LPWStr)] string oldName, [MarshalAs(UnmanagedType.LPWStr)] string newName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void BringWindowToFront(IntPtr handle);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void LockForegroundWindow();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void UnlockForegroundWindow();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void TurnMonitorOn();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void TurnMonitorOff();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CursorInsideWindow(IntPtr hwnd);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetWindowLongPointer(IntPtr hWnd, WindowLong nIndex);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowLongPointer(IntPtr hWnd, WindowLong nIndex, WndProcDelegate value);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowLongPointer(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetMainWindow(IntPtr window, byte applicationNumber);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetMainWindow(byte applicationNumber);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ClearUnusedMemory();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DeleteToRecycleBin([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.Bool)] bool silent);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void NotifyOfChange();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ShowDesktop();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MouseMove(int xDelta, int yDelta);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MouseMoveTo(int x, int y);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MouseLeftClick();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MouseRightClick();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FileReadToBuffer([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void FreeFileBuffer(IntPtr buffer);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int FileGetSize([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ExitKrento();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int EmToPixels(int em);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern SIZE GetTextSize([MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int flags);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern SIZE GetTextSizeEx([MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int proposedWidth, int flags, bool margins);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawAlphaText(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int color, int width, int height, int flags);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawAlphaTextRect(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int color, ref RECT lpRect, int flags);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateWindowsFont([MarshalAs(UnmanagedType.LPWStr)] string fontFamily, int size, int fontStyle, int fontQuality);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetParentProcessId();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern SIZE GetTextLineSize([MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawTextLine(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int color, int left, int top);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawTextOutline(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int foreColor, int backColor, int left, int top);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawTextGlow(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int foreColor, int left, int top);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawTextRect(IntPtr hDC, [MarshalAs(UnmanagedType.LPWStr)] string s, IntPtr hFont, int foreColor, int backColor, ref RECT lpRect, int flags);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CloneFont(IntPtr hFont);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTrueColorMonitor();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsFullAdministrator();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileExtensionIs([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.LPWStr)] string extension);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIsImage([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIsKrentoPackage([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void FileDelete([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void StripFileName([MarshalAs(UnmanagedType.LPWStr)] string fileName, StringBuilder sb);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FullPath([MarshalAs(UnmanagedType.LPWStr)] string fileName, StringBuilder sb);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileCopy([MarshalAs(UnmanagedType.LPWStr)] string oldName, [MarshalAs(UnmanagedType.LPWStr)] string newName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIsLink([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ReportSystemEvent(IntPtr hwnd, int eventId);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DestroyFont(IntPtr hFont);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr MakeCompatibleBitmap(IntPtr source, int width, int height);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void DrawNativeBitmap(IntPtr src, IntPtr dst, int width, int height);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StretchNativeBitmap(IntPtr src, IntPtr dst, int srcWidth, int srcHeight, int dstWidth, int dstHeight);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void AlphaBlendBitmap(IntPtr src, IntPtr hdc, int left, int top, int width, int height, int alpha);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetTaskbarPosition();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int DpiY();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void AlphaBlendNative(IntPtr hdcDest,
                             int xoriginDest,
                             int yoriginDest,
                             int wDest,
                             int hDest,
                             IntPtr hdcSrc,
                             int xoriginSrc,
                             int yoriginSrc,
                             int wSrc,
                             int hSrc,
                             byte alpha);

        /// <summary>
        /// Installs the keyboard hook.
        /// </summary>
        /// <param name="reportWindow">The report window.</param>
        /// <param name="modifiers">The key modifiers.</param>
        /// <param name="key">The key.</param>
        /// <returns>Hook id</returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int InstallKeyboardHook(IntPtr reportWindow, int modifiers, Keys key);

        /// <summary>
        /// Removes the keyboard hook.
        /// </summary>
        /// <param name="reportWindow">The report window.</param>
        /// <param name="id">The hoook id.</param>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void RemoveKeyboardHook(IntPtr reportWindow, int id);

        /// <summary>
        /// Installs the mouse hook.
        /// </summary>
        /// <param name="reportWindow">The report window.</param>
        /// <returns></returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InstallMouseHook(IntPtr reportWindow);

        /// <summary>
        /// Removes the mouse hook.
        /// </summary>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void RemoveMouseHook();


        /// <summary>
        /// Pauses the mouse hook.
        /// </summary>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void PauseMouseHook();

        /// <summary>
        /// Resumes the mouse hook.
        /// </summary>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ResumeMouseHook();


        /// <summary>
        /// Pauses the mouse hook interceptor.
        /// </summary>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void PauseIntercept();

        /// <summary>
        /// Resumes the mouse hook interceptor.
        /// </summary>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ResumeIntercept();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDesktopClick();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetDesktopClick([MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Gets the move tracking.
        /// </summary>
        /// <returns></returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMoveTracking();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetHookPaused();

        /// <summary>
        /// Gets the wheel tracking.
        /// </summary>
        /// <returns></returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWheelTracking();

        /// <summary>
        /// Gets the click tracking.
        /// </summary>
        /// <returns></returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClickTracking();

        /// <summary>
        /// Sets the wheel tracking.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetWheelTracking([MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Sets the move tracking.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetMoveTracking([MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Sets the click tracking wheel.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetClickTrackingWheel([MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Sets the click tracking X button.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetClickTrackingXButton([MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Gets the click tracking wheel.
        /// </summary>
        /// <returns></returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClickTrackingWheel();

        /// <summary>
        /// Gets the click tracking X button.
        /// </summary>
        /// <returns></returns>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClickTrackingXButton();

        /// <summary>
        /// Sets the click tracking.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetClickTracking([MarshalAs(UnmanagedType.Bool)] bool value);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetInterceptClick([MarshalAs(UnmanagedType.Bool)] bool value);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetInterceptClick();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetMouseDetected([MarshalAs(UnmanagedType.Bool)] bool value);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMouseDetected();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void GetDockRectangle(ref RECT rect);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetDockRectangle(ref RECT rect);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetPrimaryMonitor();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void GetPrimaryMonitorBounds(ref RECT rect);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void GetPrimaryMonitorArea(ref RECT rect);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMainMessage(byte applicationNumber, WindowMessage msg, IntPtr wparam, IntPtr lparam);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMainMessage(byte applicationNumber, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SendMainMessage(byte applicationNumber, WindowMessage msg, IntPtr wparam, IntPtr lparam);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SendMainMessage(byte applicationNumber, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MakeSound(string soundName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MakeSoundFromFile(string soundFileName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void MakeSoundFromResource(IntPtr handle, string soundName);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RecycleBinEmpty();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void EmptyRecycleBin();

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTopMostWindow(IntPtr handle);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetTopMostWindow(IntPtr handle, bool value);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetBottomMostWindow(IntPtr handle, bool value);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void WindowInsertAfter(IntPtr hwnd, IntPtr hwndInsertAfter);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadBitmapPNG([MarshalAs(UnmanagedType.LPWStr)] string fileName, ref int width, ref int height, out IntPtr ppvBits);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadBitmapJPG([MarshalAs(UnmanagedType.LPWStr)] string fileName, ref int width, ref int height, out IntPtr ppvBits);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadPNGResource(IntPtr handle, string szName, ref int width, ref int height, out IntPtr ppvBits);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SameText([MarshalAs(UnmanagedType.LPWStr)] string firstSting, [MarshalAs(UnmanagedType.LPWStr)] string secondString);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void CopyNativeBitmap(IntPtr src, IntPtr dstDC, int width, int height, int left, int top);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateNativeBitmap(int width, int height, out IntPtr ppvBits);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void CreateBackup(string archiveName, string baseFolder);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void ExtractArchive(string archiveName, string destination);

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void BroadcastApplicationMessage(int msg);


        public static bool StartEngineEx(IntPtr handle)
        {
            try
            {
                return StartEngine(handle, GlobalConfig.ProductName);
            }
            catch (DllNotFoundException)
            {
                throw new HookException();
            }

        }

    }
}
