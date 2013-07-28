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

namespace Laugris.Sage
{
    public sealed class ZOrderComparer : IComparer<DockItem>
    {
        public int Compare(DockItem x, DockItem y)
        {
            if (x == null)
                return 0;
            if (y == null)
                return 0;


            return x.Order - y.Order;
        }
    }
}
