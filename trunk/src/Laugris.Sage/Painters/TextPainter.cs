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
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Laugris.Sage
{
    /// <summary>
    /// Static class for drawing the text with outlining and shadow
    /// </summary>
    public static class TextPainter
    {
        /// <summary>
        /// Default font name
        /// </summary>
        private const string defaultName = "Tahoma";
        /// <summary>
        /// Default font size
        /// </summary>
        private const float defaultSize = 10.0f;
        /// <summary>
        /// Default font style
        /// </summary>
        private const FontStyle defaultStyle = FontStyle.Bold;
        /// <summary>
        /// Default text color
        /// </summary>
        private static Color defaultColor = Color.White;

        private static readonly Font defaultFont = new Font(defaultName, defaultSize, defaultStyle, graphicsUnit);

        private const GraphicsUnit graphicsUnit = GraphicsUnit.Point;

        /// <summary>
        /// Default text outline transparency
        /// </summary>
        public static int DefaultOutlineTransparency = 140;
        /// <summary>
        /// Default shadow transparency
        /// </summary>
        public static int DefaultShadowTransparency = 64;
        /// <summary>
        /// Default halo transparency
        /// </summary>
        public static int DefaultHaloTransparency = 20;
        /// <summary>
        /// Default shadow horizontal offset
        /// </summary>
        public static int DefaultShadowX = 2;
        /// <summary>
        /// Default shadow vertical offset
        /// </summary>
        public static int DefaultShadowY = 2;
        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <value>The default font.</value>
        public static Font DefaultFont
        {
            get 
            { 
                return defaultFont;
            }
        }

        public static Color DefaultColor
        {
            get { return defaultColor; }
        }

        public static void DrawString(Graphics canvas, string text, int x, int y, int width, int height)
        {
            using (Font font = new Font(defaultName, defaultSize, defaultStyle, graphicsUnit))
            {
                DrawString(canvas, text, font, x, y, width, height, DefaultColor, false);
            }
        }

        public static void DrawString(Graphics canvas, string text, Font font, int x, int y, int width, int height)
        {
            DrawString(canvas, text, font, x, y, width, height, DefaultColor, false);
        }

        public static void DrawString(Graphics canvas, string text, Font font, int x, int y, int width, int height, Color bodyColor)
        {
            DrawString(canvas, text, font, x, y, width, height, bodyColor, false);
        }

        public static void DrawString(Graphics canvas, string text, int x, int y, int width, int height, bool centered)
        {
            using (Font font = new Font(defaultName, defaultSize, defaultStyle, graphicsUnit))
            {
                DrawString(canvas, text, font, x, y, width, height, DefaultColor, centered);
            }
        }

        public static void DrawString(Graphics canvas, string text, Font font, int x, int y, int width, int height, Color bodyColor, bool centered)
        {
            DrawString(canvas, text, font, x, y, width, height, bodyColor, centered, false);
        }

        public static void DrawString(Graphics canvas, string text, Font font, int x, int y, int width, int height, Color bodyColor, bool centered, bool wrap)
        {
            if (canvas == null)
                return;

            if (string.IsNullOrEmpty(text))
                return;

            if (font == null)
                return;

            float shadowX;
            float shadowY;
            GraphicsPath textPath;
            Pen outlinePen;
            SolidBrush bodyBrush;
            SolidBrush shadowBrush;
            StringFormat stringFormat;

            canvas.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;

            bodyBrush = new SolidBrush(bodyColor);
            outlinePen = new Pen(Color.FromArgb(DefaultOutlineTransparency, Color.Black));
            outlinePen.LineJoin = LineJoin.Round;
            shadowBrush = new SolidBrush(Color.FromArgb(DefaultShadowTransparency, Color.Black));
            if (!wrap)
                stringFormat = new StringFormat(StringFormatFlags.NoWrap);
            else
                stringFormat = new StringFormat();
            textPath = new GraphicsPath(FillMode.Alternate);

            try
            {
                #region Drawing
                stringFormat.Trimming = StringTrimming.EllipsisWord;
                if (centered)
                {
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                }

                shadowX = DefaultShadowX;
                shadowY = DefaultShadowY;

                textPath.AddString(text, font.FontFamily, (int)font.Style, font.GetHeight(canvas), new Rectangle(x, y, width, height), stringFormat);
                
                canvas.TranslateTransform(shadowX, shadowY);
                canvas.FillPath(shadowBrush, textPath);
                canvas.ResetTransform();

                canvas.FillPath(bodyBrush, textPath);
                canvas.DrawPath(outlinePen, textPath);
                #endregion
            }
            finally
            {
                outlinePen.Dispose();
                outlinePen = null;

                textPath.Dispose();
                textPath = null;

                stringFormat.Dispose();
                stringFormat = null;

                bodyBrush.Dispose();
                bodyBrush = null;

                shadowBrush.Dispose();
                shadowBrush = null;
            }
        }


        public static void DrawStringHalo(Graphics canvas, string text, Font font, int x, int y, int width, int height, Color bodyColor, bool centered)
        {

            if (canvas == null)
                return;

            if (string.IsNullOrEmpty(text))
                return;

            if (font == null)
                return;

            GraphicsPath textPath;
            Pen outlinePen;
            SolidBrush bodyBrush;
            SolidBrush shadowBrush;
            StringFormat stringFormat;

            canvas.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;

            bodyBrush = new SolidBrush(bodyColor);
            outlinePen = new Pen(Color.FromArgb(DefaultOutlineTransparency, Color.Black));
            shadowBrush = new SolidBrush(Color.FromArgb(DefaultHaloTransparency, bodyColor));
            stringFormat = new StringFormat(StringFormatFlags.NoWrap);
            textPath = new GraphicsPath(FillMode.Alternate);

            try
            {
                stringFormat.Trimming = StringTrimming.EllipsisWord;
                if (centered)
                {
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                }

                textPath.AddString(text, font.FontFamily, (int)font.Style, font.GetHeight(canvas), new Rectangle(x, y, width, height), stringFormat);

                int cnt = 1;
                do
                {

                    canvas.TranslateTransform(-cnt, -cnt);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(0, -cnt);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(cnt, -cnt);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(-cnt, 0);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(cnt, 0);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(-cnt, cnt);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(0, cnt);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();

                    canvas.TranslateTransform(cnt, cnt);
                    canvas.FillPath(shadowBrush, textPath);
                    canvas.ResetTransform();
                    cnt++;
                }
                while (cnt <= 3);

                canvas.FillPath(bodyBrush, textPath);
                canvas.DrawPath(outlinePen, textPath);
            }
            finally
            {
                outlinePen.Dispose();
                outlinePen = null;

                textPath.Dispose();
                textPath = null;

                stringFormat.Dispose();
                stringFormat = null;

                bodyBrush.Dispose();
                bodyBrush = null;

                shadowBrush.Dispose();
                shadowBrush = null;
            }
        }

    }
}
