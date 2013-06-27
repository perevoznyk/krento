using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    public class Window : LayeredWindow, IDrawableCallback
    {
        private List<Visual> controls;
        private Visual mouseControl;
        private bool disposing;
        private LiveBackground liveBackground;
        private Transformation childTransformation;
        private bool BackgroundTrigger;
        private int repaintTimer = 99;

        public Window()
        {
            controls = new List<Visual>();
        }

        protected override void Dispose(bool disposing)
        {
            if (liveBackground != null)
                liveBackground.OnDestroy();
            this.disposing = true;
            int i = ControlCount;
            while (i != 0)
            {
                Visual Instance = Controls[i - 1];
                if (disposing)
                {
                    RemoveControl(Instance);
                    Instance.Dispose();
                }
                else
                {
                    controls.Remove(Instance);
                    Instance.FParent = null;
                }
                i = ControlCount;
            }

            if (childTransformation != null)
            {
                childTransformation.Dispose();
                childTransformation = null;
            }
            base.Dispose(disposing);
        }

        public List<Visual> Controls
        {
            get { return controls; }
        }

        public int ControlCount
        {
            get { return controls.Count; }
        }

        public LiveBackground LiveBackground
        {
            get { return liveBackground; }
            set
            {
                if (liveBackground != null)
                {
                    int handler = liveBackground.Handler;
                    liveBackground.Stop();
                    if (handler > 0)
                        ClearInterval(handler);
                }

                //to clear the drawing from the previous background
                //in case if the background is transparent
                liveBackground = null;
                Repaint();
                liveBackground = value;
                if (liveBackground != null)
                    liveBackground.Run();
            }
        }

        internal void RemoveControl(Visual visual)
        {
            if (visual != null)
            {
                if (!disposing)
                    visual.InvalidateControl(visual.Visible, false);
                controls.Remove(visual);
                visual.SetCallBack(null);
                visual.FParent = null;
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (liveBackground != null)
                liveBackground.OnSizeChanged();
        }

        public Visual MouseControl
        {
            get { return mouseControl; }
        }

        protected Visual ControlAtPos(Point P)
        {
            for (int i = 0; i < ControlCount; i++)
            {
                if (controls[i].HitTest(P))
                    return controls[i];
            }
            return null;
        }

        /// <summary>
        /// Raises the <see cref="E:MouseClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Visual control = ControlAtPos(e.Location);
            if (control != null)
                control.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta));
            base.OnMouseClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDoubleClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            Visual control = ControlAtPos(e.Location);
            if (control != null)
                control.OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta));
            base.OnMouseDoubleClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Visual control = ControlAtPos(e.Location);
            if (control != null)
                control.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta));
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Focused)
            {
                Visual control = ControlAtPos(e.Location);

                if (MouseControl != control)
                {
                    if (MouseControl != null)
                        MouseControl.OnMouseLeave(EventArgs.Empty);

                    if (control != null)
                        control.OnMouseEnter(EventArgs.Empty);

                    mouseControl = control;
                }

                if (control != null)
                    control.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta));
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Visual control = ControlAtPos(e.Location);
            if (control != null)
                control.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta));
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Visual Target = null;
            if (MouseControl != Target)
            {
                if (MouseControl != null)
                    MouseControl.OnMouseLeave(EventArgs.Empty);
                mouseControl = Target;
            }
            base.OnMouseLeave(e);
        }

        internal void InsertControl(Visual visual)
        {
            if (visual != null)
            {
                if (controls.IndexOf(visual) == -1)
                    controls.Add(visual);
                visual.FParent = this;
                visual.SetCallBack(this);
                visual.Invalidate();
            }
        }


        protected override void Draw()
        {
            if (!disposing)
            {
                base.Draw();
                if (liveBackground != null)
                    liveBackground.Draw(Canvas, BackgroundTrigger);
                PaintControls(null);
            }
        }

        /// <summary>
        /// CN_DRAW_BACKGROUND message handler
        /// Redraws the background internally.
        /// </summary>
        private void RedrawBackgroundInternal()
        {
            lock (this)
            {
                BackgroundTrigger = true;
                try
                {
                    RepaintInternal(null);
                }
                finally
                {
                    BackgroundTrigger = false;
                }
            }
        }

        protected internal void RedrawBackground()
        {
            PostMessageToSelf(NativeMethods.CN_DRAW_BACKGROUND, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Paints the controls. To display the changes call Update method
        /// </summary>
        /// <param name="first">The first control to paint.</param>
        protected internal virtual void PaintControls(Visual first)
        {
            int drawingTime = SystemClock.UptimeMillis();

            bool more = false;
            int i = 0;
            if (first != null)
            {
                i = controls.IndexOf(first);
                if (i < 0)
                    i = 0;
            }
            int count = ControlCount;
            while (i < count)
            {
                if (controls[i].Visible)
                {
                    more |= DrawChild(controls[i], drawingTime);
                }
                i++;
            }

            if ((more) && (!BackgroundTrigger))
                StartTimer(repaintTimer, 1);
        }

        protected override void HandleTimerTick(int timerNumber)
        {
            if (timerNumber == repaintTimer)
            {
                StopTimer(repaintTimer);
                Repaint();
            }
            else
                base.HandleTimerTick(timerNumber);
        }

        /// <summary>
        /// Draws the child.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="drawingTime">The drawing time.</param>
        /// <returns></returns>
        protected virtual bool DrawChild(Visual child, int drawingTime)
        {
            bool more = false;
            float alpha = 1.0f;
            Transformation transformToApply = null;
            bool changeSize = false;
            bool changeAngle = false;
            Animation a = child.Animation;

            #region Animation present
            if (a != null)
            {
                bool initialized = a.IsInitialized();
                if (!initialized)
                {
                    a.Initialize(child.BoundsRect);
                }

                if (childTransformation == null)
                    childTransformation = new Transformation();
                else
                    childTransformation.Clear();

                more = a.GetTransformation(drawingTime, childTransformation);
                transformToApply = childTransformation;
                changeSize = a.WillChangeBounds();

            }
            #endregion

            if ((!changeSize) && (!ClientRect.IntersectsWith(child.BoundsRect)))
                return more;

            if (transformToApply != null)
            {

                if (a.WillChangeBounds())
                {
                    Rectangle rt = transformToApply.TransformationBounds(a.BoundsRect);
                    child.SetBoundsInternal(rt);
                }
                else
                    if (a.WillChangeOffset())
                    {
                        child.MoveToInternal((int)transformToApply.OffsetX, (int)transformToApply.OffsetY);
                    }


                if (a.WillChangeAlpha())
                {
                    alpha = transformToApply.Alpha;
                    byte multipliedAlpha = (byte)(alpha * 255);
                    child.Alpha = multipliedAlpha;
                }

                changeAngle = a.WillChangeAngle();
                if (changeAngle)
                    Canvas.Transform = transformToApply.Matrix;

                child.InternalPaint();
                Canvas.ResetTransform();
            }
            else
                child.InternalPaint();

            if (a != null && !more)
            {
                child.Alpha = 255;
                FinishAnimatingVisual(child, a);
            }

            return more;
        }

        private void FinishAnimatingVisual(Visual child, Animation animation)
        {
            if (animation != null && !animation.FillAfter)
            {
                child.ClearAnimation();
            }
        }

        protected void MoveControl(Visual visual, int left, int top)
        {
            if (visual != null)
                visual.MoveToInternal(left, top);
        }

        protected void ResizeControl(Visual visual, int width, int height)
        {
            if (visual != null)
                visual.SizeToInternal(width, height);
        }

        public Visual Control(int index)
        {
            if (index < 0)
                return null;
            if (index >= controls.Count)
                return null;
            return controls[index];
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.CN_DRAW_BACKGROUND:
                    {
                        RedrawBackgroundInternal();
                        return;
                    }
                case NativeMethods.CN_UPDATEPARENT:
                    UpdateInternal(false);
                    return;
                case NativeMethods.CN_REPAINTCONTROLS:
                    PaintControls(Control((int)m.WParam));
                    UpdateInternal(false);
                    return;
                default:
                    break;
            }
            base.WndProc(ref m);
        }

        public override void Show()
        {
            base.Show();
            if (liveBackground != null)
                liveBackground.OnVisibilityChanged(true);
        }

        public override void Hide()
        {
            base.Hide();
            if (liveBackground != null)
                liveBackground.OnVisibilityChanged(false);
        }

        protected override void SilentRecreateBuffer()
        {
            if (liveBackground != null)
                liveBackground.OnCanvasDestroyed();
            base.SilentRecreateBuffer();
            if (liveBackground != null)
                liveBackground.OnCanvasCreate();
        }

        #region IDrawableCallback Members

        public void InvalidateDrawable(Drawable who)
        {
            BeginInvoke(InvalidateDrawableInternal, who);
        }

        public void InvalidateDrawableInternal(object param)
        {
            Rectangle clip;
            Drawable who = (Drawable)param;
            if (who != null)
            {
                if (who.Visible)
                {
                    clip = who.GetCorrelatedBounds();
                    Canvas.SetClip(clip);
                    InvalidateInternal(param);
                    Canvas.ResetClip();
                    UpdateInternal(true);
                }
            }
        }

        #endregion
    }
}
