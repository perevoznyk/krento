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
using System.Drawing;

namespace Laugris.Sage
{

    /// <summary>
    /// Dock Item interface
    /// </summary>
    public interface IDockItem
    {
        Image Icon { get; set; }
        string Hint { get; set;}
        string Caption { get; set;}
    }

    public interface IDockPainter
    {
        void Paint(Graphics canvas, IDockManager manager);
    }

    public interface IDockManager
    {
        List<DockItem> Items { get;}
        bool CaptionVisible(DockItem item);
        bool GetScaleCaption();
        bool UseDenomination { get; set;}
        int Count { get;}
    }
}
