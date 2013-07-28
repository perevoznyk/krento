using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    public class VisualImage : Visual
    {
        private Bitmap glyph;

        public Bitmap Glyph
        {
            get { return glyph; }
            set { glyph = value; }
        }

        protected override void Paint()
        {
            if (glyph != null)
            {
                if (Alpha == 255)
                    BitmapPainter.DrawImageScaled(glyph, Canvas, Left, Top, Width, Height);
                else
                    BitmapPainter.DrawImageScaled(glyph, Canvas, Left, Top, Width, Height, Alpha);
            }
        }
    }
}
