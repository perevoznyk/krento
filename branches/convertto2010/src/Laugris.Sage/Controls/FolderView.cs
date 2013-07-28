using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Text;

namespace Laugris.Sage
{
    public class FolderView : Window
    {
        private readonly IntPtr parentWindow;
        private readonly List<FolderItem> items;

        private Bitmap backgroundImage;
        private Bitmap overlay;
        private Bitmap shine;
        private Bitmap logo;
        private Bitmap signClose;

        private readonly SkinOffset defaultMargins = new SkinOffset(24, 24, 24, 24);


        private int distance = 20;
        private int itemSize = 56;
        private int limit = 55;

        private readonly StringFormat format = new StringFormat();
        private int maxItems = 0;
        private int maxWidth = 0;
        private int rowCount = 0;
        private int selected = -1;
        private int oldSelected = -1;

        private int textHeight;
        private int headerTextWidth;

        public const int LogoSize = 48;
        public const int HeaderSize = LogoSize + 4;

        private Rectangle closeRect;
        private string headerText;

        public FolderView(IntPtr parentWindow)
        {
            this.parentWindow = parentWindow;
            items = new List<FolderItem>();
            TopMostWindow = true;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Near;
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.FormatFlags = StringFormatFlags.LineLimit;
            overlay = NativeThemeManager.LoadBitmap("overlay.png");
            shine = NativeThemeManager.LoadBitmap("shine.png");
            logo = NativeThemeManager.LoadBitmap("Krento48.png");
            signClose = NativeThemeManager.LoadBitmap("SignClose.png");
            this.Alpha = 240;
        }

        public int ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }

        public int Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public int Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        public bool FixedItemsNumber { get; set; }

        public int FixedRowSize { get; set; }

        public int ExtraItemsOffset { get; set; }

        public List<FolderItem> Items
        {
            get { return items; }
        }

        public string HeaderText
        {
            get { return headerText; }
            set { headerText = value; }
        }

