using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Laugris.Sage;
using System.Drawing.Text;

namespace Krento.RollingStones
{
    public sealed class RingSwitcher : Window
    {
        private StonesManager manager;
        private Bitmap backgroundImage;
        private Bitmap overlay;
        private Bitmap shine;
        private Bitmap logo;
        private Bitmap signAdd;
        private Bitmap signClose;

        private int skinMarginLeft = 24;
        private int skinMarginRight = 24;
        private int skinMarginTop = 24;
        private int skinMarginBottom = 24;

        private SmoothingMode smoothingMode = SmoothingMode.HighQuality;
        private CompositingQuality compositingQuality = CompositingQuality.HighQuality;
        private InterpolationMode interpolationMode = GlobalConfig.InterpolationMode;
        private CompositingMode compositingMode = CompositingMode.SourceCopy;

        private List<KrentoRing> items;
        
        private const int distance = 20;
        private const int itemSize = 104;
        
        private StringFormat format = new StringFormat();
        private int maxItems = 0;
        private int maxWidth = 0;
        private int rowCount = 0;

        private int selected = -1;
        private int oldSelected = -1;

        private int textHeight;
        private int headerTextWidth;

        private Rectangle[] places;
        private bool inplace;

        private const int logoSize = 48;
        private const int headerSize = logoSize + 4;
        
        private Rectangle closeRect;

        public RingSwitcher(StonesManager manager, bool inplace)
        {
            this.manager = manager;
            items = new List<KrentoRing>();
            TopMostWindow = true;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Near;
            //format.FormatFlags = StringFormatFlags.NoWrap;
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.FormatFlags = StringFormatFlags.LineLimit;
            overlay = NativeThemeManager.LoadBitmap("overlay.png");
            shine = NativeThemeManager.LoadBitmap("shine.png");
            logo = NativeThemeManager.LoadBitmap("Krento48.png");
            signAdd = NativeThemeManager.LoadBitmap("SignAdd.png");
            signClose = NativeThemeManager.LoadBitmap("SignClose.png");

            this.inplace = inplace;
            this.Text = SR.SelectCircle;
            Name = "RingSwitcher";
            this.Alpha = 240;
        }

        internal void BuildItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                    items[i].Dispose();
            }

            items.Clear();
            KrentoRing ring;

            for (int i = 0; i < manager.HistoryCount; i++)
            {
                ring = new KrentoRing(manager.History[i].IniFile );
                items.Add(ring);
            }

            if (!inplace)
                items.Add(null);

