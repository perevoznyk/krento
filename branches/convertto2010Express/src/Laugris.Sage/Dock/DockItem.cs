//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Laugris.Sage
{


    /// <summary>
    /// This class is a placeholder for all information concerning the dock item drawing.
    /// The information is quite generic and used by all dock types implemented in Karna library.
    /// </summary>
    public class DockItem : IDockItem, IDisposable
    {
        private Size captionSize = new Size();
        private string imageName;
        private string hint;
        private int x;
        private int y;
        private int width;
        private int height;
        private string id;
        private int tag;
        private int order;
        private Single angle;
        private float scale = 1.0f;
        private byte alpha = 0xFF;
        private int reflectionDepth = DefaultDockSettings.ReflectionDepth;
        private IntPtr srcBitmap = IntPtr.Zero;
        private bool disposed;
        private int initialIndex;
        private Image icon;
        private bool keepNativeBitmap;
        [AccessedThroughProperty("Caption")]
        private string caption;
        [AccessedThroughProperty("ImageIndex")]
        private int imageIndex = -1;
        [AccessedThroughProperty("ImageList")]
        private ImageList imageList;
        [AccessedThroughProperty("PaintWidth")]
        private int paintWidth;
        [AccessedThroughProperty("PaintHeight")]
        private int paintHeight;

        /// <summary>
        /// Disposes the Windows native bitmap
        /// </summary>
        protected virtual void DisposeBitmap()
        {
            if (srcBitmap != IntPtr.Zero)
            {
                try
                {
                    NativeMethods.DeleteObject(srcBitmap);
                }
                finally
                {
                    srcBitmap = IntPtr.Zero;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [DefaultValue(false)]
        public bool KeepNativeBitmap
        {
            get
            {
                return keepNativeBitmap;
            }
            set
            {
                if (keepNativeBitmap != value)
                {
                    keepNativeBitmap = value;
                    DisposeBitmap();
                    if (value && (icon != null))
                    {
                        srcBitmap = BitmapPainter.GetNativeBitmap((Bitmap)icon);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the real drawing rectangle based on the with 
        /// of the icon and the scaling factor
        /// </summary>
        /// <value>The width of the paint rectangle</value>
        public int PaintWidth
        {
            get { return paintWidth; }
        }

        /// <summary>
        /// Gets or sets the height of the of the real drawing rectangle based 
        /// on the height of the icon and the scaling factor
        /// </summary>
        /// <value>The height of the paint rectangle.</value>
        public int PaintHeight
        {
            get { return paintHeight; }
        }


        public Size CaptionSize
        {
            get { return this.captionSize; }
        }

        public ImageList ImageList
        {
            get { return imageList; }
            set { imageList = value; }
        }


        /// <summary>
        /// Gets or sets the index of the image.
        /// </summary>
        /// <value>The index of the image.</value>
        [DefaultValue(-1)]
        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        protected Image GetImageFromBitmap()
        {
            if (srcBitmap == IntPtr.Zero)
                return null;
            else
                return Bitmap.FromHbitmap(srcBitmap);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //clear managed part
                    if (icon != null)
                    {
                        icon.Dispose();
                    }
                }
                //clear unmanaged resourses
                DisposeBitmap();
            }
            disposed = true;
        }


        ~DockItem()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public Image Icon
        {
            get { return icon; }

            set
            {
                if (icon != value)
                {
                    icon = value;
                    if (keepNativeBitmap)
                    {
                        DisposeBitmap();
                        if (icon != null)
                        {
                            srcBitmap = BitmapPainter.GetNativeBitmap((Bitmap)icon);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the native Windows bitmap for the dock item.
        /// if <see cref="KeepNativeBitmap"/> is true then the reference to Windows native
        /// bitmap is returned, else the new native bitmap will be created from the icon
        /// </summary>
        /// <value>The native bitmap.</value>
        public IntPtr NativeBitmap
        {
            get
            {
                if (keepNativeBitmap)
                    return srcBitmap;
                else
                    return BitmapPainter.GetNativeBitmap((Bitmap)icon);
            }
        }

        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                using (Graphics screen = Graphics.FromHwnd(IntPtr.Zero))
                {
                    SizeF captionSizeF = screen.MeasureString(caption, TextPainter.DefaultFont);
                    captionSize.Height = (int)captionSizeF.Height;
                    captionSize.Width = (int)captionSizeF.Width;
                }
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the image.
        /// </summary>
        /// <value>The X.</value>
        [DefaultValue(0)]
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the image.
        /// </summary>
        /// <value>The Y.</value>
        [DefaultValue(0)]
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Gets or sets the width of the image.
        /// Be aware: The DockItem class does not check the value of the provided image size,
        /// in case if size is smaller than the real size of the image, only part of the image will be drawn.
        /// In case if Reflection is used the total height of the real image will be the height value
        /// plus the height of the reflection.
        /// </summary>
        /// <value>The width of the image.</value>
        [DefaultValue(0)]
        public int Width
        {
            get { return width; }
            set { width = value; }
            
        }

        /// <summary>
        /// Gets or sets the height of the image.
        /// Be aware: The DockItem class does not check the value of the provided image size,
        /// in case if size is smaller than the real size of the image, only part of the image will be drawn.
        /// In case if Reflection is used the total height of the real image will be the height value
        /// plus the height of the reflection.
        /// </summary>
        /// <value>The height.</value>
        [DefaultValue(0)]
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Sets the size of the image.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }


        /// <summary>
        /// Gets or sets the file name of the image.
        /// </summary>
        /// <value>The name of the image.</value>
        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        [DefaultValue(0)]
        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public Single Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public byte Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public int ReflectionDepth
        {
            get { return reflectionDepth; }
            set { reflectionDepth = value; }
        }

        public int InitialIndex
        {
            get { return initialIndex; }
            set { initialIndex = value; }
        }

        public void ResetPaintSize()
        {
            paintHeight = (int)(height * scale);
            paintWidth = (int)(width * scale);
        }

    }
}
