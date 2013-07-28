using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Laugris.Sage
{
    /// <summary>
    /// Helper class for accessing native Windows API and resources
    /// Not recommeded to use it directly without deep understanding of Windows API
    /// </summary>
    public static class InteropHelper
    {

        public static long AlignToPage(long size)
        {
            return ((size + 0x1fffL) & -8192L);
        }

        public static void ShowDesktop()
        {
            NativeMethods.ShowDesktop();
        }

        /// <summary>
        /// Determines if the application is running on Windows 7
        /// </summary>
        public static bool RunningOnWin7
        {
            get
            {
                return NativeMethods.IsWindows7();
            }
        }

        public static bool RunningOnWin8
        {
            get
            {
                return NativeMethods.IsWindows8();
            }
        }

        public static bool MetroActive
        {
            get
            {
                return NativeMethods.IsMetroActive();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [running on vista].
        /// </summary>
        /// <value><c>true</c> if [running on vista]; otherwise, <c>false</c>.</value>
        public static bool RunningOnVista
        {
            get
            {
                return NativeMethods.IsWindowsVista();
            }
        }

        public static bool IsTrueColorMonitor
        {
            get { return NativeMethods.IsTrueColorMonitor(); }
        }

        public static bool IsAdministrator
        {
            get { return NativeMethods.IsFullAdministrator(); }
        }

        /// <summary>
        /// Gets a value indicating whether [running on XP].
        /// </summary>
        /// <value><c>true</c> if [running on XP]; otherwise, <c>false</c>.</value>
        public static bool RunningOnXP
        {
            get
            {
                return NativeMethods.IsWindowsXP();
            }
        }

        public static void ClearUnusedMemory()
        {
            NativeMethods.ClearUnusedMemory();
        }

        public static bool IsMultiTouchReady
        {
            get { return NativeMethods.IsMultiTouchReady(); }
        }

        public static bool IsTabletPC
        {
            get { return NativeMethods.IsTabletPC(); }
        }

        public static bool IsMediaCenter
        {
            get { return NativeMethods.IsMediaCenter(); }
        }

        public static bool IsConnectedToInternet
        {
            get { return NativeMethods.IsConnectedToInternet(); }
        }

        /// <summary>
        /// Loads the native (unmanaged) library.
        /// </summary>
        /// <param name="dllName">Name of the DLL.</param>
        /// <returns></returns>
        public static IntPtr LoadNativeLibrary(string dllName)
        {
            return NativeMethods.LoadLibrary(dllName);
        }

        public static string CurrentIPAddress
        {
            get
            {
                StringBuilder sb = new StringBuilder(32);
                NativeMethods.CurrentIPAddress(sb, 31);
                return sb.ToString();
            }
        }
        /// <summary>
        /// Controls whether the shield 
        /// icon is displayed for a button. This function has effect only for Vista
        /// </summary>
        /// <param name="button">The Windows Forms button to set.</param>
        /// <param name="showShield">A value that indicates whether the shield icon is displayed.</param>
        public static void SetWindowsFormsButtonShield(
          System.Windows.Forms.Button button,
          bool showShield)
        {
            if (RunningOnVista)
            {
                IntPtr fRequired = new IntPtr(showShield ? 1 : 0);
                NativeMethods.SendMessage(
                    new HandleRef(null, button.Handle),
                    NativeMethods.BCM_SETSHIELD,
                    IntPtr.Zero,
                    fRequired);
            }
        }


        /// <summary>
        /// Add/Remove registry entries for windows startup. Only for current user
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        public static void SetStartup(string appName, bool enable)
        {
            if (enable)
            {
                NativeMethods.SetStartup(appName, Application.ExecutablePath);
            }
            else
            {
                // remove startup
                NativeMethods.RemoveStartup(appName);
            }
        }

        public static bool GetStartup(string appName)
        {
            return NativeMethods.GetStartup(appName);
        }

        public static void TurnMonitorOn()
        {
            NativeMethods.TurnMonitorOn();
        }

        public static void TurnMonitorOff()
        {
            NativeMethods.TurnMonitorOff();
        }

        public static IntPtr MainWindow
        {
            get { return NativeMethods.GetMainWindow(0); }
            set { NativeMethods.SetMainWindow(value, 0); }
        }

        public static void ExitKrento()
        {
            NativeMethods.ExitKrento();
            Application.ExitThread();
        }

        public static int TaskbarPosition
        {
            get { return NativeMethods.GetTaskbarPosition(); }
        }

        public static void BroadcastApplicationMessage(int msg)
        {
            NativeMethods.BroadcastApplicationMessage(msg);
        }

        public static void OutputDebugString(string msg)
        {
            NativeMethods.OutputDebugString(msg);
        }

        public static Rectangle DockRectangle
        {
            get
            {
                RECT rect = new RECT();
                NativeMethods.GetDockRectangle(ref rect);
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
            set
            {
                RECT rect = new RECT();
                rect.left = value.Left;
                rect.top = value.Top;
                rect.right = value.Right;
                rect.bottom = value.Bottom;
                NativeMethods.SetDockRectangle(ref rect);
            }
        }
    }
}