        public bool Result { get; set; }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (closeRect.Contains(e.Location))
                {
                    selected = 0;
                    Result = false;
                    Hide();
                    return;
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Position.Contains(e.Location))
                    {
                        this.selected = i;
                        Result = true;
                        this.Hide();
                        return;
                    }
                }
            }
            base.OnMouseUp(e);
        }

        public int SelectedItem
        {
            get { return selected; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
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
                    selected = 0;
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


        protected virtual void BuildItems()
        {
        }

        internal virtual void LoadBackground()
        {
            Bitmap tmp = NativeThemeManager.LoadBitmap("TaskSwitch.png");
            LoadOptimizedBackgroundImage(tmp);
            tmp.Dispose();
        }

        internal void LoadOptimizedBackgroundImage(Bitmap original)
        {
            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
                backgroundImage = null;
            }

            backgroundImage = BitmapPainter.BuildImageFromSkin(original, Width, Height, defaultMargins);
        }


        public bool Execute()
        {
            return Execute(-1, -1);
        }

        protected void CalcRowsAndColumns()
        {
            int cntItems;

            if (FixedItemsNumber)
                cntItems = Limit;
            else
                cntItems = items.Count;

            if (FixedRowSize > 0)
                maxItems = FixedRowSize;
            else
                maxItems = (int)Math.Round(Math.Sqrt(cntItems)) + 1;
            rowCount = cntItems / maxItems;
            if (maxItems * rowCount < cntItems)
                rowCount++;

            if (rowCount == 1)
                maxItems = cntItems;
        }

        public virtual bool Execute(int leftCorner, int topCorner)
        {
            Screen screen;
            Rectangle bounds;

            Result = false;

            BuildItems();

            textHeight = (int)Canvas.MeasureString("Wg", this.Font, itemSize + distance / 2, format).Height * 2 + 4;

            using (Font headerFont = new Font("Tahoma", 24, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                headerTextWidth = (int)Canvas.MeasureString(headerText, headerFont, (int)(PrimaryScreen.Bounds.Width / 2), format).Width + 10;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Near;
            }

            selected = 0;

            CalcRowsAndColumns();

            maxWidth = maxItems * (itemSize + distance);

            if (rowCount == 1)
            {
                maxWidth = maxItems * (itemSize + distance);
            }

            int maxHeaderSize = distance + LogoSize + headerTextWidth + LogoSize + 4;
            if (maxWidth < maxHeaderSize)
                maxWidth = maxHeaderSize;

            SilentResize(maxWidth + distance, rowCount * (itemSize + textHeight) + distance * 3 + HeaderSize + ExtraItemsOffset);

            closeRect = new Rectangle(Width - distance - LogoSize, distance, LogoSize, LogoSize);

            LoadBackground();
            CalcPositions();
            PaintItems();

            if ((leftCorner == -1) && (topCorner == -1))
            {
                if (parentWindow != IntPtr.Zero)
                    screen = Screen.FromHandle(parentWindow);
                else
                {
                    Point mousePoint = PrimaryScreen.CursorPosition;
                    screen = Screen.FromPoint(mousePoint);
                }
                bounds = screen.Bounds;
                this.Left = (int)(bounds.Left + (bounds.Width / 2) - (this.Width / 2.0));
                this.Top = (int)(bounds.Top + (bounds.Height / 2) - (this.Height / 2.0));
            }
            else
            {
                this.Left = leftCorner - (Width / 2);
                this.Top = topCorner - (Height / 2);
                screen = Screen.FromPoint(new Point(leftCorner, topCorner));
                bounds = screen.Bounds;
            }

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


        internal void CalcPositions()
        {
            int offsetX;
            int offsetY = distance * 2 + HeaderSize + ExtraItemsOffset;
            int itemNumber;

            for (int i = 0; i < rowCount; i++)
            {
                offsetX = distance;

                for (int j = 0; j < maxItems; j++)
                {
                    itemNumber = i * maxItems + j;
                    if (itemNumber < (items.Count))
                    {

                        items[itemNumber].Position = new Rectangle(offsetX, offsetY, itemSize, itemSize);
                        offsetX += itemSize + distance;
                    }
                }
                offsetY += (itemSize + textHeight);
            }
        }

        protected void DrawLogoImage(Bitmap iconImage)
        {
            Canvas.DrawImage(iconImage, new RectangleF(distance, distance, LogoSize, LogoSize), new RectangleF(0, 0, iconImage.Width, iconImage.Height), GraphicsUnit.Pixel);
        }

        protected virtual void PaintWindowIcon()
        {
            DrawLogoImage(logo);
        }

        protected virtual void PaintHeaderText()
        {
            using (Font headerFont = new Font("Tahoma", 24, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                Canvas.DrawString(headerText, headerFont, Brushes.White, new RectangleF(distance + LogoSize + 4, distance, headerTextWidth, LogoSize), format);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Near;
            }
            
        }

        protected virtual void PaintWindowBackground()
        {
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            Canvas.DrawImage(backgroundImage, 0, 0);

            PaintWindowIcon();

            PaintHeaderText();

            Canvas.DrawImage(signClose, closeRect);

            using (Pen linePen = new Pen(Color.White, 2))
            {
                Canvas.DrawLine(linePen, distance, distance + HeaderSize + 4, Width - distance, distance + HeaderSize + 4);
            }
        }

        protected virtual void PaintItems()
        {

            Clear();
            Canvas.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Canvas.SmoothingMode = SmoothingMode.HighQuality;

            PaintWindowBackground();

            int itemNumber;


            for (int i = 0; i < rowCount; i++)
            {

                for (int j = 0; j < maxItems; j++)
                {
                    itemNumber = i * maxItems + j;
                    if (itemNumber < (items.Count))
                    {


                        if (itemNumber == selected)
                        {
                            Canvas.DrawImage(shine, items[itemNumber].Position);
                        }
                        else
                        {
                            Canvas.DrawImage(overlay, items[itemNumber].Position);
                        }

                        Canvas.DrawImage(items[itemNumber].Logo, new Rectangle(items[itemNumber].Position.Left + 4, items[itemNumber].Position.Top + 4, itemSize - 8, itemSize - 8));
                        Canvas.DrawString(items[itemNumber].Name, this.Font, Brushes.White, new RectangleF(items[itemNumber].Position.Left - (int)(distance / 2 - 2), items[itemNumber].Position.Top + itemSize,
                        itemSize + distance - 4, textHeight + 4), format);
                    }
                }
            }

            this.Update(true);

        }

        protected virtual void RepaintItems()
        {
            if ((items.Count > selected) && (oldSelected > -1))
            {
                Canvas.SetClip(items[selected].Position);
                try
                {
                    PaintItems();
                }
                finally
                {
                    Canvas.ResetClip();
                }
            }

            if ((items.Count > oldSelected) && (oldSelected > -1))
            {
                Canvas.SetClip(items[oldSelected].Position);
                try
                {
                    PaintItems();
                }
                finally
                {
                    Canvas.ResetClip();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case NativeMethods.WM_ACTIVATEAPP:
                    if (IntPtr.Zero == m.WParam)
                    {
                        Hide();
                    }
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    Hide();
                    m.Result = (IntPtr)1;
                    return;


            }
            base.WndProc(ref m);
        }

        protected void DisposeItems()
        {
            for (int i = 0; i < items.Count; i++)
                items[i].Dispose();
            items.Clear();
            selected = 0;
            oldSelected = -1;
        }

        protected override void Dispose(bool disposing)
        {
            DisposeItems();
            format.Dispose();

            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
                backgroundImage = null;
            }

            if (overlay != null)
            {
                overlay.Dispose();
                overlay = null;
            }

            if (shine != null)
            {
                shine.Dispose();
                shine = null;
            }

            if (logo != null)
            {
                logo.Dispose();
                logo = null;
            }


            if (signClose != null)
            {
                signClose.Dispose();
                signClose = null;
            }

            base.Dispose(disposing);
        }
    }
}
