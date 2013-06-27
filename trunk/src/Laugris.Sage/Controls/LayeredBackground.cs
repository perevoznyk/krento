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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    /// <summary>
    /// User control with layered background
    /// </summary>
    public class LayeredBackground : System.Windows.Forms.UserControl
    {

        private int transparency;
        private Bitmap layerImage;
        private Color layerColor;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush semiTransBrush;
            TextureBrush imageBrush;
            if (this.layerImage == null)
            {
                byte r;
                byte g;
                byte b;

                r = layerColor.R;
                g = layerColor.G;
                b = layerColor.B;
                semiTransBrush = new SolidBrush(Color.FromArgb(transparency, r,g, b));
                e.Graphics.CompositingQuality = CompositingQuality.GammaCorrected;
                e.Graphics.FillRectangle(semiTransBrush, 0, 0, Width, Height);
                e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), ClientRectangle);

            }
            else
            {
                imageBrush = new TextureBrush(layerImage, new Rectangle(0, 0, layerImage.Width, layerImage.Height));
                imageBrush.WrapMode = WrapMode.Tile;
                e.Graphics.FillRectangle(imageBrush, new Rectangle(0, 0, this.Width, this.Height));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredBackground"/> class.
        /// </summary>
        public LayeredBackground()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            transparency = 255;
            layerColor = System.Drawing.SystemColors.Control;
        }

        /// <summary>
        /// Gets or sets the color of the layer.
        /// </summary>
        /// <value>The color of the layer.</value>
        [Browsable(true)]
        public Color LayerColor
        {
            get
            {
                return layerColor;
            }
            set
            {
                layerColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The text associated with this control.
        /// </summary>
        /// <returns>The text associated with this control.</returns>
        [Browsable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the layer image.
        /// </summary>
        /// <value>The layer image.</value>
        [Browsable(true)]
        public Bitmap LayerImage
        {
            get
            {
                return layerImage;
            }
            set
            {
                layerImage = value;
                Invalidate();
            }
        }


        /// <summary>
        /// Gets or sets the trancparency.
        /// </summary>
        /// <value>The trancparency.</value>
        public int Trancparency
        {
            get
            {
                return transparency;
            }
            set
            {
                if (transparency != value)
                {
                    if (value > 255)
                        transparency = 255;
                    else if (value < 0)
                        transparency = 0;
                    else
                        transparency = value;
                    this.Invalidate();
                }
            }
        }
    }
}
