using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Laugris.Sage
{

    /// <summary>
    /// Represents native Windows font
    /// </summary>
    public sealed class WindowsFont : IDisposable, ICloneable
    {

        private IntPtr hFont;
        private Color color;
        private int Win32Color;

        private WindowsFont(IntPtr hFont)
        {
            this.hFont = hFont;
            color = Color.FromKnownColor(KnownColor.WindowText);
            Win32Color = ColorTranslator.ToWin32(color);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFont"/> class.
        /// </summary>
        /// <param name="faceName">Name of the face.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        /// <param name="quality">The quality.</param>
        public WindowsFont(string faceName, int size, FontStyle style, WindowsFontQuality quality)
        {
            hFont = NativeMethods.CreateWindowsFont(faceName, size, (int)style, (int)quality);
            color = Color.FromKnownColor(KnownColor.WindowText);
            Win32Color = ColorTranslator.ToWin32(color);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFont"/> class.
        /// </summary>
        /// <param name="faceName">Name of the face.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public WindowsFont(string faceName, int size, FontStyle style)
        {
            hFont = NativeMethods.CreateWindowsFont(faceName, size, (int)style, (int)WindowsFontQuality.AntiAliased);
            color = Color.FromKnownColor(KnownColor.WindowText);
            Win32Color = ColorTranslator.ToWin32(color);
        }

        public WindowsFont(string faceName, int size)
        {
            hFont = NativeMethods.CreateWindowsFont(faceName, size, (int)FontStyle.Regular, (int)WindowsFontQuality.AntiAliased);
            color = Color.FromKnownColor(KnownColor.WindowText);
            Win32Color = ColorTranslator.ToWin32(color);
        }

        public static WindowsFont FromFont(Font font)
        {
            if (font == null)
                return null;
            else
            {
                IntPtr fontHandle = font.ToHfont();
                return new WindowsFont(fontHandle);
            }
        }

        public static WindowsFont FromHFont(IntPtr hFont)
        {
            if (hFont == IntPtr.Zero)
                return null;
            else
                return new WindowsFont(hFont);
        }

        public object ToLogFont()
        {
            LOGFONT result = new LOGFONT();
            NativeMethods.GetObject(hFont, Marshal.SizeOf(result), ref result);
            return result;
        }


        /// <summary>
        /// Created default font Tahoma 10px
        /// </summary>
        /// <returns></returns>
        public static WindowsFont DefaultFont()
        {
            return new WindowsFont("Tahoma", 10);
        }

        ~WindowsFont()
        {
            Dispose(false);
        }

        private static int GetIntTextFormatFlags(TextFormatFlags flags)
        {
            if ((((ulong)flags) & 18446744073692774400L) == 0L)
            {
                return (int)flags;
            }
            return (((int)flags) & ((int)0xffffff));
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                Win32Color = ColorTranslator.ToWin32(color);
            }
        }

        public IntPtr Hfont
        {
            get
            {
                return this.hFont;
            }
        }

        /// <summary>
        /// Draws the specified text at the specified location using the specified device context and color
        /// </summary>
        /// <param name="dc">The device context in which to draw the text.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="pt">The Point that represents the upper-left corner of the drawn text</param>
        /// <param name="foreColor">The Color to apply to the drawn text</param>
        public void DrawText(IDeviceContext dc, string text, Point pt, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr = ColorTranslator.ToWin32(foreColor);
                NativeMethods.DrawTextLine(hDc, text, hFont, clr, pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public void DrawTextOutline(IDeviceContext dc, string text, Point pt, Color foreColor, Color backColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr1 = ColorTranslator.ToWin32(foreColor);
                int clr2 = ColorTranslator.ToWin32(backColor);
                NativeMethods.DrawTextOutline(hDc, text, hFont, clr1, clr2, pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }


        public void DrawTextGlow(IDeviceContext dc, string text, Point pt, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr1 = ColorTranslator.ToWin32(foreColor);
                NativeMethods.DrawTextGlow(hDc, text, hFont, clr1,  pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }


        public void DrawTextGlow(IDeviceContext dc, string text, Point pt)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                NativeMethods.DrawTextGlow(hDc, text, hFont, Win32Color, pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public void DrawText(IDeviceContext dc, string text, Rectangle bounds, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr = ColorTranslator.ToWin32(foreColor);
                RECT rect = new RECT(bounds);
                NativeMethods.DrawAlphaTextRect(hDc, text, hFont, clr, ref rect, 0);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }



        public void DrawText(IDeviceContext dc, string text, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr = ColorTranslator.ToWin32(foreColor);
                int param = GetIntTextFormatFlags(flags);
                RECT rect = new RECT(bounds);
                NativeMethods.DrawAlphaTextRect(hDc, text, hFont, clr, ref rect, param);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public void DrawText(IDeviceContext dc, string text, Rectangle bounds, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int param = GetIntTextFormatFlags(flags);
                RECT rect = new RECT(bounds);
                NativeMethods.DrawAlphaTextRect(hDc, text, hFont, Win32Color, ref rect, param);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public void DrawText(IDeviceContext dc, string text, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                RECT rect = new RECT(bounds);
                NativeMethods.DrawAlphaTextRect(hDc, text, hFont, Win32Color, ref rect, 0);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public void DrawText(IDeviceContext dc, string text, Point pt)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                NativeMethods.DrawTextLine(hDc, text, hFont, Win32Color, pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }


        public void DrawText(IntPtr hDc, string text, Point pt, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr = ColorTranslator.ToWin32(foreColor);
            NativeMethods.DrawTextLine(hDc, text, hFont, clr, pt.X, pt.Y);
        }

        public void DrawText(IntPtr hDc, string text, Rectangle bounds, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr = ColorTranslator.ToWin32(foreColor);
            RECT rect = new RECT(bounds);
            NativeMethods.DrawAlphaTextRect(hDc, text, hFont, clr, ref rect, 0);
        }


        public void DrawText(IntPtr hDc, string text, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr = ColorTranslator.ToWin32(foreColor);
            int param = GetIntTextFormatFlags(flags);
            RECT rect = new RECT(bounds);
            NativeMethods.DrawAlphaTextRect(hDc, text, hFont, clr, ref rect, param);
        }

        public void DrawText(IntPtr hDc, string text, Rectangle bounds, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int param = GetIntTextFormatFlags(flags);
            RECT rect = new RECT(bounds);
            NativeMethods.DrawAlphaTextRect(hDc, text, hFont, Win32Color, ref rect, param);
        }

        public void DrawText(IntPtr hDc, string text, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(text))
                return;

            RECT rect = new RECT(bounds);
            NativeMethods.DrawAlphaTextRect(hDc, text, hFont, Win32Color, ref rect, 0);
        }


        public void DrawText(IntPtr hDc, string text, Point pt)
        {
            if (string.IsNullOrEmpty(text))
                return;

            NativeMethods.DrawTextLine(hDc, text, hFont, Win32Color, pt.X, pt.Y);
        }

        public void DrawTextOutline(IntPtr hDc, string text, Point pt, Color foreColor, Color backColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr1 = ColorTranslator.ToWin32(foreColor);
            int clr2 = ColorTranslator.ToWin32(backColor);
            NativeMethods.DrawTextOutline(hDc, text, hFont, clr1, clr2, pt.X, pt.Y);
        }


        public void DrawTextGlow(IntPtr hDc, string text, Point pt, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr1 = ColorTranslator.ToWin32(foreColor);
            NativeMethods.DrawTextGlow(hDc, text, hFont, clr1,  pt.X, pt.Y);
        }

        public void DrawTextGlow(IntPtr hDc, string text, Point pt)
        {
            if (string.IsNullOrEmpty(text))
                return;

            NativeMethods.DrawTextGlow(hDc, text, hFont, Win32Color, pt.X, pt.Y);
        }

        public Size GetTextSize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            else
            {
                Size result = new Size();
                SIZE s = NativeMethods.GetTextLineSize(text, hFont);
                result.Width = s.cx;
                result.Height = s.cy;
                return result;
            }
        }

        public Size GetTextSize(string text, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            else
            {
                Size result = new Size();
                int param = GetIntTextFormatFlags(flags);
                SIZE s = NativeMethods.GetTextSize(text, hFont, param);
                result.Width = s.cx;
                result.Height = s.cy;
                return result;
            }
        }

        public Size GetTextSize(string text, int proposedWidth, TextFormatFlags flags, bool margins)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            else
            {
                Size result = new Size();
                int param = GetIntTextFormatFlags(flags);
                SIZE s = NativeMethods.GetTextSizeEx(text, hFont, proposedWidth, param, margins);
                result.Width = s.cx;
                result.Height = s.cy;
                return result;
            }
        }

        public void DirectDraw(IDeviceContext dc, string text, Point pt)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                NativeMethods.DrawTextDirect(hDc, text, hFont, Win32Color, pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }


        public void DirectDraw(IntPtr hDc, string text, Point pt, Color foreColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr = ColorTranslator.ToWin32(foreColor);
            NativeMethods.DrawTextDirect(hDc, text, hFont, clr, pt.X, pt.Y);
        }

        public void DirectDraw(IntPtr hDc, string text, Point pt, Color foreColor, Color backColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int clr = ColorTranslator.ToWin32(foreColor);
            int back = ColorTranslator.ToWin32(backColor);
            NativeMethods.DrawTextDirectEx(hDc, text, hFont, clr, back, pt.X, pt.Y);
        }

        public void DirectDraw(IDeviceContext dc, string text, Point pt, Color foreColor, Color backColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (dc == null)
            {
                throw new ArgumentNullException("dc");
            }

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr = ColorTranslator.ToWin32(foreColor);
                int back = ColorTranslator.ToWin32(backColor);
                NativeMethods.DrawTextDirectEx(hDc, text, hFont, clr, back, pt.X, pt.Y);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public void DrawTextRect(IDeviceContext dc, string text, Color foreColor, Color backColor, Rectangle bounds, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return;

            RECT rect = new RECT(bounds);

            IntPtr hDc = dc.GetHdc();
            try
            {
                int clr = ColorTranslator.ToWin32(foreColor);
                int back = ColorTranslator.ToWin32(backColor);
                NativeMethods.DrawTextRect(hDc, text, hFont, clr, back, ref rect, (int)flags);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public int Tag { get; set; }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (this.hFont != IntPtr.Zero)
                NativeMethods.DestroyFont(this.hFont);
            hFont = IntPtr.Zero;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            IntPtr newFont = NativeMethods.CloneFont(hFont);
            return new WindowsFont(newFont);
        }

        #endregion
    }
}
