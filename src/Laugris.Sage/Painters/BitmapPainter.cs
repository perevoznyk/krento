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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    public enum Location
    {
        Bottom,
        Left,
        Right,
        Top,
        All
    }
    /// <summary>
    /// Performs different operations with Bitmap 
    /// </summary>
    public static class BitmapPainter
    {
        #region internal matrix
        private static float[][] colorMatrixDefault = { 
            new float[] {1,  0,  0,  0,   0},
            new float[] {0,  1,  0,  0,   0},
            new float[] {0,  0,  1,  0,   0},
            new float[] {0,  0,  0,  1,   0},
            new float[] {0,  0,  0,  0,   1}};

        private static float[][] brightneesMatrixDefault = { 
            new float[] {1,  0,  0,  0,   0},
            new float[] {0,  1,  0,  0,   0},
            new float[] {0,  0,  1,  0,   0},
            new float[] {0,  0,  0,  1,   0},
            new float[] {-0.4f,  -0.4f,  -0.4f,  0,   1}};

        private static float[][] lightMatrixDefault = { 
            new float[] {1,  0,  0,  0,   0},
            new float[] {0,  1,  0,  0,   0},
            new float[] {0,  0,  1,  0,   0},
            new float[] {0,  0,  0,  1,   0},
            new float[] {-0.2f,  -0.2f,  -0.2f,  0,   1}};
        #endregion

        private static ColorMatrix colorMatrix = new ColorMatrix(colorMatrixDefault);
        private static ColorMatrix brightneesMatrix = new ColorMatrix(brightneesMatrixDefault);
        private static ImageAttributes brightAttributes = new ImageAttributes();
        private static ColorMatrix lightMatrix = new ColorMatrix(lightMatrixDefault);
        private static ImageAttributes lightAttributes = new ImageAttributes();

        static BitmapPainter()
        {
            brightAttributes.SetColorMatrix(brightneesMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            lightAttributes.SetColorMatrix(lightMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }


        public static Size AdaptProportionalSize(
                Size szMax,
                Size szReal)
        {
            int nWidth;
            int nHeight;
            double sMaxRatio;
            double sRealRatio;

            if (szMax.Width < 1 || szMax.Height < 1 || szReal.Width < 1 || szReal.Height < 1)
                return Size.Empty;

            sMaxRatio = (double)szMax.Width / (double)szMax.Height;
            sRealRatio = (double)szReal.Width / (double)szReal.Height;

            if (sMaxRatio < sRealRatio)
            {
                nWidth = Math.Min(szMax.Width, szReal.Width);
                nHeight = (int)Math.Round(nWidth / sRealRatio);
            }
            else
            {
                nHeight = Math.Min(szMax.Height, szReal.Height);
                nWidth = (int)Math.Round(nHeight * sRealRatio);
            }

            return new Size(nWidth, nHeight);
        }


        public static Size FillProportionalSize(
                Size szMax,
                Size szReal)
        {
            int nWidth;
            int nHeight;

            if (szMax.Width < 1 || szMax.Height < 1 || szReal.Width < 1 || szReal.Height < 1)
                return Size.Empty;

            double sMaxRatio = (double)szMax.Width / (double)szMax.Height;
            double sRealRatio = (double)szReal.Width / (double)szReal.Height;

            if (sMaxRatio > sRealRatio)
            {
                nWidth = Math.Min(szMax.Width, szReal.Width);
                nHeight = (int)Math.Round(nWidth / sRealRatio);
            }
            else
            {
                nHeight = Math.Min(szMax.Height, szReal.Height);
                nWidth = (int)Math.Round(nHeight * sRealRatio);
            }

            return new Size(nWidth, nHeight);
        }
        
        #region ResizeBitmap
        /// <summary>
        /// Resizes the bitmap.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Image bmp, int newWidth, int newHeight)
        {
            return ResizeBitmap(bmp, newWidth, newHeight, true);
        }

        public static Bitmap ResizeBitmap(Image bmp, int newWidth, int newHeight, bool disposeOld)
        {
            if ((bmp == null) || (newWidth <= 0) || (newHeight <= 0))
                return null;

            Bitmap result = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppPArgb);
            using (Graphics canvas = Graphics.FromImage(result))
            {
                canvas.InterpolationMode = InterpolationMode.High;
                canvas.DrawImage(bmp, 0, 0, newWidth, newHeight);
            }
            if (disposeOld)
            {
                bmp.Dispose();
                bmp = null;
            }
            return result;
        }

        public static Bitmap ResizeBitmap(Image bmp, int newWidth, int newHeight, InterpolationMode interpolationMode)
        {
            return ResizeBitmap(bmp, newWidth, newHeight, true, interpolationMode);
        }

        public static Bitmap ResizeBitmap(Image bmp, int newWidth, int newHeight, bool disposeOld, InterpolationMode interpolationMode)
        {
            if ((bmp == null) || (newWidth <= 0) || (newHeight <= 0))
                return null;

            Bitmap result = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppPArgb);
            using (Graphics canvas = Graphics.FromImage(result))
            {
                canvas.InterpolationMode = interpolationMode;
                canvas.DrawImage(bmp, 0, 0, newWidth, newHeight);
            }
            if (disposeOld)
            {
                bmp.Dispose();
                bmp = null;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Builds the image from skin.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <param name="bWidth">Width of the skin.</param>
        /// <param name="bHeight">Height of the skin.</param>
        /// <param name="skinMargin">The skin margins.</param>
        /// <returns></returns>
        public static Bitmap BuildImageFromSkin(Bitmap original, int bWidth, int bHeight, SkinOffset skinMargin)
        {
            Bitmap result = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingMode = CompositingMode.SourceCopy;

                //Draw left part of the image
                g.DrawImage(original, new RectangleF(0, 0, skinMargin.Left, skinMargin.Top), new RectangleF(-0.5f, -0.5f, skinMargin.Left, skinMargin.Top), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(0, skinMargin.Top, skinMargin.Left, bHeight - skinMargin.Top - skinMargin.Bottom), new RectangleF(-0.5f, skinMargin.Top - 0.5f, skinMargin.Left, original.Height - skinMargin.Top - skinMargin.Bottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(0, bHeight - skinMargin.Bottom, skinMargin.Left, skinMargin.Bottom), new RectangleF(-0.5f, original.Height - skinMargin.Bottom - 0.5f, skinMargin.Left, skinMargin.Bottom), GraphicsUnit.Pixel);

                //Draw central part of the image
                g.DrawImage(original, new RectangleF(skinMargin.Left, 0, bWidth - skinMargin.Left - skinMargin.Right, skinMargin.Top), new RectangleF(skinMargin.Left - 0.5f, -0.5f, original.Width - skinMargin.Left - skinMargin.Right, skinMargin.Top), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(skinMargin.Left, skinMargin.Top, bWidth - skinMargin.Left - skinMargin.Right, bHeight - skinMargin.Top - skinMargin.Bottom), new RectangleF(skinMargin.Left - 0.5f, skinMargin.Top - 0.5f, original.Width - skinMargin.Left - skinMargin.Right, original.Height - skinMargin.Top - skinMargin.Bottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(skinMargin.Left, bHeight - skinMargin.Bottom, bWidth - skinMargin.Left - skinMargin.Right, skinMargin.Bottom), new RectangleF(skinMargin.Left - 0.5f, original.Height - skinMargin.Bottom - 0.5f, original.Width - skinMargin.Left - skinMargin.Right, skinMargin.Bottom), GraphicsUnit.Pixel);

                //Draw right part of the image
                g.DrawImage(original, new RectangleF(bWidth - skinMargin.Right, 0, skinMargin.Right, skinMargin.Top), new RectangleF(original.Width - skinMargin.Right - 0.5f, -0.5f, skinMargin.Right, skinMargin.Top), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(bWidth - skinMargin.Right, skinMargin.Top, skinMargin.Right, bHeight - skinMargin.Top - skinMargin.Bottom), new RectangleF(original.Width - skinMargin.Right - 0.5f, skinMargin.Top - 0.5f, skinMargin.Right, original.Height - skinMargin.Top - skinMargin.Bottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new RectangleF(bWidth - skinMargin.Right, bHeight - skinMargin.Bottom, skinMargin.Right, skinMargin.Bottom), new RectangleF(original.Width - skinMargin.Right - 0.5f, original.Height - skinMargin.Bottom - 0.5f, skinMargin.Right, skinMargin.Bottom), GraphicsUnit.Pixel);
            }

            return result;
        }

        
        /// <summary>
        /// Draws the Windows native bitmap on Windows native bitmap
        /// </summary>
        /// <param name="dst">The destination Windows HBITMAP.</param>
        /// <param name="src">The source Windows HBITMAP</param>
        /// <param name="width">The bitmap width.</param>
        /// <param name="height">The bitmap height.</param>
        public static void DrawNativeBitmap(IntPtr dst, IntPtr src, int width, int height)
        {
            NativeMethods.DrawNativeBitmap(src, dst, width, height);
        }

        /// <summary>
        /// Performs native GDI image stretching
        /// </summary>
        /// <param name="src">The source Bitmap.</param>
        /// <param name="dst">The destination bitmap.</param>
        public static void StretchDraw(Bitmap src, ref Bitmap dst)
        {
            if ((src == null) || (dst == null))
                return;

            int dstWidth = dst.Width;
            int dstHeight = dst.Height;

            IntPtr srcBitmap = src.GetHbitmap();
            IntPtr dstBitmap = dst.GetHbitmap();

            if (NativeMethods.StretchNativeBitmap(srcBitmap, dstBitmap, src.Width, src.Height, dstWidth, dstHeight))
            {
                dst.Dispose();
                dst = Bitmap.FromHbitmap(dstBitmap);
            }

            NativeMethods.DeleteObject(srcBitmap);
            NativeMethods.DeleteObject(dstBitmap);
        }

        /// <summary>
        /// Converts Bitmap image to 32 bit colors Bitmap with premultiplied alpha
        /// </summary>
        /// <param name="realImage">The source image.</param>
        /// <returns></returns>
        public static Bitmap ConvertToRealColors(Image realImage, bool disposeOld)
        {
            if (realImage == null)
                return null;

            int w = realImage.Width;
            int h = realImage.Height;

            Bitmap result = new Bitmap(w, h, PixelFormat.Format32bppPArgb);
            using (Graphics canvas = Graphics.FromImage(result))
            {
                canvas.InterpolationMode = InterpolationMode.High;
                canvas.DrawImageUnscaled(realImage, 0, 0, w, h);
            }
            if (disposeOld)
            {
                realImage.Dispose();
            }

            return result;
        }


        public static Bitmap ConvertTo32Bit(Image realImage, bool disposeOld)
        {
            if (realImage == null)
                return null;

            int w = realImage.Width;
            int h = realImage.Height;

            Bitmap result = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics canvas = Graphics.FromImage(result))
            {
                canvas.InterpolationMode = InterpolationMode.High;
                canvas.DrawImageUnscaled(realImage, 0, 0, w, h);
            }
            if (disposeOld)
            {
                realImage.Dispose();
            }

            return result;
        }

        public static IntPtr GetCompatibleBitmap(Bitmap bm)
        {
            IntPtr hbitmap = bm.GetHbitmap();
            IntPtr result = NativeMethods.MakeCompatibleBitmap(hbitmap, bm.Width, bm.Height);
            NativeMethods.DeleteObject(hbitmap);
            return result;
        }

        /// <summary>
        /// Creates the reflection image.
        /// </summary>
        /// <param name="realImage">The real image.</param>
        /// <param name="reflectionDepth">The reflection depth.</param>
        /// <returns></returns>
        public unsafe static Image CreateReflectionImage(Image realImage, int reflectionDepth)
        {
            int w;
            int h;
            float opaque;
            int alpha;
            byte newAlpha;
            float frac;
            byte b;
            byte r;
            byte g;

            if (realImage == null)
                return null;
            if (reflectionDepth <= 0)
                return realImage;

            Bitmap result = new Bitmap(realImage.Width, realImage.Height + reflectionDepth, PixelFormat.Format32bppPArgb);
            Graphics canvas = Graphics.FromImage(result);
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap rotatedImage = new Bitmap(realImage);

            w = realImage.Width;
            h = realImage.Height;

            rotatedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Bitmap reflectionImage = new Bitmap(w, h, PixelFormat.Format32bppPArgb);

            h = reflectionDepth;

            BitmapData srcData = rotatedImage.LockBits(new Rectangle(0, 0, rotatedImage.Width, rotatedImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            BitmapData destData = reflectionImage.LockBits(new Rectangle(0, 0, reflectionImage.Width, reflectionImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);

            byte* srcScan0 = (byte*)srcData.Scan0.ToPointer();
            byte* destScan0 = (byte*)destData.Scan0.ToPointer();

            byte* ps = srcScan0;
            byte* pd = destScan0;

            for (int y = 0; y < h; y++)
            {
                opaque = ((255.0f / h) * (h - y)) - reflectionDepth;
                if (opaque < 0.0)
                    opaque = 0.0f;
                if (opaque > 255.0)
                    opaque = 255.0f;

                for (int x = 0; x < w; x++)
                {
                    b = *ps;
                    ps++;

                    g = *ps;
                    ps++;

                    r = *ps;
                    ps++;

                    alpha = *ps;
                    ps++;

                    if (alpha != 0)
                    {
                        frac = (float)(opaque / 255);
                        newAlpha = (byte)(frac * alpha);

                        *pd = b;
                        pd++;

                        *pd = g;
                        pd++;

                        *pd = r;
                        pd++;

                        *pd = (byte)newAlpha;
                        pd++;

                    }
                    else
                    {
                        pd += 4;
                    }
                }
            }


            rotatedImage.UnlockBits(srcData);
            reflectionImage.UnlockBits(destData);

            canvas.DrawImageUnscaled(reflectionImage, 0, reflectionImage.Height, 0, 0);
            canvas.DrawImage(realImage, new Rectangle(0, 0, realImage.Width, realImage.Height), 0, 0, realImage.Width, realImage.Height, GraphicsUnit.Pixel);

            rotatedImage.Dispose();
            rotatedImage = null;

            reflectionImage.Dispose();
            reflectionImage = null;

            canvas.Dispose();

            return result;
        }

        /// <summary>
        /// Changes the alpha.
        /// </summary>
        /// <param name="image">The BMP.</param>
        /// <param name="alpha">The alpha.</param>
        public unsafe static void ChangeAlpha(Bitmap image, byte alpha, bool keepTransparency)
        {
            if (image == null)
                return;

            BitmapData tmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            byte* srcScan0 = (byte*)tmpData.Scan0.ToPointer();
            byte* ps = srcScan0;
            byte srcAlpha;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    ps += 3;
                    srcAlpha = *ps;
                    if ((keepTransparency) && (srcAlpha == 0))
                        *ps = 0;
                    else
                        *ps = alpha;
                    ps++;
                }
            }

            image.UnlockBits(tmpData);
        }

        /// <summary>
        /// Multiplexes the alpha chanell of the image .
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="alpha">The alpha.</param>
        public unsafe static void MultiplexAlpha(Bitmap image, double alpha)
        {
            if (image == null)
                return;


            BitmapData tmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            try
            {
                byte* srcScan0 = (byte*)tmpData.Scan0.ToPointer();
                byte* ps = srcScan0;
                byte srcAlpha;

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        ps += 3;
                        srcAlpha = *ps;
                        *ps = (byte)(alpha * srcAlpha);
                        ps++;
                    }
                }
            }
            finally
            {
                image.UnlockBits(tmpData);
            }
        }

        public unsafe static void FadeOut(Bitmap image, int size, Location location)
        {
            if (image == null)
                return;

            int imgWidth = image.Width;
            int imgHeight = image.Height;
            double step = (double)(1.0 / size);
            double alpha = 0.0;

            BitmapData srcData = image.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            try
            {
                byte* srcScan0 = (byte*)srcData.Scan0.ToPointer();
                byte* ps = srcScan0;
                byte srcAlpha;

                switch (location)
                {
                    case Location.Bottom:
                        alpha = 1.0;
                        ps += (imgHeight - size) * imgWidth * 4;
                        for (int y = 0; y < size; y++)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                            }
                            alpha -= step;
                        }
                        break;
                    case Location.Left:
                        for (int y = 0; y < imgHeight; y++)
                        {
                            alpha = 0.0;
                            for (int x = 0; x < size; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                                alpha += step;
                            }
                            ps += (imgWidth - size) * 4;
                        }
                        break;
                    case Location.Right:
                        for (int y = 0; y < imgHeight; y++)
                        {
                            alpha = 1.0;
                            ps += (imgWidth - size) * 4;
                            for (int x = 0; x < size; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                                alpha -= step;
                            }
                        }
                        break;
                    case Location.Top:
                        for (int y = 0; y < size; y++)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                            }
                            alpha += step;
                        }
                        break;
                    case Location.All:
                        alpha = 0.0;
                        //top
                        for (int y = 0; y < size; y++)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                            }
                            alpha += step;
                        }

                        //bottom
                        ps = srcScan0;
                        alpha = 1.0;
                        ps += (imgHeight - size) * imgWidth * 4;
                        for (int y = 0; y < size; y++)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                            }
                            alpha -= step;
                        }

                        //left
                        ps = srcScan0;
                        for (int y = 0; y < imgHeight; y++)
                        {
                            alpha = 0.0;
                            for (int x = 0; x < size; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                                alpha += step;
                            }
                            ps += (imgWidth - size) * 4;
                        }

                        //right
                        ps = srcScan0;
                        for (int y = 0; y < imgHeight; y++)
                        {
                            alpha = 1.0;
                            ps += (imgWidth - size) * 4;
                            for (int x = 0; x < size; x++)
                            {
                                ps += 3;
                                srcAlpha = *ps;
                                *ps = (byte)(alpha * srcAlpha);
                                ps++;
                                alpha -= step;
                            }
                        }

                        break;
                    default:
                        break;
                }

            }
            finally
            {
                image.UnlockBits(srcData);
            }
        }

        #region DrawImageUnscaled
        /// <summary>
        /// Draws the image unscaled preserving Alpha channel
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">Drawing surface</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public static void DrawImageUnscaled(Image image, Graphics canvas, int left, int top)
        {
            if ((image == null) || (canvas == null))
                return;

            canvas.DrawImageUnscaled(image, left, top);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public static void DrawImageUnscaled(Bitmap image, IntPtr hdc, int left, int top)
        {
            if (image == null)
                return;
            DrawImageUnscaled(image, hdc, left, top, 255);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void DrawImageUnscaled(Image image, IntPtr hdc, int left, int top, int width, int height)
        {
            DrawImageUnscaled(image, hdc, left, top, width, height, 255);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="srcBitmap">The SRC bitmap.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        public static void DrawImageUnscaled(IntPtr srcBitmap, Graphics canvas, int left, int top, int imageWidth, int imageHeight)
        {
            DrawImageUnscaled(srcBitmap, canvas, left, top, imageWidth, imageHeight, 255);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="srcBitmap">The SRC bitmap.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        public static void DrawImageUnscaled(IntPtr srcBitmap, IntPtr hdc, int left, int top, int imageWidth, int imageHeight)
        {
            DrawImageUnscaled(srcBitmap, hdc, left, top, imageWidth, imageHeight, 255);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageUnscaled(Image image, Graphics canvas, int left, int top, byte alpha)
        {
            if (image == null)
                return;

            if (canvas == null)
                return;

            colorMatrix.Matrix33 = (float)(alpha / 255f);

            ImageAttributes imageAttributes = new ImageAttributes();
            try
            {
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                canvas.DrawImage(image, new Rectangle(left, top, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            finally
            {
                imageAttributes.Dispose();
                imageAttributes = null;
            }
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageUnscaled(Bitmap image, IntPtr hdc, int left, int top, byte alpha)
        {
            if (image == null)
                return;

            IntPtr srcBitmap = image.GetHbitmap(Color.FromArgb(0));
            NativeMethods.AlphaBlendBitmap(srcBitmap, hdc, left, top, image.Width, image.Height, alpha);
            NativeMethods.DeleteObject(srcBitmap);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageUnscaled(Image image, IntPtr hdc, int left, int top, int width, int height, byte alpha)
        {
            if (image == null)
                return;
            
            IntPtr srcBitmap = ((Bitmap)image).GetHbitmap(Color.FromArgb(0));
            NativeMethods.AlphaBlendBitmap(srcBitmap, hdc, left, top, width, height, alpha);
            NativeMethods.DeleteObject(srcBitmap);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="srcBitmap">The SRC bitmap.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageUnscaled(IntPtr srcBitmap, Graphics canvas, int left, int top, int imageWidth, int imageHeight, byte alpha)
        {
            if (canvas == null)
                return;

            IntPtr hdc = canvas.GetHdc();
            NativeMethods.AlphaBlendBitmap(srcBitmap, hdc, left, top, imageWidth, imageHeight, alpha);
            canvas.ReleaseHdc(hdc);
        }

        /// <summary>
        /// Draws the image unscaled.
        /// </summary>
        /// <param name="srcBitmap">The SRC bitmap.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageUnscaled(IntPtr srcBitmap, IntPtr hdc, int left, int top, int imageWidth, int imageHeight, byte alpha)
        {
            NativeMethods.AlphaBlendBitmap(srcBitmap, hdc, left, top, imageWidth, imageHeight, alpha);
        }
        #endregion

        #region DrawImageScaled
        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void DrawImageScaled(Image image, Graphics canvas, int left, int top, int width, int height)
        {
            if (image == null)
                return;
            if (canvas == null)
                return;
            canvas.DrawImage(image, left, top, width, height);
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void DrawImageScaled(Image image, IntPtr hdc, int left, int top, int width, int height)
        {
            if ((image == null) || (width == 0) || (height == 0))
                return;
            Bitmap tmpBmp = ResizeBitmap(image, width, height);
            IntPtr srcBitmap = tmpBmp.GetHbitmap(Color.FromArgb(0));
            tmpBmp.Dispose();
            tmpBmp = null;

            NativeMethods.AlphaBlendBitmap(srcBitmap, hdc, left, top, width, height, 0xFF);
            NativeMethods.DeleteObject(srcBitmap);
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void DrawImageScaled(IntPtr sourceBitmap, Graphics canvas, int left, int top, int imageWidth, int imageHeight, int width, int height)
        {
            if (canvas == null)
                return;

            IntPtr screenDC;
            IntPtr oldSrc;

            IntPtr hdc = canvas.GetHdc();
            screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr srcDC = NativeMethods.CreateCompatibleDC(screenDC);
            oldSrc = NativeMethods.SelectObject(srcDC, sourceBitmap);

            try
            {

                NativeMethods.AlphaBlendNative(hdc, left, top, width, height, srcDC,
                    0, 0, imageWidth, imageHeight, 0xFF);

            }
            finally
            {
                canvas.ReleaseHdc(hdc);

                NativeMethods.SelectObject(srcDC, oldSrc);
                NativeMethods.DeleteObject(srcDC);
                NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
            }
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void DrawImageScaled(IntPtr sourceBitmap, IntPtr hdc, int left, int top, int imageWidth, int imageHeight, int width, int height)
        {
            IntPtr screenDC;
            IntPtr oldSrc;
            screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr srcDC = NativeMethods.CreateCompatibleDC(screenDC);
            oldSrc = NativeMethods.SelectObject(srcDC, sourceBitmap);


            NativeMethods.AlphaBlendNative(hdc, left, top, width, height, srcDC,
                0, 0, imageWidth, imageHeight,  0xFF);

            NativeMethods.SelectObject(srcDC, oldSrc);
            NativeMethods.DeleteObject(srcDC);
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageScaled(Image image, Graphics canvas, int left, int top, int width, int height, byte alpha)
        {
            if (image == null)
                return;

            if (canvas == null)
                return;


            colorMatrix.Matrix33 = (float)(alpha / 255f);

            ImageAttributes imageAttributes = new ImageAttributes();
            try
            {
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                canvas.DrawImage(image, new Rectangle(left, top, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            finally
            {
                imageAttributes.Dispose();
                imageAttributes = null;
            }
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageScaled(Image image, IntPtr hdc, int left, int top, int width, int height, byte alpha)
        {
            if (image == null)
                return;

            if ((width == 0) || (height == 0))
                return;

            IntPtr screenDC;
            IntPtr oldSrc;
            Bitmap tmpBmp = ResizeBitmap(image, width, height);
            IntPtr srcBitmap = tmpBmp.GetHbitmap(Color.FromArgb(0, 0, 0, 0));
            screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr srcDC = NativeMethods.CreateCompatibleDC(screenDC);
            oldSrc = NativeMethods.SelectObject(srcDC, srcBitmap);


            NativeMethods.AlphaBlendNative(hdc, left, top, width, height, srcDC,
                0, 0, width, height, alpha);


            NativeMethods.SelectObject(srcDC, oldSrc);
            NativeMethods.DeleteObject(srcBitmap);
            NativeMethods.DeleteObject(srcDC);
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);

            tmpBmp.Dispose();
            tmpBmp = null;
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageScaled(IntPtr sourceBitmap, Graphics canvas, int left, int top, int imageWidth, int imageHeight, int width, int height, byte alpha)
        {
            if (canvas == null)
                return;

            IntPtr screenDC;
            IntPtr oldSrc;
            IntPtr hdc = canvas.GetHdc();
            screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr srcDC = NativeMethods.CreateCompatibleDC(screenDC);
            oldSrc = NativeMethods.SelectObject(srcDC, sourceBitmap);


            NativeMethods.AlphaBlendNative(hdc, left, top, width, height, srcDC,
                0, 0, imageWidth, imageHeight, alpha);


            canvas.ReleaseHdc(hdc);

            NativeMethods.SelectObject(srcDC, oldSrc);
            NativeMethods.DeleteObject(srcDC);
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
        }

        /// <summary>
        /// Draws the image scaled.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="hdc">The HDC.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageScaled(IntPtr sourceBitmap, IntPtr hdc, int left, int top, int imageWidth, int imageHeight, int width, int height, byte alpha)
        {
            IntPtr screenDC;
            IntPtr oldSrc;
            screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr srcDC = NativeMethods.CreateCompatibleDC(screenDC);
            oldSrc = NativeMethods.SelectObject(srcDC, sourceBitmap);


            NativeMethods.AlphaBlendNative(hdc, left, top, width, height, srcDC,
                0, 0, imageWidth, imageHeight, alpha);

            NativeMethods.SelectObject(srcDC, oldSrc);
            NativeMethods.DeleteObject(srcDC);
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
        }
        #endregion

        /// <summary>
        /// Copies the image unscaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public static void CopyImageUnscaled(Bitmap image, Graphics canvas, int left, int top)
        {
            if ((image == null) | (canvas == null))
                return;
            
            IntPtr hdc = canvas.GetHdc();
            try
            {
                IntPtr srcBitmap = image.GetHbitmap(Color.FromArgb(0));
                try
                {
                    NativeMethods.CopyNativeBitmap(srcBitmap, hdc, image.Width, image.Height, left, top);
                }
                finally
                {
                    NativeMethods.DeleteObject(srcBitmap);
                }
            }
            finally
            {
                canvas.ReleaseHdc(hdc);
            }

            
        }


        /// <summary>
        /// Copies the image scaled.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void CopyImageScaled(Bitmap image, Graphics canvas, int left, int top, int width, int height)
        {
            if (image == null)
                return;

            if (canvas == null)
                return;

            if ((width == 0) || (height == 0))
                return;

            const int SRCCOPY = 0xcc0020;

            IntPtr screenDC;
            IntPtr oldSrc;


            IntPtr hdc = canvas.GetHdc();
            IntPtr srcBitmap = image.GetHbitmap(Color.FromArgb(0));
            screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr srcDC = NativeMethods.CreateCompatibleDC(screenDC);
            oldSrc = NativeMethods.SelectObject(srcDC, srcBitmap);

            if ((image.Height == height) && (image.Width == width))
            {
                NativeMethods.BitBlt(hdc, left, top, width, height, srcDC, 0, 0, SRCCOPY);
            }
            else
            {
                NativeMethods.SetStretchBltMode(hdc, StretchBltMode.COLORONCOLOR);
                NativeMethods.StretchBlt(hdc, left, top, width, height, srcDC, 0, 0, image.Width, image.Height, SRCCOPY);
            }

            canvas.ReleaseHdc(hdc);

            NativeMethods.SelectObject(srcDC, oldSrc);
            NativeMethods.DeleteObject(srcBitmap);
            NativeMethods.DeleteObject(srcDC);
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
        }


        /// <summary>
        /// Creates a GDI bitmap object from a GDI+Bitmap. 
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// A handle to the GDI bitmap object that this method creates. 
        /// </returns>
        public static IntPtr GetNativeBitmap(Bitmap image)
        {
            if (image != null)
                return image.GetHbitmap(Color.FromArgb(0));
            else
                return IntPtr.Zero;
        }


        public unsafe static Image CreateReflection(Image realImage, int reflectionDepth)
        {
            int w;
            int h;
            float opaque;
            int alpha;
            byte newAlpha;
            float frac;
            byte b;
            byte r;
            byte g;

            if (realImage == null)
                return null;
            if (reflectionDepth <= 0)
                return null;

            Bitmap rotatedImage = new Bitmap(realImage);
            rotatedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Bitmap reflectionImage = new Bitmap(realImage.Width, reflectionDepth, PixelFormat.Format32bppArgb);

            h = reflectionDepth;
            w = realImage.Width;

            BitmapData srcData = rotatedImage.LockBits(new Rectangle(0, 0, w, rotatedImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            BitmapData destData = reflectionImage.LockBits(new Rectangle(0, 0, w, reflectionImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);

            byte* srcScan0 = (byte*)srcData.Scan0.ToPointer();
            byte* destScan0 = (byte*)destData.Scan0.ToPointer();

            byte* ps = srcScan0;
            byte* pd = destScan0;

            for (int y = 0; y < h; y++)
            {
                opaque = ((255.0f / h) * (h - y));
                if (opaque < 0.0)
                    opaque = 0.0f;
                if (opaque > 255.0)
                    opaque = 255.0f;

                for (int x = 0; x < w; x++)
                {
                    b = *ps;
                    ps++;

                    g = *ps;
                    ps++;

                    r = *ps;
                    ps++;

                    alpha = *ps;
                    ps++;

                    if (alpha != 0)
                    {
                        frac = (float)(opaque / 255);
                        newAlpha = (byte)(frac * alpha);

                        *pd = b;
                        pd++;

                        *pd = g;
                        pd++;

                        *pd = r;
                        pd++;

                        *pd = (byte)newAlpha;
                        pd++;

                    }
                    else
                    {
                        pd += 4;
                    }
                }
            }


            rotatedImage.UnlockBits(srcData);
            reflectionImage.UnlockBits(destData);

            rotatedImage.Dispose();
            rotatedImage = null;

            return reflectionImage;
        }

        public static IntPtr GetNativeBitmapHandle(Bitmap image)
        {
            if (image == null)
                return IntPtr.Zero;

            int width = image.Width;
            int height = image.Height;

            IntPtr hBitmap, ppvBits;
            BITMAPINFOHEADER bmi = new BITMAPINFOHEADER();
            bmi.biSize = 40;			// Needed for RtlMoveMemory()
            bmi.biBitCount = 32;		// Number of bits
            bmi.biPlanes = 1;			// Number of planes
            bmi.biWidth = width;		// Width of our new bitmap
            bmi.biHeight = -height;	// Height of our new bitmap

            hBitmap = NativeMethods.CreateDIBSection(IntPtr.Zero, bmi, 0, out ppvBits, IntPtr.Zero, 0);
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            NativeMethods.RtlMoveMemory(ppvBits, bitmapData.Scan0, height * bitmapData.Stride); // copies the bitmap
            image.UnlockBits(bitmapData);
            return hBitmap;
        }

        #region Bitmap drawing effects
        /// <summary>
        /// Draws the image sepia.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="alpha">The alpha.</param>
        public static void DrawImageSepia(Image image, Graphics canvas, int left, int top, int width, int height, byte alpha)
        {
            if ((image == null) || (canvas == null))
                return;

            float[][] colorMatrixElements = { 
            new float[] {0.393f, 0.349f, 0.272f, 0, 0},
            new float[] {0.769f, 0.686f, 0.534f, 0, 0},
            new float[] {0.189f, 0.168f, 0.131f, 0, 0},
            new float[] {0,  0,  0,  (float)(alpha / 255f), 0},
            new float[] {0,  0,  0,  0,   1}};
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            ImageAttributes imageAttributes = new ImageAttributes();
            try
            {
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                canvas.DrawImage(image, new Rectangle(left, top, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            finally
            {
                imageAttributes.Dispose();
                imageAttributes = null;
            }
        }


        public static void DrawImageBright(Image image, Graphics canvas, Rectangle rect)
        {
            if ((image == null) || (canvas == null))
                return;

            canvas.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, brightAttributes);
        }


        public static void DrawImageLight(Image image, Graphics canvas, Rectangle rect)
        {
            if ((image == null) || (canvas == null))
                return;
            canvas.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, lightAttributes);
        }

        #endregion
    }
}
