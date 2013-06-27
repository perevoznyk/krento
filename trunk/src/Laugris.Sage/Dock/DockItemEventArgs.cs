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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Laugris.Sage
{
    [Serializable]
    public class DockItemEventArgs : EventArgs
    {
        [NonSerializedAttribute]
        private readonly DockItem item;
        private readonly MouseButtons button;

        public DockItem Item
        {
            get { return this.item; }
        }

        public MouseButtons Button
        {
            get { return this.button; }
        }

        public DockItemEventArgs(DockItem item, MouseButtons button)
        {
            this.item = item;
            this.button = button;
        }
    }
}
