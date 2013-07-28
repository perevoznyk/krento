//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Laugris.Sage
{
    public sealed class Pulsar : LayeredWindow
    {
        private const int pulseTimer = 2;
        private const int mouseTimer = 3;
        private const int timerLap = 50;
        private bool mouseLeft;
        private int rad;
        private int frequency;
        private Bitmap surfaceImage;
        private CachedBitmap surfaceCache;
        private KrentoMenu popupMenu;
        private Pen pulsarPen;
        SmoothingMode smoothingMode = SmoothingMode.HighQuality;
        CompositingQuality compositingQuality = CompositingQuality.HighQuality;
        InterpolationMode interpolationMode = GlobalConfig.InterpolationMode;


        public Pulsar()
            : base()
        {
            Name = "Pulsar";
            pulsarPen = new Pen(Brushes.Pink, 2.0f);
            frequency = 100;
            TopMostWindow = true;
            surfaceImage = NativeThemeManager.LoadBitmap("Pulsar.png");
            surfaceImage = BitmapPainter.ResizeBitmap(surfaceImage, Dimension, Dimension, true);
            surfaceCache = new CachedBitmap(surfaceImage, this.Canvas);
            Text = "Krento Pulsar";
            VerticalShift = -1;
            HorizontalShift = 1;
        }

        public int HorizontalShift { get; set; }

        public int VerticalShift { get; set; }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (surfaceCache != null)
            {
                surfaceCache.Dispose();
                surfaceCache = null;
            }
            if (surfaceImage != null)
                surfaceCache = new CachedBitmap(surfaceImage, this.Canvas);
            else
                surfaceCache = null;

            if (Canvas != null)
            {
                Canvas.SmoothingMode = smoothingMode;
                Canvas.CompositingQuality = compositingQuality;
                Canvas.InterpolationMode = interpolationMode;
            }

        }

        public void Run()
        {
            Size(Dimension, Dimension);
            DrawSurface();
            mouseLeft = true;
            StartTimer(pulseTimer, frequency);
            StartTimer(mouseTimer, timerLap);
        }

        public static int Dimension
        {
            get { return 64; }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                StopTimer(pulseTimer);
                StopTimer(mouseTimer);
                if (disposing)
                {
                    if (surfaceCache != null)
                    {
                        surfaceCache.Delete();
                        surfaceCache = null;
                    }

                    if (surfaceImage != null)
                    {
                        surfaceImage.Dispose();
                        surfaceImage = null;
                    }

                    if (pulsarPen != null)
                    {
                        pulsarPen.Dispose();
                        pulsarPen = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public KrentoMenu PopupMenu
        {
            get { return popupMenu; }
            set { popupMenu = value; }
        }

        public void MoveBottomRight()
        {
            Top = (int)(PrimaryScreen.Bounds.Bottom - Height);
            Left = (int)(PrimaryScreen.Bounds.Right - Width);

        }

        public int Frequency
        {
            get { return frequency; }
            set
            {
                frequency = value;
                StopTimer(pulseTimer);
                StartTimer(pulseTimer, frequency);
            }
        }

        public void ActivatePulsar()
        {
            StopTimer(mouseTimer);
            mouseLeft = false;
            StartTimer(mouseTimer, timerLap);
        }

        protected override void OnHides(EventArgs e)
        {
            base.OnHides(e);
            StopTimer(pulseTimer);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartTimer(pulseTimer, frequency);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            ActivatePulsar();
            NativeMethods.NotifyWinEvent(3, this.Handle, 0, 0);
            base.OnMouseEnter(e);
        }

        public void DeactivatePulsar()
        {
            StopTimer(mouseTimer);
            mouseLeft = true;
            StartTimer(mouseTimer, timerLap);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            DeactivatePulsar();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Right)
            {
                if (popupMenu != null)
                {
                    POINT pt = new POINT();
                    pt.x = e.X;
                    pt.y = e.Y;
                    NativeMethods.ClientToScreen(this.Handle, ref pt);
                    popupMenu.PopupAt(pt.x, pt.y);
                }
            }
        }

        protected override void  HandleTimerTick(int timerNumber)
        {
            switch (timerNumber)
            {
                case pulseTimer:
                    {
                        if (Visible)
                        {
                            DrawSurface();
                            Update();
                            if ((Alpha >= 230) && (mouseLeft))
                                StartTimer(mouseTimer, timerLap);
                        }
                        break;
                    }
                case mouseTimer:
                    {
                        if (mouseLeft)
                        {
                            Alpha -= 10;
                            if (Alpha <= 60)
                            {
                                StopTimer(mouseTimer);
                            }
                            Update();
                        }
                        else
                        {
                            Alpha += 10;
                            if (Alpha >= 230)
                            {
                                StopTimer(mouseTimer);
                            }
                            Update();
                        }
                        break;
                    }
            }
        }


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SYSCOMMAND)
            {
                if (m.WParam == NativeMethods.SC_CLOSE)
                    return;
            }
            base.WndProc(ref m);
        }

        public override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (e == null)
                return;
            DragDropEffects allowed = e.AllowedEffect;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((allowed & DragDropEffects.Link) == DragDropEffects.Link)
                {

                    e.Effect = DragDropEffects.Link;
                }
                else
                    if ((allowed & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {

                        e.Effect = DragDropEffects.Copy;
                    }
            }
            else if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_SHELLURL))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        internal void DrawSurface()
        {
            StopTimer(pulseTimer);

            try
            {
                Clear();


                if (surfaceCache != null)
                    surfaceCache.Draw();

                Canvas.DrawEllipse(pulsarPen, (Width - rad) / 2 - 1 + HorizontalShift, (Width - rad) / 2 + 1 + VerticalShift, rad, rad);

                rad += 2;
                if (rad >= 25)
                    rad = 0;
                StartTimer(pulseTimer, frequency);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("Pulsar.DrawSurface: " + ex.Message);
            }
        }
    }
}
