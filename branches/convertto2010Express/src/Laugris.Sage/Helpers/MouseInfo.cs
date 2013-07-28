//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Laugris.Sage
{
    public static class MouseInfo
    {
        public static MouseButtons MouseButtons
        {
            get
            {
                MouseButtons result = MouseButtons.None;
                if (NativeMethods.GetKeyState(1) < 0)
                {
                    result |= MouseButtons.Left;
                }
                if (NativeMethods.GetKeyState(2) < 0)
                {
                    result |= MouseButtons.Right;
                }
                if (NativeMethods.GetKeyState(4) < 0)
                {
                    result |= MouseButtons.Middle;
                }
                if (NativeMethods.GetKeyState(5) < 0)
                {
                    result |= MouseButtons.XButton1;
                }
                if (NativeMethods.GetKeyState(6) < 0)
                {
                    result |= MouseButtons.XButton2;
                }
                return result;
            }
        }

    }
}
