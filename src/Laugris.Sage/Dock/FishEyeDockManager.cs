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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Laugris.Sage
{

    public class FishEyeDockManager : CustomDockManager
    {

        [AccessedThroughProperty("ItemsLayout")]
        private ItemsLayout itemsLayout = ItemsLayout.Centered;
        [AccessedThroughProperty("DockOrientation")]
        private DockOrientation dockOrientation = DockOrientation.Horizontal;
        private int columns;

        public FishEyeDockManager(DockSettings settings, CustomDockPainter painter)
            : base(settings, painter)
        {
        }

        public override void UpdateDockInfo()
        {
            UpdateCoordinates(true);
        }

        [DefaultValue(ItemsLayout.Centered)]
        public ItemsLayout ItemsLayout
        {
            get { return itemsLayout; }
            set { itemsLayout = value; }
        }

        [DefaultValue(DockOrientation.Horizontal)]
        public DockOrientation DockOrientation
        {
            get { return dockOrientation; }
            set { dockOrientation = value; }
        }

        protected int GetRowFromIndex(int index)
        {
            return (index / columns);
        }

        protected int GetColumnFromIndex(int index)
        {
            return (index % columns);
        }

        public override void DoMouseLeave()
        {

        }

        /// <summary>
        /// Updates all items coordinates to initial state based on the dock settings
        /// </summary>
        /// <param name="resetOrder">Reset items order to initial state</param>
        public virtual void UpdateCoordinates(bool resetOrder)
        {
            int startX = GetLeftMargin();
            int startY = GetTopMargin();
            int defSize = GetIconSize();
            int spacing = GetIconsSpacing();
            int reflection = GetReflectionDepth();
            int idx = 0;
            int availWidth = 0;
            int availHeight = 0;
            int total = Items.Count;
            bool RCPresent;
            int currRow = 0;
            int currColumn = 0;

            //Information about rows and columns are present
            RCPresent = ((columns > 0) && (dockOrientation == DockOrientation.Horizontal));

            if ((itemsLayout == ItemsLayout.Centered) && (!RCPresent))
            {
                availWidth = Width / 2;
                availHeight = Height / 2;
            }


            foreach (DockItem item in Items)
            {
                item.X = startX;
                item.Y = startY;
                item.ReflectionDepth = reflection;
                item.Width = defSize;
                item.Height = defSize + reflection;
                item.Scale = 1.0f;
                if (UseAlpha)
                    item.Alpha = 100;
                else
                    item.Alpha = 0xFF;
                if (resetOrder)
                {
                    item.InitialIndex = idx;
                }

                item.ResetPaintSize();

                if (DockOrientation == DockOrientation.Horizontal)
                {
                    if (itemsLayout == ItemsLayout.Centered)
                    {
                        item.X = (int)(availWidth + (item.InitialIndex - (total - 1) / 2) * (spacing + defSize) - item.PaintWidth / 2);
                        item.Y = (int)(availHeight - item.PaintHeight / 2);
                    }
                    else
                    {
                        //Rows and Colums are only relevant when dock's orientation is horizontal and
                        //it's not centered
                        if (!RCPresent)
                        {
                            item.Y = startY;
                            item.X = startX + item.InitialIndex * (spacing + defSize);
                        }
                        else
                        {
                            currColumn = GetColumnFromIndex(item.InitialIndex);
                            currRow = GetRowFromIndex(item.InitialIndex);
                            item.X = startX + currColumn * (spacing + defSize);
                            item.Y = startY + currRow * (spacing + defSize);
                        }
                    }
                }
                else
                {
                    if (itemsLayout == ItemsLayout.Centered)
                    {
                        item.Y = (int)(availHeight + (item.InitialIndex - (total - 1) / 2) * (spacing + defSize) - item.PaintHeight / 2);
                        item.X = (int)(availWidth - item.PaintWidth / 2);
                    }
                    else
                    {
                        item.X = startX;
                        item.Y = startY + item.InitialIndex * (spacing + defSize);
                    }
                }

                idx++;
            }

        }

        /// <summary>
        /// Updates all items coordinates to initial state based on the dock settings
        /// </summary>
        public virtual void UpdateCoordinates()
        {
            UpdateCoordinates(true);
        }

        public override void DoSizeChanged()
        {
            UpdateCoordinates(true);
        }


        public int Columns
        {
            get { return columns; }
            set
            {
                columns = value;
                UpdateCoordinates(true);
            }
        }

        public void DecreaseScale(float delta)
        {
            int startX = GetLeftMargin();
            int startY = GetTopMargin();
            int defSize = GetIconSize();
            int spacing = GetIconsSpacing();
            int total = Items.Count;
            int currRow;
            int currColumn;
            bool RCPresent;
            int availWidth = 0;
            int availHeight = 0;

            if (itemsLayout == ItemsLayout.Centered)
            {
                availWidth = Width / 2;
                availHeight = Height / 2;
            }

            //Information about rows and columns are present
            RCPresent = ((columns > 0) && (dockOrientation == DockOrientation.Horizontal));

            foreach (DockItem item in Items)
            {

                item.Scale -= delta;
                if (item.Scale < 1.0f)
                    item.Scale = 1.0f;

                item.Order = (int)(defSize * item.Scale);

                item.ResetPaintSize();

                if (UseAlpha)
                {
                    item.Alpha = (byte)Math.Min(100 * item.Scale, 255);
                }
                else
                {
                    item.Alpha = 255;
                }

                if (DockOrientation == DockOrientation.Horizontal)
                {
                    if (itemsLayout == ItemsLayout.Centered)
                    {
                        item.X = (int)(availWidth + (item.InitialIndex - (total - 1) / 2) * (spacing + defSize) - (item.PaintWidth) / 2);
                        item.Y = (int)(availHeight - (item.PaintHeight) / 2);
                    }
                    else
                    {
                        if (!RCPresent)
                        {
                            item.Y = startY;
                            item.X = startX + item.InitialIndex * (spacing + defSize);
                        }
                        else
                        {
                            currColumn = GetColumnFromIndex(item.InitialIndex);
                            currRow = GetRowFromIndex(item.InitialIndex);
                            item.X = startX + currColumn * (spacing + defSize);
                            item.Y = startY + currRow * (spacing + defSize);
                        }
                    }
                }
                else
                {
                    if (itemsLayout == ItemsLayout.Centered)
                    {
                        item.Y = (int)(availHeight + (item.InitialIndex - (total - 1) / 2) * (spacing + defSize) - (item.PaintHeight) / 2);
                        item.X = (int)(availWidth - (item.PaintWidth) / 2);
                    }
                    else
                    {
                        item.X = startX;
                        item.Y = startY + item.InitialIndex * (spacing + defSize);
                    }
                }
            }

            Items.Sort(Comparer);
        }



        public override void DoMouseMove(int CurX, int CurY)
        {
            int startX = GetLeftMargin();
            int startY = GetTopMargin();
            int defSize = GetIconSize();
            int spacing = GetIconsSpacing();
            double maxScale = this.MaxScale;
            double multiplier = this.Multiplier;
            double imageScale;
            double vScale = 0;
            double hScale = 0;
            int total = Items.Count;
            int currRow = 0;
            int currColumn = 0;
            bool RCPresent;
            double k = 2.55 / maxScale;

            int availWidth = 0;
            int availHeight = 0;

            if (itemsLayout == ItemsLayout.Centered)
            {
                availWidth = Width / 2;
                availHeight = Height / 2;
            }


            //Information about rows and columns are present
            RCPresent = ((columns > 0) && (dockOrientation == DockOrientation.Horizontal));

            foreach (DockItem item in Items)
            {
                if (DockOrientation == DockOrientation.Horizontal)
                {
                    if (!RCPresent)
                        //normal scaling
                        imageScale = maxScale - Math.Min(maxScale - 1, Math.Abs(CurX - ((double)item.X + item.PaintWidth / 2)) / multiplier);
                    else
                    {
                        hScale = maxScale - Math.Min(maxScale - 1, Math.Abs(CurX - ((double)item.X + item.PaintWidth / 2)) / multiplier);
                        vScale = maxScale - Math.Min(maxScale - 1, Math.Abs(CurY - ((double)item.Y + item.PaintHeight / 2)) / multiplier);
                        imageScale = Math.Min(vScale, hScale);
                    }
                }
                else
                {
                    imageScale = maxScale - Math.Min(maxScale - 1, Math.Abs(CurY - ((double)item.Y + item.PaintHeight / 2)) / multiplier);
                }

                if (RCPresent)
                {
                    currColumn = GetColumnFromIndex(item.InitialIndex);
                    currRow = GetRowFromIndex(item.InitialIndex);
                }

                if (Math.Abs(item.Scale - (float)imageScale) > 0.02)
                {
                    item.Scale = (float)imageScale;
                }

                item.Order = (int)(defSize * imageScale);

                item.ResetPaintSize();

                if (UseAlpha)
                {
                    if (maxScale > 2.55)
                        item.Alpha = (byte)Math.Min(100 * imageScale, 255);
                    else
                        item.Alpha = (byte)Math.Min(100 * imageScale * k, 255);
                }
                else
                {
                    item.Alpha = 255;
                }

                if (DockOrientation == DockOrientation.Horizontal)
                {
                    if (itemsLayout == ItemsLayout.Centered)
                    {
                        item.X = (int)(availWidth + (item.InitialIndex - (total - 1) / 2) * (spacing + defSize) - (item.PaintWidth) / 2);
                        item.Y = (int)(availHeight - (item.PaintHeight) / 2);
                    }
                    else
                    {
                        if (!RCPresent)
                        {
                            item.Y = startY;
                            item.X = startX + item.InitialIndex * (spacing + defSize);
                        }
                        else
                        {
                            item.X = startX + currColumn * (spacing + defSize);
                            item.Y = startY + currRow * (spacing + defSize);
                        }
                    }
                }
                else
                {
                    if (itemsLayout == ItemsLayout.Centered)
                    {
                        item.Y = (int)(availHeight + (item.InitialIndex - (total - 1) / 2) * (spacing + defSize) - (item.PaintHeight) / 2);
                        item.X = (int)(availWidth - (item.PaintWidth) / 2);
                    }
                    else
                    {
                        item.X = startX;
                        item.Y = startY + item.InitialIndex * (spacing + defSize);
                    }
                }
            }

            Items.Sort(Comparer);
        }


        protected double Multiplier
        {
            get
            {
                if (Settings == null)
                    return DefaultDockSettings.Multiplier;
                else
                    return Settings.Multiplier;
            }
        }

        protected double MaxScale
        {
            get
            {
                if (Settings == null)
                    return DefaultDockSettings.MaxScale;
                else
                    return Settings.MaxScale;
            }
        }

        public override bool CaptionVisible(DockItem item)
        {
            if (Settings.ShowCaptions)
            {
                return (item.Scale > 1.1) && (Items.IndexOf(item) == (Items.Count - 1));
            }
            else
                return false;
        }

    }
}
