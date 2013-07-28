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

    public abstract class CustomDockManager : IDockManager, IDisposable
    {
        private DockSettings settings;
        private int width;
        private int height;
        private bool useAlpha = true;
        private bool useDenomination;
        private List<DockItem> items = new List<DockItem>();
        private CustomDockPainter painter;
        [AccessedThroughProperty("ImageList")]
        private ImageList imageList;
        private ArrayList Selected = new ArrayList();
        private ZOrderComparer comparer = new ZOrderComparer();
        public event EventHandler AlphaChanged;

        ~CustomDockManager()
        {
            Dispose(false);
        }

        protected CustomDockManager(DockSettings settings, CustomDockPainter painter)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            if (painter == null)
                throw new ArgumentNullException("painter");

            items = new List<DockItem>();
            this.painter = painter;
            this.settings = settings;
        }

        protected ZOrderComparer Comparer
        {
            get { return comparer; }
        }

        public List<DockItem> Items
        {
            get { return items; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public DockSettings Settings
        {
            get { return settings; }
        }

        public CustomDockPainter Painter
        {
            get { return painter; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use denomination].
        /// Specify denomination usage before adding any items, otherwise
        /// all information about existing items will be lost.
        /// </summary>
        /// <value><c>true</c> if [use denomination]; otherwise, <c>false</c>.</value>
        public bool UseDenomination
        {
            get { return useDenomination; }
            set
            {
                if (useDenomination != value)
                {
                    useDenomination = value;
                    items.Clear();
                }
            }
        }

        public ImageList ImageList
        {
            get { return imageList; }
            set
            {
                imageList = value;
                foreach (DockItem item in items)
                {
                    item.ImageList = value;
                }
            }
        }

        #region Abstract methods

        public abstract void UpdateDockInfo();

        /// <summary>
        /// This method allows derived classes to handle the event without attaching a delegate
        /// </summary>
        /// <param name="CurX">The current horizontal coordinate of the mouse cursor</param>
        /// <param name="CurY">The current vertical coordinate of the mouse cursor</param>
        public abstract void DoMouseMove(int CurX, int CurY);

        /// <summary>
        /// This method allows derived classes to handle the event without attaching a delegate
        /// </summary>
        public abstract void DoMouseLeave();

        /// <summary>
        /// This method allows derived classes to handle the event without attaching a delegate
        /// </summary>
        public abstract void DoSizeChanged();

        #endregion

        /// <summary>
        /// Raises the <see cref="E:AlphaChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnAlphaChanged(EventArgs e)
        {
            if (AlphaChanged != null)
            {
                AlphaChanged(this, e);
            }
        }

        /// <summary>
        /// This method allows derived classes to handle the event without attaching a delegate
        /// </summary>
        protected virtual void DoAlphaChange()
        {
            OnAlphaChanged(EventArgs.Empty);
        }


        /// <summary>
        /// Loads icon for the dock item from the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public virtual Bitmap IconFromFileName(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                Bitmap result = (Bitmap)Bitmap.FromFile(fileName);
                return result;
            }
            else
                return null;
        }

        public virtual Bitmap IconFromImageList(int index)
        {
            if (imageList != null)
            {
                Bitmap result = (Bitmap)imageList.Images[index];
                return result;
            }
            else
                return null;
        }

        [DefaultValue(true)]
        public bool UseAlpha
        {
            get { return useAlpha; }
            set
            {
                if (useAlpha != value)
                {
                    useAlpha = value;
                    DoAlphaChange();
                }
            }
        }

        /// <summary>
        /// Gets the left margin of the icon.
        /// </summary>
        /// <returns></returns>
        protected int GetLeftMargin()
        {
            return settings.LeftMargin;
        }

        /// <summary>
        /// Gets the top margin of the icon.
        /// </summary>
        /// <returns></returns>
        protected int GetTopMargin()
        {
            return settings.TopMargin;
        }

        /// <summary>
        /// Gets the size of the icon.
        /// </summary>
        /// <returns></returns>
        protected int GetIconSize()
        {
            return settings.IconSize;
        }

        protected int GetSelectionThreshold()
        {
            return settings.SelectionThreshold;
        }


        /// <summary>
        /// Gets the icons spacing.
        /// </summary>
        /// <returns></returns>
        protected int GetIconsSpacing()
        {
            return settings.IconsSpacing;
        }

        /// <summary>
        /// Gets the reflection depth.
        /// </summary>
        /// <returns></returns>
        protected int GetReflectionDepth()
        {
            return settings.ReflectionDepth;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Detects if the point is located within the area of the icon
        /// </summary>
        /// <param name="item">The dock item.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>True, if the point with coordinates <paramref name="x"/> and <paramref name="y"/>
        /// lays within the item rectangle and false if not</returns>
        public bool PointInItem(DockItem item, int x, int y)
        {
            if (item == null)
                return false;
            int threshold = GetSelectionThreshold();

            Rectangle rect = new Rectangle(item.X + threshold, item.Y + threshold,
                item.PaintWidth - threshold, item.PaintHeight - threshold);
            return rect.Contains(x, y);
        }

        /// <summary>
        /// Determine if the points with coordinates <paramref name="x"/> and <paramref name="y"/> lays
        /// within the item icon rectangle
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns><see cref="DockItem"/> if the point is within the one of the dock items rectangle or null if not</returns>
        public DockItem GetItemAt(int x, int y)
        {
            Selected.Clear();

            for (int cnt = 0; cnt < items.Count; cnt++)
            {
                if (PointInItem(items[cnt], x, y))
                {
                    Selected.Add(items[cnt]);
                }
            }
            if (Selected.Count == 0)
                return null;
            else
            {
                return (DockItem)Selected[Selected.Count - 1];
            }

        }

        /// <summary>
        /// Use this method to define if the caption of the dock item will be painted
        /// </summary>
        /// <param name="item">The Dock Item.</param>
        /// <returns></returns>
        public virtual bool CaptionVisible(DockItem item)
        {
            return false;
        }


        public virtual bool GetScaleCaption()
        {
            return this.Settings.ScaleCaption;
        }

        public virtual void Paint(Graphics canvas)
        {
            painter.Paint(canvas, this);
        }


        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                        items[i].Dispose();
                }
                items.Clear();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

}
