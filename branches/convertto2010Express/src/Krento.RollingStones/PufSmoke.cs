using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Krento.RollingStones
{
    public class PufSmoke : LayeredWindow
    {
        private FrameAnimation animation;
        private string fileName;

        public PufSmoke()
            : base()
        {
            animation = new FrameAnimation(this);
            animation.Cycle = false;
            this.TopMostWindow = true;
            this.CanDrag = false;
            Name = "PufSmoke";
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public void StartSmoke(int x, int y)
        {
            Bitmap tmp = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (FileOperations.FileExists(fileName))
                    tmp = FastBitmap.FromFile(FileOperations.StripFileName(fileName));
            }

            if (tmp == null)
                tmp = NativeThemeManager.LoadBitmap("animation-poof.png");
            animation.Image = tmp;
            UpdatePosition(x - Width / 2, y - Width / 2);
            Update();
            ShowDialog();
        }

        public IntPtr NotificationHandle { get; set; }

        public void SmokeAtCursor()
        {
            Point screenPos = PrimaryScreen.CursorPosition;
            StartSmoke(screenPos.X, screenPos.Y);
        }

        internal void DrawNextFrame()
        {
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            Canvas.CompositingQuality = CompositingQuality.HighQuality;
            Canvas.InterpolationMode = InterpolationMode.High;
            Clear();
            Canvas.DrawImage(animation.ImageNext, 0, 0, Width, Height);
            Update();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == NativeMethods.CN_PAINT)
            {
                DrawNextFrame();
                return;
            }

            if (m.Msg == NativeMethods.CN_CLOSE)
            {
                Shutdown();
                return;
            }

            base.WndProc(ref m);
        }

        private void Shutdown()
        {
            if (NotificationHandle != IntPtr.Zero)
                NativeMethods.PostMessage(NotificationHandle, NativeMethods.CN_CLOSE, IntPtr.Zero, IntPtr.Zero);
            else
            {
                Hide();
                Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (animation != null)
            {
                animation.Dispose();
                animation = null;
            }
            base.Dispose(disposing);
        }
    }
}
