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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;


namespace Laugris.Sage
{

    public sealed class KrentoMenuColors
    {
        public Color BorderColor;
        public Color OutlineColor;
        public Color BodyColor;
        public Color NormalTextColor;
        public Color SelectedTextColor;
        public Color DisabledTextColor;
        public Color HighlightColor;
        public int FontSize;
        public string FontName;

        public KrentoMenuColors()
        {
            BorderColor = Color.Gainsboro;
            OutlineColor = Color.Black;
            BodyColor = Color.Black;
            NormalTextColor = Color.White;
            DisabledTextColor = Color.Gray;
            SelectedTextColor = Color.Black;
            FontSize = 12;
            FontName = "Tahoma";
            HighlightColor = Color.Orange;
        }
    }

    /// <summary>
    /// Layered and skinned popup menu
    /// </summary>
    public class KrentoMenu : IDisposable, IKrentoMenu
    {

        #region Skinning

        private static string skinFileName;
        private static int leftMargin = 38;
        private static int topMargin = 28;
        private static int bottomMargin = 28;
        private static int textTopMargin = 28;
        private static int textBottomMargin = 28;

        private static int textRightMargin = 8;
        private static int textLeftMargin = 8;

        private static int textOffset = 4;
        private static int outerBorderLeft = 0;
        private static int outerBorderTop = 0;
        private static int scrollDividerSize = 6;
        private static bool springBottom = false;

        private static int itemHeight = (int)(20 * NativeMethods.DpiY() / 96);
        private static int imageSize = 16;

        private static Bitmap defaultBackground;
        private static Bitmap defaultHighlight;
        private static System.Drawing.Font font = new System.Drawing.Font("Tahoma", (int)(12 * NativeMethods.DpiY() / 96), System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);

        #endregion

        private KrentoMenuWindow window;
        private KrentoMenuItems items;
        private int selectedIndex;
        private int shortCutOffset = 0;
        private bool hasImages = false;
        private int minWidth = 0;


        private Bitmap optimizedSurface;


        private bool active;

        private int firstVisibleItem = 0;
        private int numberOfVisibleItems = 10;
        private bool scrollUpSelected;
        private bool scrollDownSelected;
        private SolidBrush textBrush;
        StringFormat formatCaption;
        StringFormat formatShortCut;


        private TextRenderingHint renderHint = TextRenderingHint.AntiAliasGridFit;

        private static KrentoMenuColors menuColors = new KrentoMenuColors();

        /// <summary>
        /// Initializes a new instance of the <see cref="KrentoMenu"/> class.
        /// </summary>
        public KrentoMenu()
        {
            items = new KrentoMenuItems();
            selectedIndex = -1;
            textBrush = new SolidBrush(menuColors.NormalTextColor);

            formatCaption = new StringFormat();
            formatCaption.LineAlignment = StringAlignment.Center;
            formatCaption.FormatFlags = StringFormatFlags.NoWrap;
            formatCaption.Alignment = StringAlignment.Near;


            formatShortCut = new StringFormat();
            formatShortCut.LineAlignment = StringAlignment.Center;
            formatShortCut.FormatFlags = StringFormatFlags.NoWrap;
            formatShortCut.Alignment = StringAlignment.Far;

        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="KrentoMenu"/> is reclaimed by garbage collection.
        /// </summary>
        ~KrentoMenu()
        {
            Dispose(false);
        }

        public bool Active
        {
            get { return active; }
        }


        public static KrentoMenuColors MenuColors
        {
            get { return menuColors; }
            set { menuColors = value; }
        }

        public static bool DefaultSkin { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        [Description("The font used to display the text and the caption")]
        [Category("Appearance")]
        public static System.Drawing.Font Font
        {
            get
            {
                return font;
            }
            set { font = value; }
        }


        public string Caption { get; set; }

        protected void InitMenuWindow()
        {
            if (window == null)
            {
                window = new KrentoMenuWindow(this);
                window.TopMostWindow = true;
                window.CanDrag = false;
                window.BufferCreated += new EventHandler(window_BufferCreated);
            }

            if (!string.IsNullOrEmpty(Caption))
                window.Text = Caption;
        }

        void window_BufferCreated(object sender, EventArgs e)
        {
            window.Canvas.SmoothingMode = SmoothingMode.HighQuality;
            window.Canvas.CompositingQuality = CompositingQuality.HighQuality;
            window.Canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            window.Canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }


        public int MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
        }

        public IntPtr Handle
        {
            get
            {
                if (window == null)
                    return IntPtr.Zero;
                else
                    return window.Handle;
            }
        }

        /// <summary>
        /// Collection of the popup menu items
        /// </summary>
        /// <value>The items.</value>
        public KrentoMenuItems Items
        {
            get { return items; }
        }

        /// <summary>
        /// Adds new menu item.
        /// </summary>
        /// <returns></returns>
        public KrentoMenuItem AddItem()
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Add(item);
            return item;
        }

