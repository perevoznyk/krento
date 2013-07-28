//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// Popup menu items collection
    /// </summary>
    public class KrentoMenuItems : Collection<KrentoMenuItem>
    {
        /// <summary>
        /// Disposes the menu items.
        /// </summary>
        public void DisposeItems()
        {
            for (int i = 0; i < Count; i++)
            {
                Items[i].Dispose();
            }
        }
    }
}
