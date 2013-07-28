using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Laugris.Sage
{
    /// <summary>
    /// Base class for visual elements of the user interface
    /// </summary>
    public class Visual : Drawable
    {

        #region Private fields
        protected internal Window FParent;
        private Font font;
        private Color foreColor;
        private Color backColor;
        private string caption;

        #endregion

        protected Animation currentAnimation;

        #region Events
        /// <summary>
        /// Occurs when the Font property value changes. 
        /// </summary>
        public event EventHandler FontChanged;
        /// <summary>
        /// Occurs when the mouse pointer is moved over the window
        /// </summary>
        public event MouseEventHandler MouseMove;
        /// <summary>
        /// Occurs when the mouse pointer is over the window and a mouse button is pressed. 
        /// </summary>
        public event MouseEventHandler MouseDown;
        /// <summary>
        /// Occurs when the mouse pointer is over the window and a mouse button is released.
        /// </summary>
        public event MouseEventHandler MouseUp;
        /// <summary>
        /// Occurs when the window is clicked. 
        /// </summary>
        public event MouseEventHandler MouseClick;
        /// <summary>
        /// Occurs when the window is double clicked by the mouse.
        /// </summary>
        public event MouseEventHandler MouseDoubleClick;
        /// <summary>
        /// Occurs when the mouse pointer enters the window. 
        /// </summary>
        public event EventHandler MouseEnter;
        /// <summary>
        /// Occurs when the mouse pointer leaves the window. 
        /// </summary>
        public event EventHandler MouseLeave;
        /// <summary>
        /// Occurs when [fore color changed].
        /// </summary>
        public event EventHandler ForeColorChanged;
        /// <summary>
        /// Occurs when [back color changed].
        /// </summary>
        public event EventHandler BackColorChanged;
        /// <summary>
        /// Occurs when [caption changed].
        /// </summary>
        public event EventHandler CaptionChanged;
        #endregion

        /// <summary>
        /// Raises the <see cref="E:ForeColorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnForeColorChanged(EventArgs e)
        {
            EventHandler handler = ForeColorChanged;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnBackColorChanged(EventArgs e)
        {
            EventHandler handler = BackColorChanged;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnCaptionChanged(EventArgs e)
        {
            EventHandler handler = CaptionChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseMove;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseDown;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseUp;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:FontChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnFontChanged(EventArgs e)
        {
            EventHandler handler = FontChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseClick(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDoubleClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseDoubleClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseLeave(EventArgs e)
        {
            EventHandler handler = MouseLeave;
            if (handler != null)
                handler(this, e);
            if (VisualFeedback)
                Repaint();
        }

        /// <summary>
        /// Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseEnter(EventArgs e)
        {
            EventHandler handler = MouseEnter;
            if (handler != null)
                handler(this, e);
            if (VisualFeedback)
                Repaint();
        }

        /// <summary>
        /// Gets or sets a value indicating whether visual feedback is used on mouse enter or leave.
        /// </summary>
        /// <value><c>true</c> if visual feedback is used; otherwise, <c>false</c>.</value>
        public bool VisualFeedback { get; set; }

        #region Constructors

        internal void InitVisual()
        {
            foreColor = SystemColors.WindowText;
            backColor = SystemColors.Window;
        }

        protected Visual()
        {
            InitVisual();
        }

        public Visual(Window parent)
        {
            Parent = parent;
            InitVisual();
        }

        #endregion

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Visual"/> is reclaimed by garbage collection.
        /// </summary>
        ~Visual()
        {
            Dispose(false);
        }

        public virtual void StartAnimation(Animation animation)
        {
            if (animation != null)
            {
                animation.SetStartTime(-1);
                SetAnimation(animation);

                if (animation.StartOffset > 0)
                {
                    DelayHandler delay = new DelayHandler(PostDelay);
                    delay.SetInterval(animation.StartOffset);
                }
                else
                    Invalidate();
            }
        }

        private void PostDelay()
        {
            Invalidate();
        }

        public void ClearAnimation()
        {
            if (currentAnimation != null)
            {
                currentAnimation.Detach();
            }
            currentAnimation = null;
        }

        public Animation Animation
        {
            get { return currentAnimation; }
            set { SetAnimation(value); }
        }


        private void SetAnimation(Animation animation)
        {
            currentAnimation = animation;
            if (animation != null)
            {
                animation.Reset();
                animation.Target = this;
            }

        }

        public bool Opaque { get; set; }


        /// <summary>
        /// Gets or sets the foreground color of the window
        /// </summary>
        /// <value>The foreground color of the window.</value>
        public virtual Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }


        public virtual string Caption
        {
            get { return caption; }
            set
            {
                if (caption != value)
                {
                    caption = value;
                    OnCaptionChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>The color of the back.</value>
        public virtual Color BackColor
        {
            get { return backColor; }
            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the window
        /// </summary>
        /// <value>The Font to apply to the text displayed by the window.</value>
        public virtual Font Font
        {
            get
            {
                if (font != null)
                    return font;
                else
                    return TextPainter.DefaultFont;
            }
            set
            {
                if (font != null && font.Equals(value))
                {
                    return;
                }

                font = value;
                Invalidate();
                OnFontChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Sets the bounds of the control.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void SetBounds(int left, int top, int width, int height)
        {
            base.SetBounds(left, top, width, height);
            InvalidateSelf();
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
            Visible = true;
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide()
        {
            Visible = false;
        }


        /// <summary>
        /// Test the mouse hit 
        /// </summary>
        /// <param name="hitPoint">The hit point.</param>
        /// <returns></returns>
        public bool HitTest(Point hitPoint)
        {
            return BoundsRect.Contains(hitPoint);
        }

        /// <summary>
        /// Gets or sets the parent window.
        /// </summary>
        /// <value>The parent window.</value>
        public Window Parent
        {
            get { return FParent; }
            set { SetParent(value); }
        }

        /// <summary>
        /// Sets the parent window.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetParent(Window value)
        {
            if (FParent != value)
            {
                if (FParent != null)
                {
                    FParent.RemoveControl(this);
                }
                if (value != null)
                {
                    value.InsertControl(this);
                }
            }
        }


        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>The canvas.</value>
        public Graphics Canvas
        {
            get
            {
                if (FParent != null)
                    return FParent.Canvas;
                else
                    return null;
            }
        }

        /// <summary>
        /// Paints this visual control on the canvas of the parent window.
        /// This method does not call the Update method of the parent window
        /// </summary>
        protected virtual void Paint()
        {
            Draw(this.Canvas);
        }

        protected internal void InternalPaint()
        {
            if ((FParent != null) && Visible)
            {
                Paint();
            }
        }


        /// <summary>
        /// Sets the Z order position.
        /// </summary>
        /// <param name="position">The position.</param>
        protected internal void SetZOrderPosition(int position)
        {
            int i;
            int count;

            if (FParent != null)
            {
                i = FParent.Controls.IndexOf(this);
                if (i >= 0)
                {
                    count = FParent.ControlCount;
                    if (position < 0)
                        position = 0;
                    if (position >= count)
                        position = count - 1;
                    if (position != 0)
                    {
                        FParent.Controls.Remove(this);
                        FParent.Controls.Insert(position, this);
                        InvalidateControl(Visible, true);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Z order.
        /// </summary>
        /// <param name="topMost">if set to <c>true</c> [top most].</param>
        protected void SetZOrder(bool topMost)
        {
            if (FParent != null)
            {
                if (topMost)
                    SetZOrderPosition(FParent.Controls.Count - 1);
                else
                    SetZOrderPosition(0);
            }
        }

        /// <summary>
        /// Brings to front.
        /// </summary>
        public void BringToFront()
        {
            SetZOrder(true);
        }

        /// <summary>
        /// Sends to back.
        /// </summary>
        public void SendToBack()
        {
            SetZOrder(false);
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
                InvalidateControl(true, (value && Opaque));
            }
        }
        /// <summary>
        /// Invalidates this instance image on the canvas of the parent window. This method call InvalidateControl method.
        /// At the end the Update method is called, so the changes will be visible on the parent window
        /// </summary>
        public void Invalidate()
        {
            InvalidateControl(Visible, Opaque);
        }

        /// <summary>
        /// Repaints this instance and call Update method of the parent window
        /// </summary>
        public void Repaint()
        {
            if (Visible && (FParent != null))
            {
                if (Opaque)
                {
                    NativeMethods.SendMessage(FParent.Handle, NativeMethods.CN_REPAINTCONTROLS, (IntPtr)(FParent.Controls.IndexOf(this)), IntPtr.Zero);
                }
                else
                {
                    //Invalidate calls the Update directly. No second update needed at this point
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Refreshes this instance image on the parent canvas. This method call the Repaint method.
        /// The Update is called automatically.
        /// </summary>
        public void Refresh()
        {
            Repaint();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public virtual void Update()
        {
            if (FParent != null)
                NativeMethods.SendMessage(FParent.Handle, NativeMethods.CN_UPDATEPARENT, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Invalidates the control. This method calls the InvalidateRect method of the parent window.
        /// 
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="isOpaque">if set to <c>true</c> [is opaque].</param>
        protected internal void InvalidateControl(bool isVisible, bool isOpaque)
        {
            if (isVisible)
            {
                InvalidateSelf();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SetParent(null);
            else
            {
                if (FParent != null)
                {
                    FParent.Controls.Remove(this);
                    NativeMethods.PostMessage(FParent.Handle, WindowMessage.WM_PAINT, IntPtr.Zero, IntPtr.Zero);
                    FParent = null;
                }
            }
        }

        #endregion
    }
}
