using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Laugris.Sage
{
    /// <summary>
    /// Represents a display device of the primary screen 
    /// </summary>
    public static class PrimaryScreen
    {
        static PrimaryScreen()
        {
            NativeMethods.GetPrimaryMonitor();
        }

        public static bool IsUserPlayingFullscreen()
        {
            return NativeMethods.IsUserPlayingFullscreen();
        }

        public static Rectangle WholeScreen(Point pt)
        {
            return Screen.GetBounds(pt);
        }

        /// <summary>
        /// Gets the center of the screen.
        /// </summary>
        /// <value>The center of the primary screen.</value>
        public static Point Center
        {
            get
            {
                Rectangle bounds = Bounds;
                return new Point(Bounds.Left + bounds.Width / 2, Bounds.Top + bounds.Height / 2);
            }
        }

        public static Point CursorPosition
        {
            get
            {
                POINT pt = new POINT();
                NativeMethods.GetCursorPos(ref pt);
                return new Point(pt.x, pt.y);
            }
        }

        public static IntPtr WindowFromMouse
        {
            get
            {
                POINT pt = new POINT();
                NativeMethods.GetCursorPos(ref pt);
                return NativeMethods.WindowFromPoint(pt);
            }
        }

        /// <summary>
        /// Gets the working area of the primary screen.
        /// </summary>
        /// <value>The working area.</value>
        public static Rectangle WorkingArea
        {
            get
            {
                RECT rect = new RECT();
                NativeMethods.GetPrimaryMonitorArea(ref rect);
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
        }

        /// <summary>
        /// Gets the full size of the screen including all monitors.
        /// </summary>
        /// <value>
        /// The full size of the screen.
        /// </value>
        public static Rectangle FullScreenSize
        {
            get
            {
                RECT rect = new RECT();
                NativeMethods.GetFullScreenSize(ref rect);
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
        }

        /// <summary>
        /// Gets the bounds of the primary screen.
        /// </summary>
        /// <value>The bounds.</value>
        public static Rectangle Bounds
        {
            get
            {
                RECT rect = new RECT();
                NativeMethods.GetPrimaryMonitorBounds(ref rect);
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
        }
    }
}
