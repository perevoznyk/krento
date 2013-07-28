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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security.Permissions;
using Laugris.Sage;

namespace Laugris.Sage
{
    /// <summary>
    /// The popup window of the Notifier class
    /// </summary>
    internal sealed class NotifyWindow : Control
    {
        private Notifier notifier;
        private bool alphaBlend;
        private byte alphaBlendValue;
        private NotifyTimers animStatus;
        private int scrollSpeed;
        private int stepSize;
        private TaskbarPosition edge;
        internal Rectangle WARect;
        private bool mouseInControl;

        private const int RDW_ERASE = 4;
        private const int RDW_INVALIDATE = 1;
        private const int RDW_ALLCHILDREN = 0x80;
        private const int RDW_FRAME = 0x400;
        private SolidBrush brush;
        private bool flatBorder;

        private int borderWidth = 1;
        private int borderTopOffset = 0;
        private int radius = 0x12;
        private Color color1 = Color.FromArgb(20, 20, 20);
        private Color color2 = Color.Black;
        private byte alphaMax = 200;


        private Pen borderLightPen = new Pen(Color.FromArgb(0xA6, 0xCA, 0xF0));
        private Pen borderDarkPen = new Pen(Color.Navy);
        private SolidBrush borderBrush = new SolidBrush(Color.Gainsboro);

        public Pen BorderDarkPen
        {
            get { return borderDarkPen; }
            set { borderDarkPen = value; }
        }

        public Pen BorderLightPen
        {
            get { return borderLightPen; }
            set { borderLightPen = value; }
        }

        public bool FlatBorder
        {
            get { return flatBorder; }
            set { flatBorder = value; }
        }


        public Notifier Notifier
        {
            get { return notifier; }
            set { notifier = value; }
        }

        public int ScrollSpeed
        {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        public int StepSize
        {
            get { return stepSize; }
            set { stepSize = value; }
        }

        public TaskbarPosition Edge
        {
            get { return edge; }
            set { edge = value; }
        }


        public bool AlphaBlend
        {
            get { return alphaBlend; }
            set { SetAlphaBlend(value); }
        }


        public byte AlphaBlendValue
        {
            get { return alphaBlendValue; }
            set { SetAlphaBlendValue(value); }
        }


        public NotifyWindow()
        {
            alphaBlendValue = 1;
            animStatus = NotifyTimers.Hidden;
            SetStyle(ControlStyles.ContainerControl, false);
            this.AccessibleRole = AccessibleRole.Alert;
            this.AccessibleDescription = "Notification window";
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style = (int)WindowStyles.WS_POPUP;
                cp.ExStyle = (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW | (int)ExtendedWindowStyles.WS_EX_NOACTIVATE;

                cp.Parent = NativeMethods.GetDesktopWindow();
                return cp;
            }
        }

        public void DoBeforeShow()
        {
            Region region = CreateRoundedRegion(0, this.borderTopOffset, this.Width, this.Height + this.borderTopOffset, this.radius, this.radius);
            base.Region = region;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                NativeMethods.KillTimer(new HandleRef(null, this.Handle), new HandleRef(null, (IntPtr)NotifyTimers.Disappearing));
                notifier.active = false;

                if (disposing)
                {
                    if (borderDarkPen != null)
                    {
                        borderDarkPen.Dispose();
                        borderDarkPen = null;
                    }

                    if (borderLightPen != null)
                    {
                        borderLightPen.Dispose();
                        borderLightPen = null;
                    }

                    if (brush != null)
                    {
                        brush.Dispose();
                        brush = null;
                    }

                    if (borderBrush != null)
                    {
                        borderBrush.Dispose();
                        borderBrush = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal void KillTimers()
        {
            switch (animStatus)
            {
                case NotifyTimers.Appearing: NativeMethods.KillTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Appearing));
                    break;
                case NotifyTimers.Waiting: NativeMethods.KillTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Waiting));
                    break;
                case NotifyTimers.Disappearing: NativeMethods.KillTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                    break;
                default: break;
            }

            animStatus = NotifyTimers.Hidden;
        }


        private void SetLayeredAttribs()
        {
            IntPtr AStyle;
            byte LWA_ALPHA = 2;

            byte cUseAlpha = AlphaBlend ? LWA_ALPHA : (byte)0;

            AStyle = NativeMethods.GetWindowLongPointer(this.Handle, WindowLong.GWL_EXSTYLE);
            if (alphaBlend)
            {
                if (((int)AStyle & (int)ExtendedWindowStyles.WS_EX_LAYERED) == 0)
                    NativeMethods.SetWindowLongPointer(this.Handle, WindowLong.GWL_EXSTYLE, (IntPtr)((int)AStyle | (int)ExtendedWindowStyles.WS_EX_LAYERED));
                NativeMethods.SetLayeredWindowAttributes(this.Handle, ColorTranslator.ToWin32(Color.FromArgb(0)), alphaBlendValue,
                  cUseAlpha);

            }
            else
            {
                NativeMethods.SetWindowLongPointer(this.Handle, WindowLong.GWL_EXSTYLE, (IntPtr)((int)AStyle & ~(int)ExtendedWindowStyles.WS_EX_LAYERED));
                NativeMethods.RedrawWindow(new HandleRef(this, this.Handle), IntPtr.Zero, new HandleRef(this, (IntPtr)0), RDW_ERASE | RDW_INVALIDATE | RDW_FRAME | RDW_ALLCHILDREN);
            }
        }