            places = new Rectangle[items.Count];
        }

        public bool Result { get; set; }

        public bool Switch(int leftCorner, int topCorner)
        {
            int maxHeaderSize;
            Screen screen;
            Rectangle bounds;

            Result = false;

            BuildItems();

            textHeight = (int)Canvas.MeasureString("Wg", this.Font, itemSize + 10, format).Height * 2 + 4;

            if (!inplace)
            {
                using (Font headerFont = new Font("Tahoma", 24, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    headerTextWidth = (int)Canvas.MeasureString(SR.SelectCircle, headerFont, (int)(PrimaryScreen.Bounds.Width / 2), format).Width + 10;
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                }
            }

            SelectedIndex = manager.HistoryCurrentIndex;

            maxItems = (int)Math.Round(Math.Sqrt(items.Count)) + 1;
            rowCount = items.Count / maxItems;
            if (maxItems * rowCount < items.Count)
                rowCount++;
            maxWidth = maxItems * (itemSize + distance);


            if (rowCount == 1)
            {
                maxItems = items.Count;
                maxWidth = maxItems * (itemSize + distance);
            }

            if (!inplace)
            {
                maxHeaderSize = distance + logoSize + headerTextWidth + logoSize + 4;
                if (maxWidth < maxHeaderSize)
                    maxWidth = maxHeaderSize;
            }

            if (inplace)
                SilentResize(maxWidth + distance, rowCount * (itemSize + textHeight) + distance);
            else
                SilentResize(maxWidth + distance, rowCount * (itemSize + textHeight) + distance * 3 + headerSize);

            closeRect = new Rectangle(Width - distance - logoSize, distance, logoSize, logoSize);

            LoadBackground();
            CalcPositions();
            PaintItems();

            if (leftCorner < 0)
                this.Left = (int)(PrimaryScreen.Center.X - (this.Width / 2.0));
            else
                this.Left = leftCorner - (Width / 2);

            if (topCorner < 0)
                this.Top = (int)(PrimaryScreen.Center.Y - (this.Height / 2.0));
            else
                this.Top = topCorner - (Height / 2);

            screen = Screen.FromPoint(new Point(leftCorner, topCorner));
            bounds = screen.Bounds;

            if (this.Bounds.Right > bounds.Right)
            {
                this.Left -= (this.Bounds.Right - bounds.Right);
            }

            if (this.Left < bounds.Left)
            {
                this.Left += (bounds.Left - this.Left);
            }

            if (this.Bounds.Bottom > bounds.Bottom)
            {
                this.Top -= (this.Bounds.Bottom - bounds.Bottom);
            }

            if (this.Top < bounds.Top)
            {
                this.Top += (bounds.Top - this.Top);
            }

            this.ShowDialog();

            return Result;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case NativeMethods.WM_ACTIVATEAPP:
                    if (inplace)
                    {
                        if (IntPtr.Zero == m.WParam)
                        {
                            Hide();
                        }
                    }
                    break;

                case NativeMethods.WM_KILLFOCUS:
                    //  if (inplace)
                    {
                        Hide();
                        m.Result = (IntPtr)1;
                    }
                    return;

                default:
                    break;
            }
            base.WndProc(ref m);
        }

        public int SelectedIndex
        {
            get { return selected; }
            set
            {
                oldSelected = selected;
                selected = value;
            }
        }

        internal void CalcPositions()
        {
            int offsetX;
            int offsetY = inplace ? distance : distance * 2 + headerSize;
            int itemNumber;

            for (int i = 0; i < rowCount; i++)
            {
                offsetX = distance;

                for (int j = 0; j < maxItems; j++)
                {
                    itemNumber = i * maxItems + j;
                    if (itemNumber < (items.Count))
                    {

                        places[itemNumber] = new Rectangle(offsetX, offsetY, itemSize, itemSize);
                        offsetX += itemSize + distance;
                    }
                }
                offsetY += itemSize + textHeight;
            }
        }

        internal void PaintItems()
        {

            Clear();
            Canvas.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Canvas.SmoothingMode = smoothingMode;
            Canvas.CompositingQuality = compositingQuality;
            Canvas.InterpolationMode = interpolationMode;

            Canvas.DrawImage(backgroundImage, 0, 0);
            if (!inplace)
            {
                Canvas.DrawImage(logo, new RectangleF(distance, distance, logoSize, logoSize), new RectangleF(0, 0, logo.Width, logo.Height), GraphicsUnit.Pixel);
                using (Font headerFont = new Font("Tahoma", 24, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    Canvas.DrawString(SR.SelectCircle, headerFont, Brushes.White, new RectangleF(distance + logoSize + 4, distance, headerTextWidth, logoSize), format);
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                }

                Canvas.DrawImage(signClose, closeRect);

                using (Pen linePen = new Pen(Color.White, 2))
                {
                    Canvas.DrawLine(linePen, distance, distance + headerSize + 4, Width - distance, distance + headerSize + 4);
                }
            }

            int itemNumber;


            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < maxItems; j++)
                {
                    itemNumber = i * maxItems + j;
                    if (itemNumber < (items.Count))
                    {
                        if (itemNumber == SelectedIndex)
                        {
                            Canvas.DrawImage(shine, places[itemNumber]);
                        }
                        else
                        {
                            Canvas.DrawImage(overlay, places[itemNumber]);
                        }

                        format.LineAlignment = StringAlignment.Near;
                        if (items[itemNumber] != null)
                        {
                            Canvas.DrawImage(items[itemNumber].Logo, new Rectangle(places[itemNumber].Left + 4, places[itemNumber].Top + 4, itemSize - 8, itemSize - 8));
                            Canvas.DrawString(items[itemNumber].Caption, this.Font, Brushes.White, new RectangleF(places[itemNumber].Left - 5, places[itemNumber].Top + itemSize,
                                                            itemSize + 10, textHeight + 4), format);
                        }
                        else
                        {
                            Canvas.DrawImage(signAdd, new Rectangle(places[itemNumber].Left + 4, places[itemNumber].Top + 4, itemSize - 8, itemSize - 8));
                            Canvas.DrawString(SR.CreateCircle, this.Font, Brushes.White, new RectangleF(places[itemNumber].Left - 5, places[itemNumber].Top + itemSize,
                                                            itemSize + 10, textHeight + 4), format);
                        }
                    }
                }
            }
            this.Update(true);
        }

        internal void RepaintItems()
        {
            Canvas.SetClip(places[selected]);
            PaintItems();
            Canvas.ResetClip();

            Canvas.SetClip(places[oldSelected]);
            PaintItems();
            Canvas.ResetClip();
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if ((e.KeyCode == Keys.ControlKey) && inplace)
            {
                Result = true;
                this.Hide();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (places[i].Contains(e.Location))
                    {
                        this.SelectedIndex = i;
                        Result = true;
                        this.Hide();
                        return;
                    }
                }

                if (closeRect.Contains(e.Location) && (!inplace))
                {
                    SelectedIndex = manager.HistoryCurrentIndex;
                    Result = false;
                    Hide();
                    return;
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (inplace)
            {
                if ((e.KeyCode == Keys.Tab) && e.Control)
                {
                    if (e.Shift)
                        SelectPrevItem();
                    else
                        SelectNextItem();
                }
            }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    SelectPrevItem();
                    break;
                case Keys.Right:
                    SelectNextItem();
                    break;
                case Keys.Up:
                    SelectUpItem();
                    break;
                case Keys.Down:
                    SelectDownItem();
                    break;
                case Keys.Enter:
                    Result = items.Count > 0;
                    Hide();
                    break;
                case Keys.Escape:
                    SelectedIndex = manager.HistoryCurrentIndex;
                    Result = false;
                    Hide();
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int scrollSpeed = e.Delta;
            if (scrollSpeed < 0)
                SelectNextItem();
            else
                SelectPrevItem();

            base.OnMouseWheel(e);
        }

        public void SelectNextItem()
        {
            if (items.Count > 0)
            {
                oldSelected = selected;
                selected++;
                if (selected >= items.Count)
                    selected = 0;
                RepaintItems();
            }
        }

        public void SelectPrevItem()
        {
            if (items.Count > 0)
            {
                oldSelected = selected;
                selected--;
                if (selected < 0)
                    selected = items.Count - 1;
                RepaintItems();
            }
        }

        public void SelectUpItem()
        {
            oldSelected = selected;
            int row = selected / maxItems;
            row--;
            if (row < 0)
                row = rowCount - 1;
            int newItem = row * maxItems + (selected % maxItems);

            if (newItem < items.Count)
                selected = newItem;
            RepaintItems();
        }

        public void SelectDownItem()
        {
            oldSelected = selected;
            int row = selected / maxItems;
            row++;
            if (row >= rowCount)
                row = 0;
            int newItem = row * maxItems + (selected % maxItems);

            if (newItem < items.Count)
                selected = newItem;
            RepaintItems();
        }

        public List<KrentoRing> Items
        {
            get { return items; }
        }

        internal void LoadBackground()
        {
            Bitmap tmp = NativeThemeManager.LoadBitmap("TaskSwitch.png");
            LoadOptimizedBackgroundImage(tmp);
            tmp.Dispose();
        }

        internal void LoadOptimizedBackgroundImage(Bitmap original)
        {
            int bHeight = Height;
            int bWidth = Width;

            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
                backgroundImage = null;
            }

            backgroundImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(backgroundImage))
            {
                g.SmoothingMode = smoothingMode;
                g.CompositingQuality = compositingQuality;
                g.InterpolationMode = interpolationMode;
                g.CompositingMode = compositingMode;

                //Draw left part of the image
                g.DrawImage(original, new RectangleF(0, 0, skinMarginLeft, skinMarginTop), new RectangleF(-0.5f, -0.5f, skinMarginLeft, skinMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(0, skinMarginTop, skinMarginLeft, bHeight - skinMarginTop - skinMarginBottom), new RectangleF(-0.5f, skinMarginTop - 0.5f, skinMarginLeft, original.Height - skinMarginTop - skinMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(0, bHeight - skinMarginBottom, skinMarginLeft, skinMarginBottom), new RectangleF(-0.5f, original.Height - skinMarginBottom - 0.5f, skinMarginLeft, skinMarginBottom), GraphicsUnit.Pixel);

                //Draw central part of the image
                g.DrawImage(original, new RectangleF(skinMarginLeft, 0, bWidth - skinMarginLeft - skinMarginRight, skinMarginTop), new RectangleF(skinMarginLeft - 0.5f, -0.5f, original.Width - skinMarginLeft - skinMarginRight, skinMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(skinMarginLeft, bHeight - skinMarginBottom, bWidth - skinMarginLeft - skinMarginRight, skinMarginBottom), new RectangleF(skinMarginLeft - 0.5f, original.Height - skinMarginBottom - 0.5f, original.Width - skinMarginLeft - skinMarginRight, skinMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(skinMarginLeft, skinMarginTop, bWidth - skinMarginLeft - skinMarginRight, bHeight - skinMarginTop - skinMarginBottom), new RectangleF(skinMarginLeft - 0.5f, skinMarginTop - 0.5f, original.Width - skinMarginLeft - skinMarginRight, original.Height - skinMarginTop - skinMarginBottom), GraphicsUnit.Pixel);

                //Draw right part of the image
                g.DrawImage(original, new RectangleF(bWidth - skinMarginRight, 0, skinMarginRight, skinMarginTop), new RectangleF(original.Width - skinMarginRight - 0.5f, -0.5f, skinMarginRight, skinMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(bWidth - skinMarginRight, bHeight - skinMarginBottom, skinMarginRight, skinMarginBottom), new RectangleF(original.Width - skinMarginRight - 0.5f, original.Height - skinMarginBottom - 0.5f, skinMarginRight, skinMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(bWidth - skinMarginRight, skinMarginTop, skinMarginRight, bHeight - skinMarginTop - skinMarginBottom), new RectangleF(original.Width - skinMarginRight - 0.5f, skinMarginTop - 0.5f, skinMarginRight, original.Height - skinMarginTop - skinMarginBottom), GraphicsUnit.Pixel);
            }

        }

        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                    items[i].Dispose();
            }
            items.Clear();
            format.Dispose();
            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
                backgroundImage = null;
            }
            overlay.Dispose();
            shine.Dispose();
            logo.Dispose();
            signAdd.Dispose();
            signClose.Dispose();
            base.Dispose(disposing);
        }

    }
}
