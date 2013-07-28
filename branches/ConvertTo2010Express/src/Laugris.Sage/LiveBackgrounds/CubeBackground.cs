using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;

namespace Laugris.Sage.LiveBackgrounds
{
    public class CubeBackground : LiveBackground
    {
        private int startTime;
        private int offset;
        private float mCenterX;
        private float mCenterY;
        long now = SystemClock.ElapsedRealtime();

        public CubeBackground(Window window)
            : base(window)
        {
            startTime = SystemClock.ElapsedRealtime();
            mCenterX = window.Width / 2.0f;
            mCenterY = window.Height / 2.0f;
        }

        public override void OnVisibilityChanged(bool value)
        {
            if (value)
                Run();
            else
                Stop();
        }

        public override void OnCanvasCreate()
        {
            Run();
        }

        protected internal override void Draw(Graphics canvas, bool reschedule)
        {
            if (reschedule)
                now = SystemClock.ElapsedRealtime();

            canvas.TranslateTransform(mCenterX, mCenterY);
            DrawLine(canvas, -400, -400, -400, 400, -400, -400);
            DrawLine(canvas, 400, -400, -400, 400, 400, -400);
            DrawLine(canvas, 400, 400, -400, -400, 400, -400);
            DrawLine(canvas, -400, 400, -400, -400, -400, -400);

            DrawLine(canvas, -400, -400, 400, 400, -400, 400);
            DrawLine(canvas, 400, -400, 400, 400, 400, 400);
            DrawLine(canvas, 400, 400, 400, -400, 400, 400);
            DrawLine(canvas, -400, 400, 400, -400, -400, 400);

            DrawLine(canvas, -400, -400, 400, -400, -400, -400);
            DrawLine(canvas, 400, -400, 400, 400, -400, -400);
            DrawLine(canvas, 400, 400, 400, 400, 400, -400);
            DrawLine(canvas, -400, 400, 400, -400, 400, -400);
            canvas.ResetTransform();

            if (reschedule)
                Reschedule(400);
        }

        public override void OnOffsetChanged(int offsetX, int offsetY)
        {
            offset = offsetX;
        }

        /*
         * Draw a 3 dimensional line on to the screen
         */
        void DrawLine(Graphics c, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            
            float xrot = ((float)(now - startTime)) / 1000;
            float yrot = (0.5f - offset) * 2.0f;

            // 3D transformations

            // rotation around X-axis
            float newy1 = (float)(Math.Sin(xrot) * z1 + Math.Cos(xrot) * y1);
            float newy2 = (float)(Math.Sin(xrot) * z2 + Math.Cos(xrot) * y2);
            float newz1 = (float)(Math.Cos(xrot) * z1 - Math.Sin(xrot) * y1);
            float newz2 = (float)(Math.Cos(xrot) * z2 - Math.Sin(xrot) * y2);

            // rotation around Y-axis
            float newx1 = (float)(Math.Sin(yrot) * newz1 + Math.Cos(yrot) * x1);
            float newx2 = (float)(Math.Sin(yrot) * newz2 + Math.Cos(yrot) * x2);
            newz1 = (float)(Math.Cos(yrot) * newz1 - Math.Sin(yrot) * x1);
            newz2 = (float)(Math.Cos(yrot) * newz2 - Math.Sin(yrot) * x2);

            // 3D-to-2D projection
            float startX = newx1 / (4 - newz1 / 400);
            float startY = newy1 / (4 - newz1 / 400);
            float stopX = newx2 / (4 - newz2 / 400);
            float stopY = newy2 / (4 - newz2 / 400);

            c.DrawLine(Pens.Black, startX, startY, stopX, stopY);
        }


    }
}
