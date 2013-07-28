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
    public static class KeyboardInfo
    {
        public static Keys ModifierKeys
        {
            get
            {
                Keys result = Keys.None;
                if (NativeMethods.GetKeyState(0x10) < 0)
                {
                    result |= Keys.Shift;
                }
                if (NativeMethods.GetKeyState(0x11) < 0)
                {
                    result |= Keys.Control;
                }
                if (NativeMethods.GetKeyState(0x12) < 0)
                {
                    result |= Keys.Alt;
                }
                return result;
            }
        }

    }
}
