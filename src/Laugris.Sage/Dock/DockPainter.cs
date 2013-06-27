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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Laugris.Sage
{
    /// <summary>
    /// Painter for windowed docks
    /// </summary>
    public class DockPainter : CustomDockPainter
    {
        public virtual void PaintCaption(Graphics canvas, DockItem item, bool scaleCaption)
        {
            if (item == null)
                return;

            if (canvas == null)
                return;

            if (!string.IsNullOrEmpty(item.Caption))
            {
                Font cFont = null;
                int captionWidth = 0;
                int captionHeight = 0;

                try
                {

                    if (scaleCaption)
                    {
                        cFont = new Font(this.Font.Name, this.Font.Size * item.Scale, this.Font.Style, GraphicsUnit.Point);

                        StringFormat format = (StringFormat)StringFormat.GenericTypographic;
                        try
                        {
                            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                            SizeF captionSizeF = canvas.MeasureString(item.Caption, cFont, new PointF(0, 0), format);
                            captionHeight = (int)captionSizeF.Height;
                            captionWidth = (int)captionSizeF.Width;

                            int posX = (int)(item.PaintWidth / 2) + item.X - captionWidth;
                            int posY = (int)(item.Y + (item.Height - item.ReflectionDepth) * item.Scale);
                            TextPainter.DrawString(canvas, item.Caption, cFont, posX, posY, captionWidth * 2, captionHeight * 2, this.ForeColor, true);
                        }
                        finally
                        {
                            format.Dispose();
                            format = null;
                        }
                    }
                    else
                    {
                        StringFormat format = (StringFormat)StringFormat.GenericTypographic;
                        try
                        {
                            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                            SizeF captionSizeF = canvas.MeasureString(item.Caption, this.Font, new PointF(0, 0), format);
                            captionHeight = (int)captionSizeF.Height;
                            captionWidth = (int)captionSizeF.Width;

                            int posX = (int)(item.PaintWidth / 2) + item.X - captionWidth;
                            int posY = (int)(item.Y + (item.Height - item.ReflectionDepth) * item.Scale);
                            TextPainter.DrawString(canvas, item.Caption, this.Font, posX, posY, captionWidth * 2, captionHeight * 2, this.ForeColor, true);
                        }
                        finally
                        {
                            format.Dispose();
                            format = null;
                        }
                    }

                }
                finally
                {
                    if (cFont != null)
                    {
                        cFont.Dispose();
                        cFont = null;
                    }
                }
            }

        }

        protected override void DoDefaultPaint(Graphics canvas, IDockManager manager)
        {
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < manager.Items.Count; i++)
            {
                if (manager.UseDenomination)
                {
                    PaintDenominatedItem(canvas, manager.Items[i]);
                }
                else
                {
                    PaintItem(canvas, manager.Items[i]);
                }
                if (manager.CaptionVisible(manager.Items[i]))
                {
                    PaintCaption(canvas, manager.Items[i], manager.GetScaleCaption());
                }
            }

        }

        /// <summary>
        /// Performs the painting of one dock item in case if the icon size is not equal to the 
        /// given size of the image (denomination).
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="item">The item.</param>
        protected virtual void PaintDenominatedItem(Graphics g, DockItem item)
        {
            if (item != null)
            {
                if (item.Alpha == 255)
                    BitmapPainter.DrawImageScaled(item.Icon, g, item.X, item.Y, item.PaintWidth, item.PaintHeight);
                else
                {
                    BitmapPainter.DrawImageScaled(item.Icon, g, item.X, item.Y, item.PaintWidth, item.PaintHeight, item.Alpha);
                }
            }
        }

        protected virtual void PaintItem(Graphics g, DockItem item)
        {
            if (item != null)
            {
                if (item.Scale == 1.0f)
                {
                    if (item.Alpha == 255)
                        BitmapPainter.DrawImageUnscaled(item.Icon, g, item.X, item.Y);
                    else
                        BitmapPainter.DrawImageUnscaled(item.Icon, g, item.X, item.Y, item.Alpha);

                }
                else
                {
                    if (item.Alpha == 255)
                        BitmapPainter.DrawImageScaled(item.Icon, g, item.X, item.Y, item.PaintWidth, item.PaintHeight);
                    else
                    {
                        BitmapPainter.DrawImageScaled(item.Icon, g, item.X, item.Y, item.PaintWidth, item.PaintHeight, item.Alpha);
                    }
                }

            }
        }

    }
}
