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
    /// <summary>
    /// Event arguments for Krento popup menu
    /// </summary>
    public class KrentoMenuArgs : EmptyEventArgs
    {
        private  KrentoMenuItem menuItem;
        private readonly int itemIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="KrentoMenuArgs"/> class.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <param name="itemIndex">Index of the item.</param>
        public KrentoMenuArgs(KrentoMenuItem menuItem, int itemIndex)
        {
            this.menuItem = menuItem;
            this.itemIndex = itemIndex;
        }

        /// <summary>
        /// Gets the menu item.
        /// </summary>
        /// <value>The menu item.</value>
        public KrentoMenuItem MenuItem
        {
            get { return menuItem; }
        }

        protected override void Dispose(bool disposing)
        {
            this.menuItem = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        /// <value>The index of the item.</value>
        public int ItemIndex
        {
            get { return itemIndex; }
        } 

    }
}
