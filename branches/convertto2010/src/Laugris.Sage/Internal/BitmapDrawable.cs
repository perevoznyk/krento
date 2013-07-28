using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    public class BitmapDrawable : Drawable
    {
        private int bitmapWidth;
        private int bitmapHeight;
        private Bitmap bitmap;

        public BitmapDrawable()
        {
            ComputeSize();
        }
        
        public BitmapDrawable(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        private void ComputeSize()
        {
            if (bitmap == null)
            {
                bitmapWidth = bitmapHeight = -1;
            }
            else
            {
                bitmapWidth = bitmap.Width;
                bitmapHeight = bitmap.Height;
            }
        }

        public Bitmap Bitmap
        {
            get { return bitmap; }
            set
            {
                bitmap = value;
                ComputeSize();
            }
        }

        public int BitmapWidth
        {
            get { return bitmapWidth; }
        }

        public int BitmapHeight
        {
            get { return bitmapHeight; }
        }

        public override void Draw(Graphics canvas)
        {
            BitmapPainter.DrawImageScaled(bitmap, canvas, Left, Top, Width, Height, Alpha);
        }

        public static BitmapDrawable FromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return new BitmapDrawable();
            else
            {
                Bitmap tmp = BitmapPainter.ConvertToRealColors(bitmap, false);
                return new BitmapDrawable(tmp);
            }
        }

    }
}
