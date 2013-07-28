//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Laugris.Sage;

namespace Krento.Toys
{
    /// <summary>
    /// Krento about window
    /// </summary>
    internal sealed class AboutWindow : LayeredWindow
    {
        private Image background;
        private const int SparkeTimer = 2;
        private double count = 2;
        private double gravity = 0.5;
        private MinMaxValue dotSize = new MinMaxValue(1.0, 3.0);
        private Velocity velocity = new Velocity(5, 5);
        private List<MagicSparkle> Children = new List<MagicSparkle>();
        private Image toyIcon;

        public string Version {get; set;}

        public string AboutText { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        private string bigIcon;


        public string BigIcon
        {
            get { return bigIcon; }
            set
            {
                bigIcon = value;
                if (!string.IsNullOrEmpty(bigIcon))
                {
                    if (FileOperations.FileExists(bigIcon))
                    {
                        if (toyIcon != null)
                        {
                            toyIcon.Dispose();
                            toyIcon = null;
                        }

                        try
                        {
                            toyIcon = Image.FromFile(bigIcon);
                        }
                        catch
                        {
                            toyIcon = NativeThemeManager.Load("Toys.png");
                        }

                    }
                }

                if (toyIcon == null)
                    toyIcon = NativeThemeManager.Load("Toys.png");

                toyIcon = BitmapPainter.ResizeBitmap(toyIcon, 64, 64, true);
            }
        }


        public AboutWindow()
            : base()
        {
            Text = "About Krento";
            Name = "AboutWindow";
            background = NativeThemeManager.LoadBitmap("WindowBackground.png");
            this.CustomPaint = true;
            this.Width = background.Width + 16;
            this.Height = background.Height + 16;
            this.Alpha = 230;
            this.ColorKey = ColorUtils.WhiteKey;
            this.Repaint();
        }


        private void DrawVersionCopyright()
        {
            using (Font transFont = new Font("Tahoma", 8.0f, FontStyle.Bold, GraphicsUnit.Point))
            {
                TextPainter.DrawString(Canvas, Version, transFont, 96, 154, 400, 400, Color.White, false);
                TextPainter.DrawString(Canvas, Copyright, transFont, 96, 240, 400, 400, Color.White, false);

            }
        }

        protected override void OnHides(EventArgs e)
        {
            StopTimer(SparkeTimer);
            base.OnHides(e);
        }

        protected override void OnShown(EventArgs e)
        {
            StartTimer(SparkeTimer, 42);
            base.OnShown(e);
        }

        protected override void OnPainting(System.Windows.Forms.PaintEventArgs e)
        {

            try
            {
                base.OnPainting(e);
                Canvas.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                this.Canvas.DrawImage(background, 8, 8, background.Width, background.Height);
                if (toyIcon != null)
                {
                    Canvas.DrawImage(toyIcon, 24, 50, 64, 64);
                }
                DrawToyName();
                TextPainter.DrawStringHalo(Canvas, Author, TextPainter.DefaultFont, 96, 128, 400, 400, Color.White, false);
                DrawVersionCopyright();
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                for (int cnt = 0; cnt < Children.Count; cnt++)
                {
                    Children[cnt].Paint(e.Graphics);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Draws the name of the toy.
        /// </summary>
        private void DrawToyName()
        {
            if (string.IsNullOrEmpty(AboutText))
                AboutText = "Krento Toy";

            using (Font aboutFont = new Font("Tahoma", 12.0f, FontStyle.Bold, GraphicsUnit.Point))
            {
                TextPainter.DrawString(Canvas, AboutText, aboutFont, 96, 40, 275, 400, Color.White, false);
            }

            using (Font descFont = new Font("Tahoma", 8.0f, FontStyle.Bold, GraphicsUnit.Point))
            {
                TextPainter.DrawString(Canvas, Description, descFont, 96, 70, 275, 120, Color.White, false, true);
            }

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                StopTimer(SparkeTimer);
                Children.Clear();
                if (disposing)
                {
                    if (background != null)
                    {
                        background.Dispose();
                        background = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:TimerTick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Karna.Windows.UI.TimerEventArgs"/> instance containing the event data.</param>
        protected override void OnTimerTick(TimerEventArgs e)
        {
            base.OnTimerTick(e);
            if (e.TimerId == SparkeTimer)
            {
                if (this.Visible)
                    ProcessSparkleTimer();
            }
        }

        private void ProcessSparkleTimer()
        {
            MoveChildren();
            Repaint();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            AddChildren(e.X, e.Y);
        }

        private void MoveChildren()
        {
            int cnt = Children.Count;
            double halfSize;
            MagicSparkle sparkle;

            for (int i = cnt - 1; i >= 0; i--)
            {
                try
                {
                    sparkle = Children[i];
                }
                catch
                {
                    continue;
                }
                sparkle.Run();
                if (sparkle.Opacity <= 0.1)
                {
                    Children.Remove(sparkle);
                }
                halfSize = sparkle.TotalSize / 2;
                if ((sparkle.X < halfSize) ||
                   (sparkle.Y < halfSize) ||
                   (sparkle.Y > this.Height - halfSize) ||
                   (sparkle.X > this.Width - halfSize))
                {
                    Children.Remove(sparkle);
                }
            }
        }

        private void AddChildren(double x, double y)
        {
            int seed = (int)DateTime.Now.Ticks;

            for (int i = 0; i < this.count; i++)
            {
                seed += (int)DateTime.Now.Ticks;
                Random r = new Random(seed);
                double size = dotSize.MinValue + (dotSize.MaxValue - dotSize.MinValue) * r.NextDouble();
                byte red = (byte)(128 + (128 * r.NextDouble()));
                byte green = (byte)(128 + (128 * r.NextDouble()));
                byte blue = (byte)(128 + (128 * r.NextDouble()));

                double xVelocity = velocity.X - 2 * velocity.X * r.NextDouble();
                double yVelocity = -velocity.Y * r.NextDouble();

                MagicSparkle sparkle = new MagicSparkle(red, green, blue, size);
                sparkle.X = x;
                sparkle.Y = y;
                sparkle.XVelocity = xVelocity;
                sparkle.YVelocity = yVelocity;
                sparkle.Gravity = gravity;
                sparkle.Run();
                Children.Add(sparkle);

                Children.Add(sparkle);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case NativeMethods.WM_ACTIVATEAPP:
                    if (IntPtr.Zero == m.WParam)
                    {
                        Hide();
                    }
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    Hide();
                    m.Result = (IntPtr)1;
                    return;


            }
            base.WndProc(ref m);
        }

        private static class NativeMethods
        {
            public const int WM_ACTIVATEAPP = 0x001C;
            public const int WM_KILLFOCUS = 8;
        }
    }
}
