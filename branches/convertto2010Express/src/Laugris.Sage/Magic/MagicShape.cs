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

namespace Laugris.Sage
{

    /// <summary>
    /// Information holder for <see cref="MagicShape"/>
    /// </summary>
    public class MagicEllipse : IDisposable
    {
        private double width;
        private double height;
        private Color fill;
        private double x;
        private double y;
        private SolidBrush fillBrush;

        public MagicEllipse()
        {
            fillBrush = new SolidBrush(fill);
        }

        ~MagicEllipse()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets the width of the ellipse.
        /// </summary>
        /// <value>The width of the ellipse.</value>
        public double Width
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// Gets or sets the height of the ellipse.
        /// </summary>
        /// <value>The height of the ellipse.</value>
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets or sets the color of the ellipse.
        /// </summary>
        /// <value>The color of the ellipse.</value>
        public Color Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                fillBrush.Color = value;
            }
        }

        public SolidBrush FillBrush
        {
            get { return fillBrush; }
        }

        /// <summary>
        /// Gets or sets the horizontal position of the ellipse.
        /// </summary>
        /// <value>The horizontal position of the ellipse.</value>
        public double X
        {
            get { return x; }
            set { x = value; }
        }
        /// <summary>
        /// Gets or sets the vertical position of the ellipse.
        /// </summary>
        /// <value>The vertical position of the ellipse.</value>
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        #region IDisposable Members

        protected void Dispose(bool disposing)
        {
            fillBrush.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }


    /// <summary>
    /// Base class for drawing different special effects
    /// </summary>
    public abstract class MagicShape : IDisposable
    {
        #region Private members
        private double x;
        private double y;
        private Color color;
        private int ellipseCount = 5;
        private int totalSize;

        private double opacity = 0.6;
        private List<MagicEllipse> Children = new List<MagicEllipse>();
        #endregion


        /// <summary>
        /// Gets or sets the number of ellipses. By default the shape is constructed from 5 
        /// ellipses
        /// </summary>
        /// <value>The number of the ellipses used to build the shape</value>
        public int EllipseCount
        {
            get { return ellipseCount; }
            set { ellipseCount = value; }
        }

        /// <summary>
        /// Gets the total size of the shape, including all ellipses
        /// </summary>
        /// <value>The total size of the shape.</value>
        public int TotalSize
        {
            get { return totalSize; }
        }

        /// <summary>
        /// Gets or sets the opacity of the shape. The value can be 0..1
        /// </summary>
        /// <value>The opacity of the shape.</value>
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (value < 0)
                    opacity = 0;
                else
                    if (value > 1.0)
                        opacity = 1.0;
                    else
                        opacity = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position of the shape.
        /// </summary>
        /// <value>The horizontal position of the shape.</value>
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Gets or sets the vertical position of the shape.
        /// </summary>
        /// <value>The vertical position of the shape.</value>
        public double Y
        {
            get { return y; }
            set { y = value; }
        }


        /// <summary>
        /// Gets or sets the color of the shape.
        /// </summary>
        /// <value>The color of the shape.</value>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }


        /// <summary>
        /// Performs the shape animation
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Paints the shape on the drawing surface
        /// </summary>
        /// <param name="canvas">The drawing surface.</param>
        public virtual void Paint(Graphics canvas)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                //using (SolidBrush brush = new SolidBrush(Children[i].Fill))
                //{
                canvas.FillEllipse(Children[i].FillBrush, (float)(X + Children[i].X), (float)(Y + Children[i].Y), (float)Children[i].Width, (float)Children[i].Height);
                //}
            }
        }


        protected void CreateEllipses(double size)
        {
            double opac = opacity;
            byte red = Color.R;
            byte green = Color.G;
            byte blue = Color.B;
            double opacityInc = opacity / (ellipseCount - 1);

            for (int i = 0; i < ellipseCount; i++)
            {
                MagicEllipse ellipse = new MagicEllipse();
                ellipse.Width = size;
                ellipse.Height = size;
                if (i == 0)
                {
                    ellipse.Fill = Color.FromArgb(255, 255, 255, 255);
                }
                else
                {
                    ellipse.Fill = Color.FromArgb((int)(opac * 255), red, green, blue);
                    opac -= opacityInc;
                    if (opac < 0)
                        opac = 0;
                    else
                        if (opac > 1.0)
                            opac = 1.0;
                    size += size;
                    if (size < 0)
                        size = 0;
                }

                ellipse.X = -ellipse.Width / 2;
                ellipse.Y = -ellipse.Height / 2;
                this.Children.Add(ellipse);
            }

            totalSize = (int)(size + 1);
        }

        /// <summary>
        /// Resets the opacity of the shape to the initial value.
        /// </summary>
        protected void ResetOpacity()
        {
            double opac = opacity;
            byte red = Color.R;
            byte green = Color.G;
            byte blue = Color.B;
            double opacityInc = opacity / (ellipseCount - 1);

            for (int i = 1; i < ellipseCount; i++)
            {
                MagicEllipse ellipse = Children[i];
                {
                    ellipse.Fill = Color.FromArgb((int)(opac * 255), red, green, blue);
                    opac -= opacityInc;
                    if (opac < 0)
                        opac = 0;
                    else
                        if (opac > 1.0)
                            opac = 1.0;
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MagicShape"/> class.
        /// </summary>
        /// <param name="red">The red component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="size">The size of the shape</param>
        protected MagicShape(byte red, byte green, byte blue, double size)
        {
            this.color = Color.FromArgb(red, green, blue);
            CreateEllipses(size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicShape"/> class.
        /// </summary>
        /// <param name="color">The color of the shape.</param>
        /// <param name="size">The size of the shape.</param>
        protected MagicShape(Color color, double size)
        {
            this.color = color;
            CreateEllipses(size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicShape"/> class.
        /// </summary>
        protected MagicShape()
            : base()
        {
        }

        ~MagicShape()
        {
            Dispose(false);
        }

        #region IDisposable Members

        protected void Dispose(bool disposing)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Dispose();
            }

            Children.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
