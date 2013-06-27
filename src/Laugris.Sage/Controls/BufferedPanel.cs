//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Laugris.Sage
{
    public partial class BufferedPanel : Control, IDisposable
    {
        private Image wallpaper;

        public BufferedPanel()
        {
            InitializeComponent();
            BackColor = Color.Black;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.DoubleBuffer, true);
        }

        public BufferedPanel(IContainer container)
        {
            if (container != null)
                container.Add(this);

            InitializeComponent();
            BackColor = Color.Black;
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.DoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (wallpaper == null)
                base.OnPaint(pe);
            else
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                BitmapPainter.CopyImageScaled((Bitmap)wallpaper, pe.Graphics, 0, 0, this.Width, this.Height);
                base.OnPaint(pe);
            }
        }


        private void WMEraseBakground(ref Message msg)
        {
            msg.Result = (IntPtr)(1);
        }

        private void WmPaint(ref Message m)
        {
            const int SRC_COPY = 0xcc0020;

            IntPtr wParam;
            Rectangle clientRectangle;
            PAINTSTRUCT lpPaint = new PAINTSTRUCT();
            bool emptyDC = false;
            if (m.WParam == IntPtr.Zero)
            {
                wParam = NativeMethods.BeginPaint(this.Handle, ref lpPaint);
                clientRectangle = new Rectangle(lpPaint.rcPaint_left, lpPaint.rcPaint_top, lpPaint.rcPaint_right - lpPaint.rcPaint_left, lpPaint.rcPaint_bottom - lpPaint.rcPaint_top);
                emptyDC = true;
            }
            else
            {
                wParam = m.WParam;
                clientRectangle = this.ClientRectangle;
            }

            if ((clientRectangle.Width > 0) && (clientRectangle.Height > 0))
            {
                Rectangle targetRectangle = this.ClientRectangle;
                using (BufferedCanvas canvas = new BufferedCanvas(targetRectangle.Width, targetRectangle.Height))
                {
                    Graphics internalGraphics = Graphics.FromHdc(canvas.Handle);
                    internalGraphics.SetClip(clientRectangle);
                    using (PaintEventArgs args = new PaintEventArgs(internalGraphics, clientRectangle))
                    {
                        this.OnPaint(args);
                    }
                    internalGraphics.Dispose();
                    NativeMethods.BitBlt(wParam, 0, 0, targetRectangle.Width, targetRectangle.Height, canvas.Handle, 0, 0, SRC_COPY);
                }
            }

            if (emptyDC)
            {
                NativeMethods.EndPaint(this.Handle, ref lpPaint);
            }

        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {

            WindowMessage msg = (WindowMessage)m.Msg;

            switch (msg)
            {
                case WindowMessage.WM_ERASEBKGND:
                    WMEraseBakground(ref m);
                    return;
                case WindowMessage.WM_PAINT:
                    WmPaint(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        public Image Wallpaper
        {
            get
            {
                return wallpaper;
            }
            set
            {
                if (wallpaper != value)
                {
                    wallpaper = value;
                    Invalidate();
                }
            }
        }
    }
}
