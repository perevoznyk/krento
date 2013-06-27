using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    public class TextLabel : Visual
    {
        private bool autoSize;
        private bool wordWrap;
        private StringAlignment layout;
        private StringAlignment alignment;
        private int Win32Color;

        public TextLabel(Window parent)
            : base(parent)
        {
            autoSize = true;
            Win32Color = ColorTranslator.ToWin32(ForeColor);
            SetSize(65, 17);
            Caption = "Text label";
        }

        public bool AutoSize
        {
            get { return autoSize; }
            set
            {
                if (autoSize != value)
                {
                    autoSize = value;
                    AdjustBounds();
                }
            }
        }

        public bool WordWrap
        {
            get { return wordWrap; }
            set
            {
                wordWrap = value;
                AdjustBounds();
                Invalidate();
            }
        }

        public StringAlignment Layout
        {
            get { return layout; }
            set
            {
                layout = value;
                Invalidate();
            }
        }

        public StringAlignment Alignment
        {
            get { return alignment; }
            set
            {
                alignment = value;
                Invalidate();
            }
        }

        protected override void OnCaptionChanged(EventArgs e)
        {
            base.OnCaptionChanged(e);
            Invalidate();
            AdjustBounds();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustBounds();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            Win32Color = ColorTranslator.ToWin32(ForeColor);
        }

        protected override void Paint()
        {
            if (Opaque)
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    Canvas.FillRectangle(brush, BoundsRect);
                }
            }

            TextFormatFlags flags;

            switch (alignment)
            {
                case StringAlignment.Center:
                    flags = TextFormatFlags.HorizontalCenter;
                    break;
                case StringAlignment.Far:
                    flags = TextFormatFlags.Right;
                    break;
                case StringAlignment.Near:
                    flags = TextFormatFlags.Left;
                    break;
                default:
                    flags = TextFormatFlags.Left;
                    break;
            }

            switch (layout)
            {
                case StringAlignment.Center:
                    flags |= TextFormatFlags.VerticalCenter;
                    break;
                case StringAlignment.Far:
                    flags |= TextFormatFlags.Bottom;
                    break;
                case StringAlignment.Near:
                    flags |= TextFormatFlags.Top;
                    break;
                default:
                    flags |= TextFormatFlags.Top;
                    break;
            }

            if (wordWrap)
                flags |= TextFormatFlags.WordBreak;

            IntPtr hdc = Canvas.GetHdc();
            IntPtr hf = Font.ToHfont();
            NativeMethods.DrawAlphaText(hdc, Caption, hf, Win32Color, BoundsRect.Width, BoundsRect.Height, (int)flags);
            NativeMethods.DeleteObject(hf);
            Canvas.ReleaseHdc(hdc);
        }

        internal SIZE GetTextSize()
        {
            IntPtr hFont = Font.ToHfont();
            int flags = 0;
            if (WordWrap)
                flags = 0x00000010;
            SIZE s = NativeMethods.GetTextSize(Caption, hFont, flags);
            NativeMethods.DeleteObject(hFont);
            return s;
        }

        protected void AdjustBounds()
        {
            if (autoSize)
            {
                SIZE s = GetTextSize();
                int x = Left;
                if (alignment == StringAlignment.Far)
                    x += (Width - s.cx);
                SetBounds(x, Top, s.cx, s.cy);
            }
        }
    }
}
