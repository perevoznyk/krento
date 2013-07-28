using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    public static class VirtualMouse
    {
        ///<summary>
        /// simulates movement of the mouse.  parameters specify changes
        /// in relative position.  positive values indicate movement
        /// right or down
        ///</summary>
        public static void Move(int xDelta, int yDelta)
        {
            NativeMethods.MouseMove(xDelta, yDelta);
        }


        ///<summary>
        /// simulates movement of the mouse.  parameters specify an
        /// absolute location, with the top left corner being the
        /// origin
        ///</summary>
       public static void MoveTo(int x, int y)
        {
            NativeMethods.MouseMoveTo(x, y);
        }


        ///<summary>
        /// simulates a click-and-release action of the left mouse
        /// button at its current position
        ///</summary>
        public static void LeftClick()
        {
            NativeMethods.MouseLeftClick();
        }


        public static void RightClick()
        {
            NativeMethods.MouseRightClick();
        }

    }
}
