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
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using Laugris.Sage;

namespace Krento
{
    internal sealed class SplashWindow : LayeredWindow
    {
        private string versionInfo;
        private const int DelayTimer = 3;

        private Bitmap background;

        private int bmpHeight;
        private int bmpWidth;

        public SplashWindow()
            : base()
        {

            Text = "Krento Splash Screen";
            if (!string.IsNullOrEmpty(GlobalSettings.SplashName))
            {
                if (FileOperations.FileExists(GlobalSettings.SplashName))
                    background = FastBitmap.FromFile(FileOperations.StripFileName(GlobalSettings.SplashName));
            }

            if (background == null)
                background = NativeThemeManager.LoadBitmap("WindowKrentoSplash.png");


            bmpHeight = background.Height;
            bmpWidth = background.Width;

            this.Width = background.Width + 16;
            this.Height = background.Height + 16;

            this.Alpha = 240;
            this.ColorKey = ColorUtils.WhiteKey;

            StringBuilder sb = new StringBuilder("Version ");
            sb.Append(Application.ProductVersion);
#if PORTABLE
            sb.Append(" Portable");
#endif
            if (NativeMethods.IsNativeWin64())
                sb.Append("  x64");
            else
                sb.Append("  x86");

            versionInfo = sb.ToString();


            using (Graphics g = Graphics.FromImage(background))
            {
                using (Font versionFont = new Font("Tahoma", 11.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    TextPainter.DrawStringHalo(g, versionInfo, versionFont, -16, 120, bmpWidth, 50, Color.White, true);
                }
            }


            this.Canvas.DrawImage(background, 8, 8, bmpWidth, bmpHeight);

            this.Update();

        }

        protected override void OnTimerTick(TimerEventArgs e)
        {
            base.OnTimerTick(e);
            if (e.TimerId == DelayTimer)
            {
                StopTimer(DelayTimer);
                StartFade(300);
            }
        }

        public void Shutdown(int delayDuration)
        {
            StartTimer(DelayTimer, delayDuration);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (background != null)
                    {
                        background.Dispose();
                        background = null;
                    }
                }
                versionInfo = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

    }
}
