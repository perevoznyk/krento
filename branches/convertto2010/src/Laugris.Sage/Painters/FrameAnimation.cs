using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    public sealed class FrameAnimation : IDisposable
    {
        private LayeredWindow window;
        private int iFrame;
        private Thread updateThread;
        private string imageName;
        private Bitmap imageMain;
        private int nFrames;
        private Image[] images;
        Image imageNext = null;

        public FrameAnimation(LayeredWindow window)
        {
            this.window = window;
            this.Duration = 50;
        }

        ~FrameAnimation()
        {
            Dispose(false);
        }

        public bool Terminated { get; set; }

        private void EndUpdateThread()
        {
            Terminated = true;
            //if (updateThread != null)
            //{
            //    if (updateThread.IsAlive)
            //        updateThread.Abort();
            //}
        }

        private void StartUpdateThread()
        {
            Terminated = false;
            updateThread = new Thread(new ThreadStart(UpdateImage));
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        public int Duration { get; set; }

        private void UpdateImage()
        {
            while ((imageNext = NextFrameImage()) != null)
            {
                if (Terminated)
                    break;
                NativeMethods.SendMessage(new HandleRef(this, window.Handle), NativeMethods.CN_PAINT, IntPtr.Zero, IntPtr.Zero);
                NativeMethods.Sleep(Duration);
            }

            NativeMethods.PostMessage(window.Handle, NativeMethods.CN_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public Image ImageNext
        {
            get { return imageNext; }
        }

        private void CreateFrames()
        {
            ReleaseFrames();

            int iWidth = imageMain.Width;
            int iHeight = imageMain.Height;
            int frameSize;

            if ((iWidth == 0) || (iHeight == 0))
                return;

            if (iWidth > iHeight)
            {
                nFrames = ((int)(iWidth / iHeight));
                frameSize = iHeight;
            }
            else
            {
                nFrames = ((int)(iHeight / iWidth));
                frameSize = iWidth;
            }

            iFrame = 0;

            window.Size(frameSize, frameSize);

            images = new Image[nFrames];

            for (int i = 0; i < nFrames; i++)
            {
                if (iWidth > iHeight)
                    images[i] = imageMain.Clone(new Rectangle((i * frameSize), 0, frameSize, frameSize), PixelFormat.DontCare);
                else
                    images[i] = imageMain.Clone(new Rectangle(0, (i * frameSize), frameSize, frameSize), PixelFormat.DontCare);
            }

        }

        private void ReleaseFrames()
        {
            if (images == null)
            {
                nFrames = 0;
                iFrame = 0;
                return;
            }

            int i;
            for (i = 0; i < nFrames; i++)
            {
                if (images[i] != null)
                    images[i].Dispose();
                images[i] = null;
            }

            images = null;
            nFrames = 0;
        }

        private void ReleaseImage()
        {
            EndUpdateThread();
            ReleaseFrames();

            if (imageMain != null)
                imageMain.Dispose();
            imageMain = null;
        }

        private void SetImage(string image)
        {
            ReleaseImage();
            this.imageName = image;
            imageMain = FastBitmap.FromFile(image);
            CreateFrames();
            StartUpdateThread();
        }

        private void SetImage(Bitmap image)
        {
            ReleaseImage();
            this.imageMain = image;
            CreateFrames();
            StartUpdateThread();
        }

        public bool Cycle
        {
            get;
            set;
        }

        private Image NextFrameImage()
        {
            if ((images == null) || (nFrames <= 0))
                return null;

            if (!Cycle)
            {
                if (iFrame == nFrames)
                    return null;
            }

            Image imageReturn = images[iFrame];

            iFrame++;

            if (Cycle)
                iFrame %= nFrames;

            return imageReturn;
        }

        public int CurrentFrameNumber
        {
            get { return iFrame; }
        }

        public string ImageName
        {
            get { return imageName; }
            set { SetImage(value); }
        }

        public Bitmap Image
        {
            get { return imageMain; }
            set { SetImage(value); }
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            ReleaseImage();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
