using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    public class VisualButton : Visual
    {
        private Bitmap normalFace;
        private Bitmap disabledFace;
        private Bitmap focusedFace;
        private Bitmap pressedFace;
        private VisualButtonState state;

        public VisualButtonState State
        {
            get { return state; }
            set { state = value; }
        }

        protected internal override void OnMouseLeave(EventArgs e)
        {
            state = VisualButtonState.Normal;
            Repaint();
            base.OnMouseLeave(e);
        }

        protected internal override void OnMouseEnter(EventArgs e)
        {
            state = VisualButtonState.Focused;
            Repaint();
            base.OnMouseEnter(e);
        }

        protected internal override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            state = VisualButtonState.Pressed;
            Repaint();
            base.OnMouseDown(e);
        }

        protected internal override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            state = VisualButtonState.Focused;
            Repaint();
            base.OnMouseUp(e);
        }

        public Bitmap NormalFace
        {
            get { return normalFace; }
            set {

                if (normalFace != null)
                {
                    normalFace.Dispose();
                    normalFace = null;
                }
                normalFace = value;
                if (normalFace != null)
                {
                    Width = normalFace.Width;
                    Height = normalFace.Height;
                }
            }
        }

        public Bitmap DisabledFace
        {
            get { return disabledFace; }
            set 
            {
                if (disabledFace != null)
                {
                    disabledFace.Dispose();
                    disabledFace = null;
                }
                disabledFace = value; 
            }
        }

        public Bitmap FocusedFace
        {
            get { return focusedFace; }
            set 
            {
                if (focusedFace != null)
                {
                    focusedFace.Dispose();
                    focusedFace = null;
                }
                focusedFace = value; 
            }
        }

        public Bitmap PressedFace
        {
            get { return pressedFace; }
            set 
            {
                if (pressedFace != null)
                {
                    pressedFace.Dispose();
                    pressedFace = null;
                }
                pressedFace = value; 
            }
        }

        public override void Draw(Graphics canvas)
        {
            Bitmap face = null;
            switch (state)
            {
                case VisualButtonState.Normal:
                    face = normalFace;
                    break;
                case VisualButtonState.Disabled:
                    face = disabledFace;
                    break;
                case VisualButtonState.Focused:
                    face = focusedFace;
                    break;
                case VisualButtonState.Pressed:
                    face = pressedFace;
                    break;
                default:
                    break;
            }

            if (face == null)
                face = normalFace;

            if (face != null)
            {
                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(face, this.BoundsRect);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (normalFace != null)
            {
                normalFace.Dispose();
                normalFace = null;
            }

            if (disabledFace != null)
            {
                disabledFace.Dispose();
                disabledFace = null;
            }

            if (focusedFace != null)
            {
                focusedFace.Dispose();
                focusedFace = null;
            }

            if (pressedFace != null)
            {
                pressedFace.Dispose();
                pressedFace = null;
            }

            base.Dispose(disposing);
        }
    }
}
