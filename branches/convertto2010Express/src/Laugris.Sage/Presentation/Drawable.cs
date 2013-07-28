using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    public class Drawable : IDisposable
    {
        private bool visible;
        private byte alpha;
        private Rectangle boundsRect;
        private IDrawableCallback callback;

        public Drawable()
        {
            visible = true;
            alpha = 255;
            boundsRect = new Rectangle();
        }


        ~Drawable()
        {
            Dispose(false);
        }

        public virtual Rectangle GetCorrelatedBounds()
        {
            return boundsRect;
        }

        public byte Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public void SetCallBack(IDrawableCallback callback)
        {
            this.callback = callback;
        }

        public void InvalidateSelf()
        {
            if (callback != null)
            {
                callback.InvalidateDrawable(this);
            }
        }

        public virtual void Draw(Graphics canvas)
        {
        }

        public int Tag { get; set; }

        /// <summary>
        /// Occurs when [visible changing].
        /// </summary>
        public event EventHandler VisibleChanging;

        /// <summary>
        /// Raises the <see cref="E:VisibleChanging"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnVisibleChanging(EventArgs e)
        {
            if (VisibleChanging != null)
                VisibleChanging(this, e);
        }

        public virtual bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    OnVisibleChanging(EventArgs.Empty);
                    visible = value;
                }
            }
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Gets or sets the width of the visual element.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return boundsRect.Width; }
            set
            {
                if (boundsRect.Width != value)
                {
                    SetBounds(boundsRect.Left, boundsRect.Top, value, boundsRect.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the visual element.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return boundsRect.Height; }
            set
            {
                if (boundsRect.Height != value)
                {
                    SetBounds(boundsRect.Left, boundsRect.Top, boundsRect.Width, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the left coordinate of the visual element.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get { return boundsRect.Left; }
            set
            {
                if (boundsRect.Left != value)
                {
                    SetBounds(value, boundsRect.Top, boundsRect.Width, boundsRect.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets the top coordinate of the visual element.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get { return boundsRect.Top; }
            set
            {
                if (boundsRect.Top != value)
                {
                    SetBounds(boundsRect.Left, value, boundsRect.Width, boundsRect.Height);
                }
            }
        }

        public void MoveTo(int left, int top)
        {
            SetBounds(left, top, boundsRect.Width, boundsRect.Height);
        }

        public void SetSize(int width, int height)
        {
            SetBounds(boundsRect.Left, boundsRect.Top, width, height);
        }

        internal void MoveToInternal(int left, int top)
        {
            boundsRect.X = left;
            boundsRect.Y = top;
        }

        protected internal virtual void SizeToInternal(int width, int height)
        {
            boundsRect.Width = width;
            boundsRect.Height = height;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get { return boundsRect.Location; }
        }

        /// <summary>
        /// Gets or sets the bounds rect.
        /// </summary>
        /// <value>The bounds rect.</value>
        public Rectangle BoundsRect
        {
            get { return boundsRect; }
            set
            {
                SetBounds(value.Left, value.Top, value.Width, value.Height);
            }
        }

        /// <summary>
        /// Gets the client rectangle in the coordinates of the visual element.
        /// The bounds rectangle returns the bound of the element on the surface of the parent window, 
        /// the client rectangle returns the dimensions of the element.
        /// </summary>
        /// <value>The client rect.</value>
        public Rectangle ClientRect
        {
            get { return new Rectangle(0, 0, boundsRect.Width, boundsRect.Height); }
        }

        /// <summary>
        /// Sets the bounds.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public virtual void SetBounds(int left, int top, int width, int height)
        {
            boundsRect.X = left;
            boundsRect.Y = top;
            boundsRect.Width = width;
            boundsRect.Height = height;
            OnResize(EventArgs.Empty);
        }

        protected internal void SetBoundsInternal(int left, int top, int width, int height)
        {
            boundsRect.X = left;
            boundsRect.Y = top;
            boundsRect.Width = width;
            boundsRect.Height = height;
        }

        protected internal void SetBoundsInternal(Rectangle value)
        {
            boundsRect.X = value.Left;
            boundsRect.Y = value.Top;
            boundsRect.Width = value.Width;
            boundsRect.Height = value.Height;
        }

        /// <summary>
        /// Occurs when [resize].
        /// </summary>
        public event EventHandler Resize;

        /// <summary>
        /// Raises the <see cref="E:Resize"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnResize(EventArgs e)
        {
            if (Resize != null)
                Resize(this, e);
        }


    }
}
