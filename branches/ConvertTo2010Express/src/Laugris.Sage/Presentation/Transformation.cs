using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Laugris.Sage
{
    /// <summary>
    /// Defines the transformation to be applied at
    /// one point in time of an Animation.
    /// </summary>
    public class Transformation : IDisposable
    {
        private Matrix matrix;
        private float alpha;

        public Transformation()
        {
            Clear();
        }

        ~Transformation()
        {
            Dispose(false);
        }

        public void Clear()
        {
            if (matrix == null)
            {
                matrix = new Matrix();
            }
            else
            {
                matrix.Reset();
            }

            alpha = 1.0f;
        }


        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        /// <summary>
        /// Gets the bounds of the transformation
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns></returns>
        public Rectangle TransformationBounds(Rectangle src)
        {
            Point V1 = new Point(src.Left, src.Top);
            Point V2 = new Point(src.Right, src.Top);
            Point V3 = new Point(src.Left, src.Bottom);
            Point V4 = new Point(src.Right, src.Bottom);

            Point[] pts = { V1, V2, V3, V4 };
            matrix.VectorTransformPoints(pts);
            int l, r, t, b;
            l = (int)Math.Round(Math.Min(Math.Min(pts[0].X, pts[1].X), Math.Min(pts[2].X, pts[3].X)) - 0.5);
            r = (int)Math.Round(Math.Max(Math.Max(pts[0].X, pts[1].X), Math.Max(pts[2].X, pts[3].X)) + 0.5);
            t = (int)Math.Round(Math.Min(Math.Min(pts[0].Y, pts[1].Y), Math.Min(pts[2].Y, pts[3].Y)) - 0.5);
            b = (int)Math.Round(Math.Max(Math.Max(pts[0].Y, pts[1].Y), Math.Max(pts[2].Y, pts[3].Y)) + 0.5);

            Rectangle result = Rectangle.FromLTRB(l, t, r, b);
            result.Offset((int)OffsetX, (int)OffsetY);
            return result;
        }


        public Matrix Matrix
        {
            get { return matrix; }
        }

        public float Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }


        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (matrix != null)
                matrix.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
