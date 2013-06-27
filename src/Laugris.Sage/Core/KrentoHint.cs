using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Laugris.Sage
{
    public class KrentoHint : LayeredWindow
    {
        private string caption;
        private Color hintOutlineColor;
        private Color hintBorderColor;
        private Color hintBodyColor;

        private Pen penOutLine;
        private SolidBrush brushBorder;
        private SolidBrush brushBody;
        private WindowsFont windowsFont;

        public KrentoHint()
            : base()
        {

            Name = "KrentoHint";
            //this.Font = new Font("Arial", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            windowsFont = new WindowsFont("Tahoma", NativeMethods.EmToPixels(12), FontStyle.Bold, WindowsFontQuality.ClearType);

            this.Alpha = 200;
            this.ColorKey = ColorUtils.WhiteKey;
            this.TopMostWindow = true;

            ForeColor = Color.White;
            HintOutlineColor = Color.Black;
            HintBodyColor = Color.Black;
            HintBorderColor = Color.Gainsboro;

            windowsFont.Color = ForeColor;

        }

        public void SelectNewFont(string fontName, int fontSize, FontStyle fontStyle)
        {
            if (windowsFont != null)
                windowsFont.Dispose();
            fontSize = NativeMethods.EmToPixels(fontSize);
            windowsFont = new WindowsFont(fontName, fontSize, fontStyle, WindowsFontQuality.ClearType);
            windowsFont.Color = ForeColor;
            //if (Font != null)
            //    Font.Dispose();
            //Font = new Font(fontName, fontSize, fontStyle, GraphicsUnit.Pixel);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                    break;

            }
            base.WndProc(ref m);
        }

        public void MoveWindowDelta(int deltaX, int deltaY)
        {
            this.UpdatePosition(this.Left + deltaX, this.Top + deltaY);
        }

        public void PaintCaption(string caption)
        {
            this.caption = caption;
            this.Text = caption;
            RepaintAll();
        }

        public void RepaintAll()
        {
            //SizeF captionSizeF = TextRenderer.MeasureText(caption, Font, new Size(200, 1), TextFormatFlags.Left | TextFormatFlags.WordBreak);
            Size captionSize = windowsFont.GetTextSize(caption, 200, TextFormatFlags.Left | TextFormatFlags.WordBreak, true);

            int captionWidth = (int)(captionSize.Width + 1);
            int captionHeight = (int)captionSize.Height;

            int totalWidth = captionWidth + 8;
            int totalHeight = captionHeight + 8;

            this.Size(totalWidth, totalHeight);

            this.Clear();

            Bitmap tmp = new Bitmap(totalWidth, totalHeight, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(tmp))
            {

                //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.FillRectangle(brushBorder, new Rectangle(1, 1, totalWidth - 2, totalHeight - 2));
                g.FillRectangle(brushBody, new Rectangle(3, 3, totalWidth - 6, totalHeight - 6));
                g.DrawRectangle(penOutLine, new Rectangle(0, 0, totalWidth - 1, totalHeight - 1));
             //   TextRenderer.DrawText(g, caption, Font, new Rectangle(4, 4, captionWidth, captionHeight), ForeColor, hintBodyColor, TextFormatFlags.Left | TextFormatFlags.WordBreak);
              //  windowsFont.DirectDraw(g, caption, new Point(4, 4), ForeColor, hintBodyColor);
                windowsFont.DrawTextRect(g, caption, ForeColor, hintBodyColor, new Rectangle(4, 4, captionWidth, captionHeight), TextFormatFlags.WordBreak);


            }


            Canvas.DrawImageUnscaled(tmp, 0, 0);


            tmp.Dispose();

            this.Update(true);
        }

        public Color HintOutlineColor
        {
            get { return hintOutlineColor; }
            set
            {
                hintOutlineColor = value;
                if (penOutLine != null)
                    penOutLine.Dispose();
                penOutLine = new Pen(hintOutlineColor);
            }
        }

        public Color HintBorderColor
        {
            get { return hintBorderColor; }
            set
            {
                hintBorderColor = value;
                if (brushBorder != null)
                    brushBorder.Dispose();
                brushBorder = new SolidBrush(hintBorderColor);
            }
        }

        public Color HintBodyColor
        {
            get { return hintBodyColor; }
            set
            {
                hintBodyColor = value;
                if (brushBody != null)
                    brushBody.Dispose();
                brushBody = new SolidBrush(hintBodyColor);
            }
        }

        protected override void Dispose(bool disposing)
        {

            //Font.Dispose();
            windowsFont.Dispose();
            if (penOutLine != null)
                penOutLine.Dispose();
            if (brushBody != null)
                brushBody.Dispose();
            if (brushBorder != null)
                brushBorder.Dispose();

            base.Dispose(disposing);
        }
    }
}
