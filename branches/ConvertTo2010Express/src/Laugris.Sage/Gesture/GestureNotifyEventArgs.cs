using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Laugris.Sage
{
    class GestureNotifyEventArgs : EventArgs
    {
        // Methods
        internal GestureNotifyEventArgs(IntPtr lparam)
        {
            GESTURENOTIFYSTRUCT gesturenotifystruct = (GESTURENOTIFYSTRUCT)Marshal.PtrToStructure(lparam, typeof(GESTURENOTIFYSTRUCT));
            this.Location = new Point(gesturenotifystruct.ptsLocation.x, gesturenotifystruct.ptsLocation.y);
            this.TargetHwnd = gesturenotifystruct.hwndTarget;
        }

        // Properties
        public Point Location { get; private set; }

        public IntPtr TargetHwnd { get; private set; }
    }
}