        /// <summary>
        /// Adds the new menu item.
        /// </summary>
        /// <param name="tag">The tag of the new menu item.</param>
        /// <returns></returns>
        public KrentoMenuItem AddItem(KrentoMenuTag tag)
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Add(item);
            item.Tag = tag;
            return item;
        }

        public KrentoMenuItem AddItem(KrentoMenuTag tag, Keys shortCut)
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Add(item);
            item.ShortCut = shortCut;
            item.Tag = tag;
            return item;
        }

        public KrentoMenuItem AddItem(EventHandler clickHandler)
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Add(item);
            item.Execute += new EventHandler(clickHandler);
            return item;
        }

        public KrentoMenuItem InsertItem(int index, EventHandler clickHandler)
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Insert(index, item);
            item.Execute += new EventHandler(clickHandler);
            return item;
        }

        public IKrentoMenuItem AddMenuItem(EventHandler clickHandler)
        {
            return (IKrentoMenuItem)AddItem(clickHandler);
        }

        public IKrentoMenuItem InsertMenuItem(int index, EventHandler clickHandler)
        {
            return (IKrentoMenuItem)InsertItem(index, clickHandler);
        }

        public int Count
        {
            get { return this.Items.Count; }
        }

        public void Clear()
        {
            this.Items.DisposeItems();
            this.Items.Clear();
        }

        public IKrentoMenuItem this[int index]
        {
            get
            {
                if (index >= items.Count)
                    return null;
                if (index < 0)
                    return null;

                return (IKrentoMenuItem)items[index];
            }
        }

        public IKrentoMenuItem this[string name]
        {
            get { return this.FindItemByName(name); }
        }

        public int IndexOf(IKrentoMenuItem item)
        {
            return items.IndexOf((KrentoMenuItem)item);
        }

        public void Remove(IKrentoMenuItem item)
        {
            items.Remove((KrentoMenuItem)item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public KrentoMenuItem AddItem(KrentoMenuTag tag, EventHandler clickHandler)
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Add(item);
            item.Tag = tag;
            item.Execute += new EventHandler(clickHandler);
            return item;
        }

        public KrentoMenuItem AddItem(Keys shortCut, EventHandler clickHandler)
        {
            KrentoMenuItem item = new KrentoMenuItem();
            Items.Add(item);
            item.ShortCut = shortCut;
            item.Execute += new EventHandler(clickHandler);
            return item;
        }

        /// <summary>
        /// Finds the item by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>First item with Tag property equal to tag value or null if no items found</returns>
        public KrentoMenuItem FindItemByTag(KrentoMenuTag tag)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Tag == tag)
                    return Items[i];
            }

            return null;
        }

        public KrentoMenuItem FindItemByShortCut(Keys shortCut)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ShortCut == shortCut)
                    return Items[i];
            }

            return null;
        }


        public KrentoMenuItem FindItemByCaption(string caption)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (TextHelper.SameText(Items[i].Caption, caption))
                    return Items[i];
            }

            return null;
        }

        public KrentoMenuItem FindItemByName(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (TextHelper.SameText(Items[i].Name, name))
                    return Items[i];
            }

            return null;
        }

        public KrentoMenuItem FindItemByData(string data)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (TextHelper.SameText(Items[i].Data, data))
                    return Items[i];
            }

            return null;
        }

        private void BuildOptimizedSurface()
        {
            int rightMargin = leftMargin;
            int bHeight = window.Height;
            int bWidth = window.Width;

            if (optimizedSurface != null)
            {
                optimizedSurface.Dispose();
                optimizedSurface = null;
            }

            optimizedSurface = new Bitmap(window.Width, window.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            if (defaultBackground == null)
            {
                BuildDefaultBackground();
            }

            if (defaultHighlight == null)
            {
                BuildDefaultHighlight();
            }

            using (Graphics g = Graphics.FromImage(optimizedSurface))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //Draw left part of the image
                g.DrawImage(defaultBackground, new Rectangle(0, 0, leftMargin, topMargin), new Rectangle(0, 0, leftMargin, topMargin), GraphicsUnit.Pixel);
                g.DrawImage(defaultBackground, new Rectangle(0, bHeight - bottomMargin, leftMargin, bottomMargin), new Rectangle(0, defaultBackground.Height - bottomMargin, leftMargin, bottomMargin), GraphicsUnit.Pixel);
                g.DrawImage(defaultBackground, new Rectangle(0, topMargin, leftMargin, bHeight - topMargin - bottomMargin), new Rectangle(0, topMargin, leftMargin, defaultBackground.Height - topMargin - bottomMargin), GraphicsUnit.Pixel);

                //Draw central part of the image
                g.DrawImage(defaultBackground, new Rectangle(leftMargin, 0, bWidth - leftMargin - rightMargin, topMargin), new Rectangle(leftMargin, 0, defaultBackground.Width - leftMargin - rightMargin, topMargin), GraphicsUnit.Pixel);
                g.DrawImage(defaultBackground, new Rectangle(leftMargin, bHeight - bottomMargin, bWidth - leftMargin - rightMargin, bottomMargin), new Rectangle(leftMargin, defaultBackground.Height - bottomMargin, defaultBackground.Width - leftMargin - rightMargin, bottomMargin), GraphicsUnit.Pixel);
                g.DrawImage(defaultBackground, new Rectangle(leftMargin, topMargin, bWidth - leftMargin - rightMargin, bHeight - topMargin - bottomMargin), new Rectangle(leftMargin, topMargin, defaultBackground.Width - leftMargin - rightMargin, defaultBackground.Height - topMargin - bottomMargin), GraphicsUnit.Pixel);

                //Draw right part of the image
                g.DrawImage(defaultBackground, new Rectangle(bWidth - rightMargin, 0, rightMargin, topMargin), new Rectangle(defaultBackground.Width - rightMargin, 0, rightMargin, topMargin), GraphicsUnit.Pixel);
                g.DrawImage(defaultBackground, new Rectangle(bWidth - rightMargin, bHeight - bottomMargin, rightMargin, bottomMargin), new Rectangle(defaultBackground.Width - rightMargin, defaultBackground.Height - bottomMargin, rightMargin, bottomMargin), GraphicsUnit.Pixel);
                g.DrawImage(defaultBackground, new Rectangle(bWidth - rightMargin, topMargin, rightMargin, bHeight - topMargin - bottomMargin), new Rectangle(defaultBackground.Width - rightMargin, topMargin, rightMargin, defaultBackground.Height - topMargin - bottomMargin), GraphicsUnit.Pixel);

            }
        }

        /// <summary>
        /// Draws menu
        /// </summary>
        internal void DrawSurface()
        {
            if (window == null)
                return;


            int firstItem;
            int lastItem;

            window.Clear();

            Graphics sg;
            Bitmap surface = new Bitmap(window.Width, window.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            sg = Graphics.FromImage(surface);

            if (optimizedSurface != null)
                sg.DrawImage(optimizedSurface, 0, 0);

            if (items.Count > 0)
            {

                firstItem = firstVisibleItem;
                lastItem = firstVisibleItem + numberOfVisibleItems;

                if (ScrollUpVisible)
                {
                    DrawScrollItem(sg, true, scrollUpSelected);
                    lastItem--;
                }

                if (ScrollDownVisible)
                {
                    DrawScrollItem(sg, false, scrollDownSelected);
                    lastItem--;
                }

                if (lastItem >= items.Count)
                    lastItem = items.Count;



                for (int i = firstItem; i < lastItem; i++)
                    DrawMenuItem(i, sg);
            }


            window.Canvas.DrawImageUnscaled(surface, 0, 0);
            sg.Dispose();
            surface.Dispose();

            window.Update(true);

        }

        internal int FindMenuItem(int y)
        {
            int itemIndex = -1;
            int count = items.Count;
            int topOffset = 0;
            int bottomOffset = 0;

            scrollUpSelected = false;
            scrollDownSelected = false;

            RectangleF topRect = ScrollAreaRectangle(true);
            RectangleF bottomRect = ScrollAreaRectangle(false);

            if (ScrollUpVisible)
            {
                topOffset = (int)topRect.Height + textTopMargin;
                if (y <= topOffset)
                {
                    scrollUpSelected = true;
                    return -1;
                }
            }

            if (ScrollDownVisible)
            {
                bottomOffset = (int)bottomRect.Height + textBottomMargin;
                if (y >= window.Height - bottomOffset)
                {
                    scrollDownSelected = true;
                    return -1;
                }
            }


            if (count > 0)
            {
                itemIndex = (y - textTopMargin) / itemHeight;
                if (itemIndex >= count)
                    itemIndex = count - 1;
                if (itemIndex < 0)
                    itemIndex = 0;

                if (itemIndex >= firstVisibleItem + numberOfVisibleItems)
                    itemIndex = firstVisibleItem + numberOfVisibleItems - 1;

                if (ScrollUpVisible)
                    itemIndex--;

                itemIndex = itemIndex + firstVisibleItem;
            }

            return itemIndex;
        }

        internal void DoMouseMove(int x, int y)
        {
            int itemIndex;

            itemIndex = FindMenuItem(y);

            if (scrollDownSelected || scrollUpSelected)
            {
                DrawSurface();
                return;
            }

            if (itemIndex != selectedIndex)
            {
                if ((itemIndex >= items.Count) || (itemIndex < 0))
                {
                    Deselect();
                }
                else
                {
                    if (items[itemIndex].Caption == "-")
                    {
                        Deselect();
                    }
                    else
                        SelectNewItem(selectedIndex, itemIndex);
                }
            }
        }


        internal bool DoMouseClick(int x, int y)
        {
            int itemIndex;
            itemIndex = FindMenuItem(y);
            if (itemIndex == -1)
            {
                if (scrollUpSelected)
                {
                    DoArrowUp();
                    return false;
                }
                else if (scrollDownSelected)
                {
                    DoArrowDown();
                    return false;
                }
            }

            return true;
        }

        public void Execute(KrentoMenuItem item)
        {
            if (item != null)
            {
                int itemIndex = items.IndexOf(item);
                if ((itemIndex >= 0) && (itemIndex < items.Count))
                {
                    using (KrentoMenuArgs args = new KrentoMenuArgs(items[itemIndex], itemIndex))
                    {
                        OnItemClick(args);
                    }
                }
            }
        }

        public void Execute()
        {
            if ((selectedIndex >= 0) && (selectedIndex < items.Count))
            {
                using (KrentoMenuArgs args = new KrentoMenuArgs(items[selectedIndex], selectedIndex))
                {
                    OnItemClick(args);
                }
                Deselect();
            }

        }

        /// <summary>
        /// Gets or sets the first visible item.
        /// </summary>
        /// <value>The first visible item.</value>
        protected int FirstVisibleItem
        {
            get { return firstVisibleItem; }
            set
            {
                if (firstVisibleItem != value)
                {
                    firstVisibleItem = value;
                    if (active)
                        DrawSurface();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of visible items.
        /// </summary>
        /// <value>The number of visible items.</value>
        public int NumberOfVisibleItems
        {
            get { return numberOfVisibleItems; }
            set { numberOfVisibleItems = value; }
        }

        /// <summary>
        /// If menu item has Execute event assigned, it will be called, otherwise
        /// MenuClick event of KarnaMenu class will be called
        /// </summary>
        /// <param name="e">Menu item</param>
        protected virtual void OnItemClick(KrentoMenuArgs e)
        {
            if (!e.MenuItem.Enabled)
                return;

            if (GlobalSettings.UseSound)
                NativeThemeManager.MakeSound("#106");
            if (e.MenuItem.EventAssigned)
            {
                e.MenuItem.DoExecute();
            }
            else
            {
                if (MenuClick != null)
                    MenuClick(this, e);
            }
        }

        public bool ScrollUpVisible
        {
            get { return firstVisibleItem > 0; }
        }

        public bool ScrollDownVisible
        {
            get
            {
                int cnt = numberOfVisibleItems;
                if (firstVisibleItem > 0)
                    cnt--;
                return (firstVisibleItem + cnt < items.Count);
            }
        }

        internal bool ValidIndex(int index)
        {
            if (items.Count == 0)
                return false;
            if (index >= items.Count)
                return false;
            if (index < 0)
                return false;
            return true;
        }

        /// <summary>
        /// Selects the new menu item.
        /// </summary>
        /// <param name="oldItem">The old selected item.</param>
        /// <param name="newItem">The new selected item.</param>
        internal void SelectNewItem(int oldItem, int newItem)
        {
            bool oneScroll;
            int realVisibleItems;


            if (ValidIndex(newItem))
            {
                if (items[newItem].Caption == "-")
                {
                    if (newItem > oldItem)
                        newItem++;
                    else
                        newItem--;
                }
            }

            if (items.Count == 0)
                selectedIndex = -1;
            else
                if (newItem >= items.Count)
                    selectedIndex = -1;
                else
                {
                    if (selectedIndex != newItem)
                    {
                        NativeMethods.NotifyWinEvent(0x8005, window.Handle, 0xFFFFFFFC, newItem + 1);
                        //NativeMethods.NotifyWinEvent(0x8006, window.Handle, 0xFFFFFFFC, newItem + 1);
                        // NativeMethods.NotifyWinEvent(0x800C, window.Handle, 0xFFFFFFFC, newItem + 1);
                    }
                    selectedIndex = newItem;
                }


            realVisibleItems = numberOfVisibleItems;
            if (ScrollUpVisible)
                realVisibleItems--;
            if (ScrollDownVisible)
                realVisibleItems--;

            if (firstVisibleItem != newItem)
            {
                if ((oldItem < (firstVisibleItem + realVisibleItems)) && (oldItem >= firstVisibleItem))
                {
                    if ((newItem < (firstVisibleItem + realVisibleItems)) && (newItem > firstVisibleItem))
                        oneScroll = true;
                    else
                        oneScroll = false;
                }
                else
                {
                    oneScroll = false;
                }

                if (!oneScroll)
                {
                    if ((firstVisibleItem - newItem == 1) && (newItem >= 0))
                        firstVisibleItem--;
                    else
                        firstVisibleItem = selectedIndex - realVisibleItems + 1;
                }

                //fix after selecting new firstVisibleItem

                realVisibleItems = numberOfVisibleItems;
                if (ScrollUpVisible)
                    realVisibleItems--;
                if (ScrollDownVisible)
                    realVisibleItems--;


                if (selectedIndex - firstVisibleItem + 1 > realVisibleItems)
                    firstVisibleItem++;


                //end fix

                if (firstVisibleItem >= items.Count)
                    firstVisibleItem = items.Count - 1;

                if (firstVisibleItem < 0)
                    firstVisibleItem = 0;
            }

            DrawSurface();
        }


        void DrawTriangle(Graphics g, float x, float y, bool flipped, bool selected)
        {
            int triHeight = itemHeight;
            int triWidth = itemHeight;

            float triHeight2 = triHeight / 2;
            float triHeight4 = triHeight / 4;

            PointF[] poligon;

            System.Drawing.Brush brush;
            if (selected)
                brush = new SolidBrush(menuColors.HighlightColor);
            else
                brush = new SolidBrush(menuColors.NormalTextColor);
            if (flipped)
            {
                g.FillPolygon(brush, new PointF[] {
					new PointF(x,                y + triHeight2 - triHeight4),
					new PointF(x + triWidth / 2, y + triHeight2 + triHeight4),
					new PointF(x + triWidth,     y + triHeight2 - triHeight4),
				});

            }
            else
            {
                poligon = new PointF[] {
					new PointF(x,                y +  triHeight2 + triHeight4),
					new PointF(x + triWidth / 2, y +  triHeight2 - triHeight4),
					new PointF(x + triWidth,     y +  triHeight2 + triHeight4),
				};
                g.FillPolygon(brush, poligon);
            }
            brush.Dispose();
        }

        private void DrawScrollItem(Graphics g, bool up, bool selected)
        {
            float top;

            if (up)
                top = textTopMargin;
            else
                top = (numberOfVisibleItems - 1) * itemHeight + TextTopMargin;



            DrawTriangle(g, (window.Width / 2) - (itemHeight / 2), top, !up, selected);

            RectangleF textRectangle = new RectangleF(textLeftMargin, top, (window.Width - textLeftMargin - textRightMargin), itemHeight);

            Pen linePen;

            if (selected)
                linePen = new Pen(menuColors.HighlightColor);
            else
                linePen = new Pen(menuColors.NormalTextColor);

            if (up)
                g.DrawLine(linePen, new PointF(textRectangle.Left + scrollDividerSize, textRectangle.Bottom - 1), new PointF(textRectangle.Right - scrollDividerSize, textRectangle.Bottom - 1));
            else
                g.DrawLine(linePen, new PointF(textRectangle.Left + scrollDividerSize, textRectangle.Top + 1), new PointF(textRectangle.Right - scrollDividerSize, textRectangle.Top + 1));

            linePen.Dispose();
        }

        private RectangleF ScrollAreaRectangle(bool up)
        {
            float top;

            if (up)
                top = textTopMargin;
            else
                top = (numberOfVisibleItems - 1) * itemHeight + textTopMargin;

            RectangleF scrollRectangle = new RectangleF(leftMargin, top, (window.Width - leftMargin * 2), itemHeight);

            return scrollRectangle;
        }

        /// <summary>
        /// Draws the menu item.
        /// </summary>
        /// <param name="menuItem">The krento menu item.</param>
        private void DrawMenuItem(int itemIndex, Graphics g)
        {
            Color textColor;
            RectangleF textRectangle;

            if (itemIndex < 0)
                return;
            if (itemIndex >= items.Count)
                return;
            KrentoMenuItem menuItem = items[itemIndex];
            if (menuItem == null)
                return;


            float top = (itemIndex - firstVisibleItem) * itemHeight;
            if (ScrollUpVisible)
                top += itemHeight;

            if (menuItem.Enabled)
            {
                if (selectedIndex == itemIndex)
                {
                    textColor = menuColors.SelectedTextColor;
                }
                else
                    textColor = menuColors.NormalTextColor;
            }

            else
            {
                textColor = menuColors.DisabledTextColor;
            }


            textRectangle = new RectangleF(textLeftMargin, (top + textTopMargin), (window.Width - textLeftMargin - textRightMargin), itemHeight);
            textBrush.Color = textColor;

            if (selectedIndex == itemIndex)
                g.DrawImage(defaultHighlight, textRectangle);

            if (hasImages)
                textRectangle.X += textOffset;

            if (menuItem.Image != null)
            {
                g.DrawImage(menuItem.Image, new RectangleF(textRectangle.X, textRectangle.Y + ((textRectangle.Height - imageSize) / 2),
                    imageSize, imageSize));

            }

            if (hasImages)
                textRectangle.X += imageSize;

            if (menuItem.Caption == "-")
            {
                Pen linePen = new Pen(menuColors.NormalTextColor);
                try
                {
                    g.DrawLine(linePen, new PointF(textRectangle.Left + textOffset, textRectangle.Top + itemHeight / 2), new PointF(textRectangle.Right - textOffset, textRectangle.Top + itemHeight / 2));
                }
                finally
                {
                    linePen.Dispose();
                }
            }
            else
            {
                #region Paint XP caption

                //if (DefaultSkin)
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                //else
                  //  g.TextRenderingHint = renderHint;

                textRectangle.X += textOffset;

                g.DrawString(menuItem.Caption, KrentoMenu.Font, textBrush, textRectangle, formatCaption);

                if (!string.IsNullOrEmpty(menuItem.ShortCutName))
                {
                    g.DrawString(menuItem.ShortCutName, KrentoMenu.Font, textBrush,
                        new RectangleF(shortCutOffset, (top + textTopMargin), (window.Width - shortCutOffset - textRightMargin - textOffset), itemHeight), formatShortCut);

                }
                #endregion
            }
        }

        /// <summary>
        /// Popups at cursor position.
        /// </summary>
        public void PopupAtCursor()
        {
            POINT pt = new POINT();
            if (NativeMethods.GetCursorPos(ref pt))
            {
                PopupAt(pt.x, pt.y);
            }
        }

        /// <summary>
        /// Popups at specified point at the screen.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="deselect">if set to <c>true</c> [deselect].</param>
        public void PopupAt(int x, int y, bool deselectItem)
        {
            if (items.Count == 0)
            {
                return;
            }

            InitMenuWindow();

            if (window != null)
            {

                //LoadSkin();
                if (active)
                    CloseUp();
                DoBeforePopup();

                CalculateWindowSize();
                CalculateWindowPosition(x, y);
                DrawSurface();

                window.Show(true);
                window.BringToFront();
                window.Activate();
                if (window.CanFocus)
                    window.Focus();
                if (deselectItem)
                    Deselect();
                DoAfterPopup();
                active = true;
                NativeMethods.NotifyWinEvent(6, window.Handle, 0xFFFFFFFC, 0);
                window.StartTrace();
            }
        }

        public void PopupAt(int x, int y)
        {
            PopupAt(x, y, true);
        }

        public void PopupAt(Point pt)
        {
            PopupAt(pt.X, pt.Y, true);
        }

        public void CloseUp()
        {
            if (window != null)
                NativeMethods.NotifyWinEvent(7, window.Handle, 0xFFFFFFFC, 0);

            if (active)
            {
                try
                {
                    if (window != null)
                    {
                        window.StopTrace();
                        window.Hide();
                    }
                    OnClose(EventArgs.Empty);
                }
                finally
                {
                    active = false;
                }
            }
            else
            {
                if (window != null)
                {
                    if (window.Visible)
                        window.Hide();
                }
            }
        }


        /// <summary>
        /// Calculates the window position.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private void CalculateWindowPosition(int x, int y)
        {

            if (window == null)
                return;

            if (springBottom)
            {
                x = x - (window.Width / 2);
                y = y - window.Height + outerBorderTop;
            }
            else
            {
                Rectangle screenRect = PrimaryScreen.WholeScreen(new Point(x, y));

                if (x < 0)
                    x = 0;

                if (y < 0)
                    y = 0;

                x = x - outerBorderLeft;

                if (y > PrimaryScreen.Center.Y)
                {
                    y = y - window.Height + outerBorderTop - 1;
                }
                else
                {
                    y = y - outerBorderTop;
                }

                if (y + window.Height > screenRect.Bottom)
                {
                    int delta = y + window.Height - screenRect.Bottom;
                    y = y - delta - 1;
                }

                if (x + window.Width > screenRect.Right)
                {
                    int delta = x + window.Width - screenRect.Right;
                    x = x - delta - 1;
                }
            }

            window.UpdatePosition(x, y);
        }

        /// <summary>
        /// Calculates the size of the window.
        /// </summary>
        private void CalculateWindowSize()
        {
            int h = 0;
            int w = 0;
            int maxWidth = 0;
            System.Drawing.SizeF captionSizeF;
            System.Drawing.SizeF scSizeF;
            int captionWidth = 0;
            int scWidth = 0;
            int maxSC = 0;
            int maxItems;
            hasImages = false;

            if (window == null)
                return;

            StringFormat format = new StringFormat();
            try
            {
                format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
                window.Canvas.TextRenderingHint = renderHint;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Image != null)
                        hasImages = true;
                    captionSizeF = window.Canvas.MeasureString(items[i].Caption, KrentoMenu.Font, new PointF(0, 0), format);
                    captionWidth = (int)(captionSizeF.Width + 1);
                    if (captionWidth > maxWidth)
                        maxWidth = captionWidth;

                    if (items[i].ShortCut == Keys.None)
                        scWidth = 0;
                    else
                    {
                        scSizeF = window.Canvas.MeasureString(items[i].ShortCutName, KrentoMenu.Font, new PointF(0, 0), format);
                        scWidth = (int)(scSizeF.Width + 1);
                        if (scWidth > maxSC)
                            maxSC = scWidth;
                    }
                }
            }
            finally
            {
                format.Dispose();
                format = null;
            }

            maxItems = NumberOfVisibleItems;
            if (maxItems > items.Count)
                maxItems = items.Count;

            h = maxItems * itemHeight + textTopMargin + textBottomMargin;

            w = maxWidth + textLeftMargin + textRightMargin + textOffset * 2;

            if (maxSC > 0)
            {
                shortCutOffset = maxWidth + textLeftMargin + textOffset * 3;
                w += maxSC + textOffset * 2;
            }
            else
                shortCutOffset = 0;

            if (hasImages)
                w += imageSize + textOffset;

            if (w < minWidth)
                w = minWidth;
            window.Size(w, h);

            BuildOptimizedSurface();
        }

        #region IDisposable Members


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                items.DisposeItems();
                items.Clear();

                textBrush.Dispose();
                formatCaption.Dispose();
                formatShortCut.Dispose();

                if (optimizedSurface != null)
                {
                    optimizedSurface.Dispose();
                    optimizedSurface = null;
                }

                if (window != null)
                {
                    try
                    {
                        window.Dispose();
                    }
                    finally
                    {
                        window = null;
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public events

        public event EventHandler BeforePopup;

        public event EventHandler AfterPopup;

        public event EventHandler<KrentoMenuArgs> MenuClick;

        public event EventHandler Close;

        #endregion

        protected virtual void OnClose(EventArgs e)
        {
            if (Close != null)
                Close(this, e);
        }

        protected virtual void OnBeforePopup(EventArgs e)
        {
            if (BeforePopup != null)
                BeforePopup(this, e);
        }

        protected virtual void OnAfterPopup(EventArgs e)
        {
            if (AfterPopup != null)
                AfterPopup(this, e);
        }

        private void DoBeforePopup()
        {
            OnBeforePopup(EventArgs.Empty);
        }

        public void Revalidate()
        {
            OnBeforePopup(EventArgs.Empty);
        }

        private void DoAfterPopup()
        {
            OnAfterPopup(EventArgs.Empty);
        }

        internal void DoMouseLeave()
        {
            SelectNewItem(selectedIndex, -1);
        }

        public void Deselect()
        {
            SelectNewItem(selectedIndex, -1);
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
        }

        public void SelectItem(int itemIndex)
        {
            SelectNewItem(selectedIndex, itemIndex);
        }

        internal void DoArrowUp()
        {
            if (items.Count == 0)
                return;
            int itemIndex = selectedIndex - 1;
            if (itemIndex < 0)
                SelectNewItem(-1, items.Count - 1);
            else
                SelectNewItem(selectedIndex, itemIndex);
        }

        internal void DoPageUp()
        {
            if (items.Count == 0)
                return;
            int itemIndex = selectedIndex - numberOfVisibleItems;
            if (ScrollUpVisible)
                itemIndex--;
            if (ScrollDownVisible)
                itemIndex--;

            if (itemIndex < 0)
                SelectNewItem(-1, items.Count - 1);
            else
                SelectNewItem(selectedIndex, itemIndex);
        }

        internal void DoArrowDown()
        {
            if (items.Count == 0)
                return;
            int itemIndex = selectedIndex + 1;
            if (itemIndex >= items.Count)
                SelectNewItem(selectedIndex, 0);
            else
                SelectNewItem(selectedIndex, itemIndex);
        }

        internal void DoPageDown()
        {
            if (items.Count == 0)
                return;
            int itemIndex = selectedIndex + numberOfVisibleItems;
            if (ScrollUpVisible)
                itemIndex--;
            if (ScrollDownVisible)
                itemIndex--;
            if (itemIndex >= items.Count)
                SelectNewItem(selectedIndex, 0);
            else
                SelectNewItem(selectedIndex, itemIndex);
        }

        internal void DoMouseWheel(int delta)
        {
            if (delta < 0)
            {
                DoArrowDown();
            }
            else
            {
                DoArrowUp();
            }
        }


        #region Skinning Implementation

        public static int LeftMargin
        {
            get { return leftMargin; }
            set { leftMargin = value; }
        }

        public static int TopMargin
        {
            get { return topMargin; }
            set { topMargin = value; }
        }

        public static int TextTopMargin
        {
            get { return textTopMargin; }
            set { textTopMargin = value; }
        }

        public static int TextBottomMargin
        {
            get { return textBottomMargin; }
            set { textBottomMargin = value; }
        }

        public static int BottomMargin
        {
            get { return bottomMargin; }
            set { bottomMargin = value; }
        }

        public static bool SpringBottom
        {
            get { return springBottom; }
            set { springBottom = value; }
        }

        public static int OuterBorderLeft
        {
            get { return outerBorderLeft; }
            set { outerBorderLeft = value; }
        }

        public static int OuterBorderTop
        {
            get { return outerBorderTop; }
            set { outerBorderTop = value; }
        }

        /// <summary>
        /// Gets or sets the text offset from the left side of the highlight bar.
        /// </summary>
        /// <value>The text offset from the left side of the highlight bar.</value>
        public static int TextOffset
        {
            get { return textOffset; }
            set { textOffset = value; }
        }

        /// <summary>
        /// Gets or sets the text highlight bar left margin.
        /// </summary>
        /// <value>The text highlight bar left margin.</value>
        public static int TextLeftMargin
        {
            get { return textLeftMargin; }
            set { textLeftMargin = value; }
        }

        /// <summary>
        /// Gets or sets the text highlight bar right margin.
        /// </summary>
        /// <value>The text highlight bar right margin.</value>
        public static int TextRightMargin
        {
            get { return textRightMargin; }
            set { textRightMargin = value; }
        }

        public static int ScrollDividerSize
        {
            get { return scrollDividerSize; }
            set { scrollDividerSize = value; }
        }


        public static int ItemHeight
        {
            get { return itemHeight; }
            set { itemHeight = value; }
        }


        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        public static int ImageSize
        {
            get { return imageSize; }
            set { imageSize = value; }
        }

        public static string SkinFileName
        {
            get { return skinFileName; }
            set
            {
                //if (!TextHelper.SameText(skinFileName, value))
                //{
                    skinFileName = value;
                    LoadSkin();
                //}
            }
        }

        private static void BuildDefaultBackground()
        {
            SolidBrush brushBorder = new SolidBrush(menuColors.BorderColor);
            SolidBrush brushBody = new SolidBrush(menuColors.BodyColor);
            Pen penOutLine = new Pen(menuColors.OutlineColor);

            int totalWidth = 180;
            int totalHeight = 180;

            
            defaultBackground = new Bitmap(totalWidth, totalHeight, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(defaultBackground))
            {
                g.FillRectangle(brushBorder, new Rectangle(1, 1, totalWidth - 2, totalHeight - 2));
                g.FillRectangle(brushBody, new Rectangle(3, 3, totalWidth - 6, totalHeight - 6));
                g.DrawRectangle(penOutLine, new Rectangle(0, 0, totalWidth - 1, totalHeight - 1));
            }

            brushBorder.Dispose();
            brushBody.Dispose();
            penOutLine.Dispose();

            //defaultBackground.Save("generated.png");
        }

        private static void BuildDefaultHighlight()
        {
            SolidBrush brushHighLightBottom = new SolidBrush(Color.FromArgb(127, menuColors.HighlightColor));
            SolidBrush brushHighLightTop = new SolidBrush(menuColors.HighlightColor);

            defaultHighlight = new Bitmap(200, 28, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(defaultHighlight))
            {
                g.FillRectangle(brushHighLightTop, new Rectangle(0, 0, 200, 14));
                g.FillRectangle(brushHighLightBottom, new Rectangle(0, 14, 200, 14));
            }

            brushHighLightTop.Dispose();
            brushHighLightBottom.Dispose();
        }

        private static void LoadDefaultMenuSkin()
        {

            DefaultSkin = true;
            if (defaultBackground != null)
            {
                defaultBackground.Dispose();
                defaultBackground = null;
            }

            BuildDefaultBackground();

            if (defaultHighlight != null)
            {
                defaultHighlight.Dispose();
                defaultHighlight = null;
            }

            BuildDefaultHighlight();

            skinFileName = null;
            leftMargin = 38;
            topMargin = 28;
            bottomMargin = 28;

            textLeftMargin = 8;
            textRightMargin = 8;
            textTopMargin = 8;
            textBottomMargin = 8;

            textOffset = 4;
            outerBorderLeft = 0;
            outerBorderTop = 0;
            scrollDividerSize = 6;
            springBottom = false;

            itemHeight = 20;
            imageSize = 16;

            if (font != null)
            {
                font.Dispose();
                font = null;
            }

            font = new System.Drawing.Font(menuColors.FontName, (int)(menuColors.FontSize * NativeMethods.DpiY() / 96), System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Loads the menu skin.
        /// </summary>
        public static void LoadSkin()
        {
            DefaultSkin = false;
            if (string.IsNullOrEmpty(skinFileName))
            {
                LoadDefaultMenuSkin();
                return;
            }

            if (!FileOperations.FileExists(skinFileName))
            {
                LoadDefaultMenuSkin();
                return;
            }

            KrentoMenuSkin skin = new KrentoMenuSkin(skinFileName);
            try
            {
                if (skin.LoadFromFile())
                {
                    itemHeight = (int)(skin.ItemHeight * NativeMethods.DpiY() / 96);
                    imageSize = skin.ImageSize;

                    leftMargin = skin.LeftMargin;
                    topMargin = skin.TopMargin;
                    bottomMargin = skin.BottomMargin;
                    textLeftMargin = skin.TextLeftMargin;
                    textRightMargin = skin.TextRightMargin;
                    textOffset = skin.TextOffset;
                    textTopMargin = skin.TextTopMargin;
                    textBottomMargin = skin.TextBottomMargin;
                    outerBorderLeft = skin.OuterBorderLeft;
                    outerBorderTop = skin.OuterBorderTop;
                    scrollDividerSize = skin.ScrollDividerSize;

                    menuColors.NormalTextColor = skin.ForeColor;
                    menuColors.SelectedTextColor = skin.SelectedColor;
                    menuColors.DisabledTextColor = skin.DisabledColor;
                    springBottom = skin.SpringBottom;

                    if (font != null)
                    {
                        font.Dispose();
                        font = null;
                    }
                    font = new System.Drawing.Font(skin.FontName, (int)(skin.FontSize * NativeMethods.DpiY() / 96), System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);

                    if (defaultHighlight != null)
                        defaultHighlight.Dispose();
                    if (defaultBackground != null)
                        defaultBackground.Dispose();
                    defaultHighlight = new Bitmap(skin.Highlight);
                    defaultBackground = new Bitmap(skin.Background);
                }
                else
                {
                    LoadDefaultMenuSkin();
                }
            }
            finally
            {
                skin.Dispose();
            }

        }

        #endregion
    }
}
