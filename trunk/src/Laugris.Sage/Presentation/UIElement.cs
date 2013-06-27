//--------------------------------------------------------------------- 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY 
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//PARTICULAR PURPOSE. 
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Laugris.Sage
{
    public abstract class UIElement : IDisposable
    {
        #region events

        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event EventHandler Click;
        public event EventHandler Invalidate;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;

        #endregion

        #region fields

        private bool visible;
        private string name;

        private Rectangle bounds = new Rectangle();


        private Color foreground;
        private Color background;

        private VisualCollection parent;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UIElement"/> class.
        /// </summary>
        public UIElement()
        {
            this.visible = true;
            this.name = "";
        }

        #endregion

        #region public and protected methods

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UIElement"/> is reclaimed by garbage collection.
        /// </summary>
        ~UIElement()
        {
            Dispose(false);
        }

        /// <summary>
        /// Adjusts the bounds.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AdjustBounds(int left, int top, int width, int height)
        {
            this.bounds.X = left;
            this.bounds.Y = top;
            this.bounds.Width = width;
            this.bounds.Height = height;
        }

        /// <summary>
        /// Adjusts the bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public void AdjustBounds(Rectangle bounds)
        {
            this.bounds = bounds;
        }

        /// <summary>
        /// Detects a hit test.
        /// </summary>
        /// <param name="point">a point to detect</param>
        /// <returns>True: if the UIElement was hit.</returns>
        public bool HitTest(Point point)
        {            
            return this.Rectangle.Contains(point);
        }

        /// <summary>
        /// Renders the UIElement.
        /// </summary>
        /// <param name="graphics">Graphics object.</param>
        public void Render(Graphics graphics)
        {
            if (visible)
            {
                this.OnRender(graphics);
            }
        }

        /// <summary>
        /// Draws to the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        public void Draw(Graphics graphics)
        {
            if (visible)
            {
                this.OnRender(graphics);
            }
        }

        /// <summary>
        /// Render method to implement by inheritors
        /// </summary>
        /// <param name="graphics">Graphics object.</param>
        protected virtual void OnRender(Graphics graphics)
        {

        }

        /// <summary>
        /// Repaints this element. Repaint calls Invalidate method
        /// </summary>
        public void Repaint()
        {
            OnInvalidate();
        }

        /// <summary>
        /// Called when invalidate of the drawing surface is needed.
        /// </summary>
        protected virtual void OnInvalidate()
        {
            if (this.Invalidate != null)
            {
                this.Invalidate(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Click"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnClick(EventArgs e)
        {
            if (visible)
            {
                if (this.Click != null)
                {
                    this.Click(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseDown(MouseEventArgs e)
        {
            if (visible)
            {
                if (this.MouseDown != null)
                {
                    this.MouseDown(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseUp(MouseEventArgs e)
        {
            if (visible)
            {
                if (this.MouseUp != null)
                {
                    this.MouseUp(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseMove(MouseEventArgs e)
        {
            if (visible)
            {
                if (this.MouseMove != null)
                {
                    this.MouseMove(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseEnter(EventArgs e)
        {
            if (visible)
            {
                if (this.MouseEnter != null)
                {
                    this.MouseEnter(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnMouseLeave(EventArgs e)
        {
            if (visible)
            {
                if (this.MouseLeave != null)
                {
                    this.MouseLeave(this, e);
                }
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get
            {
                return bounds.X;
            }
            set
            {
                bounds.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get
            {
                return bounds.Y;
            }
            set
            {
                bounds.Y = value;
            }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public Rectangle Rectangle
        {
            get
            {
                return bounds;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return bounds.Height;
            }
            set
            {
                bounds.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public VisualCollection Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return bounds.Width;
            }
            set
            {
                bounds.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UIElement"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>The foreground color.</value>
        public Color Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                if (this.foreground != value)
                {
                    this.foreground = value;
                    this.OnInvalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The background color.</value>
        public Color Background
        {
            get
            {
                return background;
            }
            set
            {
                if (this.background != value)
                {
                    this.background = value;
                    this.OnInvalidate();
                }
            }
        }

        #endregion


        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
