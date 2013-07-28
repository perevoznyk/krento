//===============================================================================
// Copyright Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    public sealed class BufferedCanvas : IDisposable
    {
        private int width;
        private int height;
        private IntPtr memDC;
        private IntPtr ppvBits;
        private IntPtr imageHandle;
        private IntPtr oldBitmap;


        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedCanvas"/> class.
        /// </summary>
        /// <param name="width">The width of the drawing canvas in pixels.</param>
        /// <param name="height">The height of the drawing canvas in pixels.</param>
        public BufferedCanvas(int width, int height)
        {
            SetBounds(width, height);
        }


        ~BufferedCanvas()
        {
            this.Dispose(false);
        }

        #region IDisposable Members


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            DisposeBuffer();
        }

        #endregion

        /// <summary>
        /// Disposes the bitmap buffer.
        /// </summary>
        private void DisposeBuffer()
        {
            if (imageHandle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                NativeMethods.SelectObject(memDC, oldBitmap);
                NativeMethods.DeleteObject(imageHandle);
                NativeMethods.DeleteDC(memDC);
#if PRESSURE
                long pressure = InteropHelper.AlignToPage(width * height * 4);
                if (pressure != 0L)
                {
                    GC.RemoveMemoryPressure(pressure);
                }
#endif
            }
            finally
            {
                imageHandle = IntPtr.Zero;
                memDC = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Sets the drawing canvas bounds.
        /// </summary>
        /// <param name="width">The width of the canvas in pixels.</param>
        /// <param name="height">The height of the canvas in pixels.</param>
        public void SetBounds(int width, int height)
        {
            this.width = width;
            this.height = height;
            RecreateBuffer();
        }

        internal static IntPtr GetScreenDC()
        {
            return NativeMethods.GetDC(IntPtr.Zero);
        }

        internal static void ReleaseScreenDC(IntPtr screenDC)
        {
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
            screenDC = IntPtr.Zero;
        }

        /// <summary>
        /// Recreates the canvas bitmap buffer.
        /// </summary>
        private void RecreateBuffer()
        {
            IntPtr ScreenDC;

            DisposeBuffer();

            ScreenDC = GetScreenDC();
            try
            {
                memDC = NativeMethods.CreateCompatibleDC(ScreenDC);

                imageHandle = NativeMethods.CreateNativeBitmap(width, height, out ppvBits);

                oldBitmap = NativeMethods.SelectObject(memDC, imageHandle);

#if PRESSURE
                long pressure = InteropHelper.AlignToPage(width * height * 4);
                if (pressure != 0L)
                {
                    GC.AddMemoryPressure(pressure);
                }
#endif

            }
            finally
            {
                ReleaseScreenDC(ScreenDC);
            }
        }

        /// <summary>
        /// Gets the native handle of the drawing context for the canvas.
        /// </summary>
        /// <value>The drawing context handle (DC).</value>
        public IntPtr Handle
        {
            get { return memDC; }
        }

        /// <summary>
        /// Gets the image handle.
        /// </summary>
        /// <value>The image handle (HBITMAP).</value>
        public IntPtr ImageHandle
        {
            get { return imageHandle; }
        }

        /// <summary>
        /// Returns or sets the width of the canvas, in pixels.
        /// </summary>
        /// <value>The width of the canvas.</value>
        public int Width
        {
            get { return this.width; }
            set
            {
                this.width = value;
                RecreateBuffer();
            }
        }

        public IntPtr Bits
        {
            get { return ppvBits; }
        }

        /// <summary>
        /// Returns or sets the height of the canvas, in pixels.
        /// </summary>
        /// <value>The height of the canvas.</value>
        public int Height
        {
            get { return this.height; }
            set
            {
                this.height = value;
                RecreateBuffer();
            }
        }

    }
}
