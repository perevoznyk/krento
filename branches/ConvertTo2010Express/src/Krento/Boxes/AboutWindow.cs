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
using Krento.RollingStones;
using Laugris.Sage;

namespace Krento
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
        private string versionInfo;
        private string translatorName;
        private string translator;
        private RollingStoneBase stone;
        private Bitmap stoneIcon;
        private string description;
        private string author;
        private string copyright;

        public AboutWindow()
            : base()
        {

            this.Text = "About Krento";

            if (!string.IsNullOrEmpty(GlobalSettings.AboutBoxName))
            {
                if (FileOperations.FileExists(GlobalSettings.AboutBoxName))
                {
                    background = FastBitmap.FromFile(FileOperations.StripFileName(GlobalSettings.AboutBoxName));
                }
            }

            if (background == null)
                background = NativeThemeManager.LoadBitmap("WindowKrentoAbout.png");

            this.Width = background.Width + 16;
            this.Height = background.Height + 16;
            this.Alpha = 230;
            this.ColorKey = ColorUtils.WhiteKey;
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            this.Canvas.DrawImage(background, 8, 8, background.Width, background.Height);
            if (!TextHelper.SameText(Language.Culture.Name, "en-US"))
            {
                translatorName = SR.GetCaption("Translator");
                if (TextHelper.SameText(translatorName, "Put your name here"))
                    translatorName = "";
            }
            if (!string.IsNullOrEmpty(translatorName))
                translator = "Translator: " + translatorName;
            versionInfo = "Version " + Application.ProductVersion;

            this.Repaint();
        }

        public AboutWindow(RollingStoneBase stone)
            : base()
        {

            this.stone = stone;

            if (!string.IsNullOrEmpty(GlobalSettings.AboutStoneName))
            {
                if (FileOperations.FileExists(GlobalSettings.AboutStoneName))
                    background = FastBitmap.FromFile(FileOperations.StripFileName(GlobalSettings.AboutStoneName));
            }

            if (background == null)
                background = NativeThemeManager.LoadBitmap("WindowBackground.png");


            this.Width = background.Width + 16;
            this.Height = background.Height + 16;
            this.Alpha = 230;
            this.ColorKey = ColorUtils.WhiteKey;
            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            this.Canvas.DrawImage(background, 8, 8, background.Width, background.Height);

            if (!string.IsNullOrEmpty(stone.StoneAuthor))
                this.author = "Author: " + stone.StoneAuthor;
            this.description = stone.StoneDescription;
            this.stoneIcon = stone.StoneIcon;

            versionInfo = "Version: " + stone.StoneVersion;
            copyright = stone.StoneCopyright;
        }


        private void DrawTranslatorName()
        {
            if (string.IsNullOrEmpty(translator))
                return;

            using (Font transFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                TextPainter.DrawStringHalo(Canvas, translator, transFont, 26, 110, background.Width - 16, 50, Color.White, true);
            }
        }

        private void DrawDesignerName()
        {
            int y;

            if (string.IsNullOrEmpty(translator))
                y = 110;
            else
                y = 130;

            using (Font transFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                TextPainter.DrawStringHalo(Canvas, "Design: Stan Ragets", transFont, 26, y, background.Width - 16, 50, Color.White, true);
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

        private void DrawStoneName()
        {
            if (string.IsNullOrEmpty(description))
                description = "Krento Stone";

            using (Font aboutFont = new Font("Tahoma", 16.0f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                TextPainter.DrawString(Canvas, description, aboutFont, 96, 40, 400, 400, Color.White, false);
            }

            if (stone != null)
                if (stone.StoneBuiltIn)
                {
                    using (Font descFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        TextPainter.DrawString(Canvas, "Built In Krento Stone", descFont, 96, 70, 400, 400, Color.White, false);
                    }
                }
                else
                {
                    using (Font descFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        TextPainter.DrawString(Canvas, "External Krento Stone", descFont, 96, 70, 400, 400, Color.White, false);
                    }
                }


        }

        private void DrawVersionCopyright()
        {
            using (Font transFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                TextPainter.DrawString(Canvas, versionInfo, transFont, 96, 154, 400, 400, Color.White, false);
                TextPainter.DrawString(Canvas, copyright, transFont, 96, 240, 400, 400, Color.White, false);

            }
        }

        protected override void Draw()
        {

            Canvas.SmoothingMode = SmoothingMode.HighQuality;
            this.Canvas.DrawImage(background, 8, 8, background.Width, background.Height);
            if (stone == null)
            {
                using (Font transFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    TextPainter.DrawStringHalo(Canvas, versionInfo, transFont, 26, 80, background.Width - 16, 50, Color.White, true);
                }
                DrawTranslatorName();
                DrawDesignerName();
            }
            else
            {
                if (stoneIcon != null)
                {
                    Canvas.DrawImage(stoneIcon, 24, 50, 64, 64);
                }
                DrawStoneName();
                using (Font transFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    TextPainter.DrawStringHalo(Canvas, author, transFont, 96, 128, 400, 400, Color.White, false);
                }
                DrawVersionCopyright();

            }
            Canvas.SmoothingMode = SmoothingMode.AntiAlias;
            for (int cnt = 0; cnt < Children.Count; cnt++)
            {
                Children[cnt].Paint(Canvas);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                StopTimer(SparkeTimer);
                for (int i = 0; i < Children.Count; i++)
                    Children[i].Dispose();
                Children.Clear();
                if (disposing)
                {
                    if (background != null)
                    {
                        background.Dispose();
                        background = null;
                    }

                    if (stoneIcon != null)
                    {
                        stoneIcon.Dispose();
                        stoneIcon = null;
                    }

                    versionInfo = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

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
                    sparkle.Dispose();
                }
                else
                {
                    halfSize = sparkle.TotalSize / 2;
                    if ((sparkle.X < halfSize) ||
                       (sparkle.Y < halfSize) ||
                       (sparkle.Y > this.Height - halfSize) ||
                       (sparkle.X > this.Width - halfSize))
                    {
                        Children.Remove(sparkle);
                        sparkle.Dispose();
                    }
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

    }
}
