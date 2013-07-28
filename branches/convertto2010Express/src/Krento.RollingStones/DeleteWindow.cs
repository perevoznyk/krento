using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Krento.RollingStones
{
    public class DeleteWindow : LayeredWindow
    {
        private Bitmap normalSurface;
        private Bitmap seletedSurface;

        public DeleteWindow()
        {
            normalSurface = NativeThemeManager.LoadBitmap("DeleteWindowNormal.png");
            seletedSurface = NativeThemeManager.LoadBitmap("DeleteWindowSelected.png");
            this.Size(normalSurface.Width, normalSurface.Height);
            CanDrag = false;
        }

        public void DrawNormal()
        {
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            Canvas.CompositingQuality = CompositingQuality.HighQuality;
            Canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Clear();
            Canvas.DrawImage(normalSurface, 0, 0, Width, Height);
            Update();
        }

        public void DrawSeleted()
        {
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            Canvas.CompositingQuality = CompositingQuality.HighQuality;
            Canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Clear();
            Canvas.DrawImage(seletedSurface, 0, 0, Width, Height);
            Update();
        }

        public Rectangle RecycleArea
        {
            get { return new Rectangle(Left, Top, Width, Height); }
        }

        protected override void Dispose(bool disposing)
        {
            if (normalSurface != null)
                normalSurface.Dispose();
            if (seletedSurface != null)
                seletedSurface.Dispose();
            base.Dispose(disposing);
        }
    }
}