        private void SetAlphaBlend(bool value)
        {
            if (alphaBlend != value)
            {
                alphaBlend = value;
                SetLayeredAttribs();
            }
        }

        private void SetAlphaBlendValue(byte value)
        {
            if (alphaBlendValue != value)
            {
                alphaBlendValue = value;
                SetLayeredAttribs();
            }
        }


        private void WMMouseLeave(ref Message msg)
        {
            if (mouseInControl)
            {
                mouseInControl = false;
                if (animStatus == NotifyTimers.Waiting)
                    NativeMethods.SetTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Waiting), notifier.ScrollTime, new HandleRef(this, IntPtr.Zero));
            }
            msg.Result = IntPtr.Zero;
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

        private void WMMouseMove()
        {
            TRACKMOUSEEVENT mouseEvent = new TRACKMOUSEEVENT();
            mouseEvent.dwFlags = NativeMethods.TME_LEAVE | NativeMethods.TME_HOVER;
            mouseEvent.hwndTrack = this.Handle;
            mouseEvent.dwHoverTime = 1;

            NativeMethods.TrackMouseEvent(mouseEvent);
        }

        private HandleRef ThisHandle
        {
            get { return new HandleRef(this, this.Handle); }
        }

        private void WMTimer(ref Message msg)
        {
            switch ((NotifyTimers)msg.WParam)
            {
                case NotifyTimers.Appearing:
                    animStatus = NotifyTimers.Appearing;
                    if (alphaBlendValue + StepSize >= alphaMax)
                    {
                        AlphaBlendValue = alphaMax;
                        if (!notifier.Sliding)
                        {
                            NativeMethods.KillTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Appearing));
                            NativeMethods.SetTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Waiting), notifier.ShowTime, new HandleRef(this, IntPtr.Zero));
                            animStatus = NotifyTimers.Waiting;
                        }
                    }
                    else
                    {
                        AlphaBlendValue = (byte)(alphaBlendValue + StepSize);
                    }

                    if (notifier.Sliding)
                        switch (edge)
                        {
                            #region ABE_LEFT
                            case TaskbarPosition.ScreenLeft:
                                {
                                    if ((this.Width + scrollSpeed) < notifier.AdjustedWidth)
                                    {
                                        Width = Width + scrollSpeed;
                                    }
                                    else
                                    {
                                        this.Width = notifier.AdjustedWidth;
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Appearing));
                                        NativeMethods.SetTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Waiting), notifier.ShowTime, new HandleRef(this, IntPtr.Zero));
                                        animStatus = NotifyTimers.Waiting;
                                    }
                                    break;
                                }
                            #endregion
                            #region ABE_TOP
                            case TaskbarPosition.ScreenTop:
                                {
                                    if ((this.Height + ScrollSpeed) < notifier.AdjustedHeight)
                                    {
                                        this.Height = this.Height + ScrollSpeed;
                                    }
                                    else
                                    {
                                        this.Height = notifier.AdjustedHeight;
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Appearing));
                                        NativeMethods.SetTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Waiting), notifier.ShowTime, new HandleRef(this, IntPtr.Zero));
                                        animStatus = NotifyTimers.Waiting;
                                    }
                                    break;
                                }
                            #endregion
                            #region ABE_BOTTOM
                            case TaskbarPosition.ScreenBottom:
                                {
                                    if ((this.Height + ScrollSpeed) < notifier.AdjustedHeight)
                                    {
                                        this.Top = this.Top - ScrollSpeed;
                                        this.Height = this.Height + ScrollSpeed;
                                    }
                                    else
                                    {
                                        this.Height = notifier.AdjustedHeight;
                                        this.Top = WARect.Bottom - this.Height;
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Appearing));
                                        NativeMethods.SetTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Waiting), notifier.ShowTime, new HandleRef(this, IntPtr.Zero));
                                        animStatus = NotifyTimers.Waiting;
                                    }
                                    break;
                                }
                            #endregion
                            #region ABE_RIGHT
                            case TaskbarPosition.ScreenRight:
                                {
                                    if ((this.Width + ScrollSpeed) < notifier.AdjustedWidth)
                                    {
                                        this.Left = this.Left - ScrollSpeed;
                                        this.Width = this.Width + ScrollSpeed;
                                    }
                                    else
                                    {
                                        this.Width = notifier.AdjustedWidth;
                                        this.Left = WARect.Right - this.Width;
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Appearing));
                                        NativeMethods.SetTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Waiting), notifier.ShowTime, new HandleRef(this, IntPtr.Zero));
                                        animStatus = NotifyTimers.Waiting;
                                    }
                                    break;
                                }
                            #endregion
                        }
                    break;
                case NotifyTimers.Waiting:
                    NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Waiting));
                    NativeMethods.SetTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Disappearing), notifier.Precision, new HandleRef(this, IntPtr.Zero));
                    animStatus = NotifyTimers.Disappearing;
                    break;
                case NotifyTimers.Disappearing:

                    if (alphaBlendValue - StepSize <= 1)
                    {
                        AlphaBlendValue = 1;
                        if (!notifier.Sliding)
                        {
                            NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                            animStatus = NotifyTimers.Hidden;
                            NativeMethods.ShowWindow(Handle, (int)ShowWindowStyles.SW_HIDE);
                            Notifier.DoDeactivate();
                        }
                    }
                    else
                    {
                        AlphaBlendValue = (byte)(AlphaBlendValue - StepSize);
                    }
                    if (Notifier.Sliding)

                        switch (Notifier.GetEdge())
                        {
                            #region ABE_LEFT
                            case TaskbarPosition.ScreenLeft:
                                {
                                    if ((this.Width - ScrollSpeed) > 0)
                                    {
                                        this.Width = this.Width - ScrollSpeed;
                                    }
                                    else
                                    {
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                                        animStatus = NotifyTimers.Hidden;
                                        NativeMethods.ShowWindow(Handle, (int)ShowWindowStyles.SW_HIDE);
                                        Notifier.DoDeactivate();
                                    }
                                    break;
                                }

                            #endregion
                            #region ABE_TOP
                            case TaskbarPosition.ScreenTop:
                                {
                                    if ((this.Height - ScrollSpeed) > 0)
                                    {
                                        this.Height = this.Height - ScrollSpeed;
                                    }
                                    else
                                    {
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                                        animStatus = NotifyTimers.Hidden;
                                        NativeMethods.ShowWindow(Handle, (int)ShowWindowStyles.SW_HIDE);
                                        Notifier.DoDeactivate();
                                    }
                                    break;
                                }
                            #endregion
                            #region ABE_BOTTOM
                            case TaskbarPosition.ScreenBottom:
                                {
                                    if ((this.Height - ScrollSpeed) > 0)
                                    {
                                        this.Top = this.Top + ScrollSpeed;
                                        this.Height = this.Height - ScrollSpeed;
                                    }
                                    else
                                    {
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                                        animStatus = NotifyTimers.Hidden;
                                        NativeMethods.ShowWindow(Handle, (int)ShowWindowStyles.SW_HIDE);
                                        Notifier.DoDeactivate();
                                    }
                                    break;
                                }
                            #endregion
                            #region ABE_RIGHT
                            case TaskbarPosition.ScreenRight:
                                {
                                    if ((this.Width - ScrollSpeed) > 0)
                                    {
                                        this.Left = this.Left + ScrollSpeed;
                                        this.Width = this.Width - ScrollSpeed;
                                    }
                                    else
                                    {
                                        NativeMethods.KillTimer(ThisHandle, new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                                        animStatus = NotifyTimers.Hidden;
                                        NativeMethods.ShowWindow(Handle, (int)ShowWindowStyles.SW_HIDE);
                                        Notifier.DoDeactivate();
                                    }
                                    break;
                                }
                            #endregion
                        }

                    break;

            }
        }




        private void WMMouseHover(ref Message msg)
        {
            if (animStatus == NotifyTimers.Waiting)
                NativeMethods.KillTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Waiting));

            if (!mouseInControl)
            {
                mouseInControl = true;

                if (animStatus == NotifyTimers.Disappearing)
                {
                    NativeMethods.KillTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Disappearing));
                    animStatus = NotifyTimers.Appearing;
                    NativeMethods.SetTimer(new HandleRef(this, this.Handle), new HandleRef(this, (IntPtr)NotifyTimers.Appearing), notifier.Precision, new HandleRef(this, IntPtr.Zero));
                }
            }
            msg.Result = IntPtr.Zero;
        }


        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if (brush != null)
                {
                    brush.Dispose();
                    brush = null;
                }
                brush = new SolidBrush(this.BackColor);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintSmoke(e);
        }


        public static Region CreateRoundedRegion(int top, int left, int width, int height, int xradius, int yradius)
        {
            Region region = null;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = NativeMethods.CreateRoundRectRgn(top, left, width, height, xradius, yradius);
                region = Region.FromHrgn(zero);
            }
            finally
            {
                NativeMethods.DeleteObject(zero);
            }
            return region;
        }

        private void PaintSmoke(PaintEventArgs e)
        {
            int X;
            int Y;
            SizeF s;
            Rectangle r = new Rectangle(0, 0, notifier.AdjustedWidth, notifier.AdjustedHeight);
            Graphics graphics = e.Graphics;

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            //graphics.FillRectangle(borderBrush, r);
            Region region = CreateRoundedRegion(this.borderWidth, this.borderTopOffset + this.borderWidth, r.Width - this.borderWidth, (r.Height + this.borderTopOffset) - this.borderWidth, this.radius - this.borderWidth, this.radius - this.borderWidth);
            graphics.SetClip(region, CombineMode.Exclude);
            graphics.FillRegion(this.borderBrush, base.Region);
            graphics.ResetClip();


            using (region)
            {
                LinearGradientBrush brush = new LinearGradientBrush(region.GetBounds(e.Graphics), this.color1, this.color2, LinearGradientMode.Vertical);
                using (brush)
                {
                    Blend blend = new Blend();
                    float[] numArray = new float[3];
                    numArray[1] = 0.1f;
                    numArray[2] = 1f;
                    blend.Factors = numArray;
                    float[] numArray2 = new float[3];
                    numArray2[1] = 0.3f;
                    numArray2[2] = 1f;
                    blend.Positions = numArray2;
                    brush.Blend = blend;
                    graphics.FillRegion(brush, region);
                }
            }

            try
            {
                //graphics.FillRectangle(brush, r);

                X = 4;
                if (notifier.Glyph != null)
                {
                    Y = (notifier.AdjustedHeight - notifier.GlyphSize) / 2;
                    e.Graphics.DrawImage(notifier.Glyph, X, Y, notifier.GlyphSize, notifier.GlyphSize);
                    X = X + notifier.GlyphSize + 4;
                }
                Y = 4;
                r = new Rectangle(X, Y, notifier.AdjustedWidth - 4 - X, notifier.AdjustedHeight - 4 - Y);
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                if (!string.IsNullOrEmpty(notifier.Caption))
                {
                    Font captionFont = new Font(notifier.Font, FontStyle.Bold);
                    try
                    {
                        SolidBrush captionBrush = new SolidBrush(notifier.CaptionColor);
                        try
                        {
                            StringFormat formatCaption = new StringFormat();
                            try
                            {
                                formatCaption.Alignment = StringAlignment.Center;
                                formatCaption.Trimming = StringTrimming.EllipsisWord;
                                s = e.Graphics.MeasureString(notifier.Caption, captionFont, notifier.AdjustedWidth, formatCaption);
                                e.Graphics.DrawString(notifier.Caption, captionFont, captionBrush, r, formatCaption);
                            }
                            finally
                            {
                                formatCaption.Dispose();
                            }
                        }
                        finally
                        {
                            captionBrush.Dispose();
                        }
                    }
                    finally
                    {
                        captionFont.Dispose();
                    }
                    Y = (int)(Y + s.Height + 4);
                    e.Graphics.DrawLine(Pens.Gainsboro, new Point(r.Right - 4, Y), new Point(r.Left, Y));
                    Y += 6;
                }
                r.Y = Y;
                r.Width -= 4;
                StringFormat formatBody = new StringFormat();
                try
                {
                    formatBody.Alignment = StringAlignment.Near;
                    formatBody.Trimming = StringTrimming.EllipsisWord;
                    Font bodyFont = new Font(notifier.Font, FontStyle.Regular);
                    try
                    {
                        SolidBrush bodyBrush = new SolidBrush(notifier.TextColor);
                        try
                        {
                            e.Graphics.DrawString(notifier.Text, bodyFont, bodyBrush, r, formatBody);
                        }
                        finally
                        {
                            bodyBrush.Dispose();
                            bodyBrush = null;
                        }
                    }
                    finally
                    {
                        bodyFont.Dispose();
                        bodyFont = null;
                    }
                }
                finally
                {
                    formatBody.Dispose();
                    formatBody = null;
                }
            } //back color brush 
            finally
            {
            }
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {

            WindowMessage msg = (WindowMessage)m.Msg;

            switch (msg)
            {
                case WindowMessage.WM_MOUSEHOVER:
                    WMMouseHover(ref m);
                    break;
                case WindowMessage.WM_TIMER:
                    WMTimer(ref m);
                    break;
                case WindowMessage.WM_MOUSEMOVE:
                    WMMouseMove();
                    break;
                case WindowMessage.WM_ERASEBKGND:
                    WMEraseBakground(ref m);
                    return;
                case WindowMessage.WM_MOUSELEAVE:
                    WMMouseLeave(ref m);
                    break;
                case WindowMessage.WM_PAINT:
                    WmPaint(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

    }


}
