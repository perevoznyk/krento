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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Laugris.Sage
{

    /// <summary>
    /// Provides data for the Draw event.
    /// </summary>
    public class DrawEventArgs : EventArgs, IDisposable
    {
        private Graphics graphics;
        private DockItem item;

        public DrawEventArgs(Graphics graphics, DockItem item)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            this.graphics = graphics;
            this.item = item;
        }

        /// <summary>
        /// Gets the graphics used to paint.
        /// </summary>
        /// <value>
        /// The Graphics object used to paint. The Graphics object provides methods for drawing objects on the display device.
        /// </value>
        public Graphics Graphics
        {
            get { return graphics; }
        }

        /// <summary>
        /// Gets the Dock item.
        /// </summary>
        /// <value>The Dock item.</value>
        public DockItem Item
        {
            get { return item; }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DrawEventArgs"/> is reclaimed by garbage collection.
        /// </summary>
        ~DrawEventArgs()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if ((disposing && (this.graphics != null)))
            {
                this.graphics.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }

    /// <summary>
    /// The base class for all Dock Painters
    /// </summary>
    public abstract class CustomDockPainter : IDockPainter
    {
        private Font font = TextPainter.DefaultFont;
        private Color foreColor = TextPainter.DefaultColor;

        /// <summary>
        /// Occurs when custom drawing for dock item is needed.
        /// </summary>
        public event EventHandler<DrawEventArgs> Draw;

        protected void DoDraw(Graphics g, DockItem item)
        {
            DrawEventArgs e = new DrawEventArgs(g, item);
            OnDraw(e);
        }

        /// <summary>
        /// Raises the <see cref="E:Draw"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Karna.Windows.UI.Dock.DrawEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDraw(DrawEventArgs e)
        {
            if (Draw != null)
                Draw(this, e);
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font
        {
            get { return this.font; }
            set { this.font = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color
        /// </summary>
        /// <value>The color of the fore.</value>
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        /// <summary>
        /// Does the default paint.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="manager">The manager.</param>
        protected virtual void DoDefaultPaint(Graphics canvas, IDockManager manager)
        {
            //default painting of the items is defined in derived classes
        }

        /// <summary>
        /// Paints the dock items using provided event handler for painting
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="manager">The manager.</param>
        protected virtual void DoCustomPaint(Graphics canvas, IDockManager manager)
        {
            for (int i = 0; i < manager.Items.Count; i++)
            {
                DoDraw(canvas, manager.Items[i]);
            }
        }

        /// <summary>
        /// Paints the dock items
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="manager">The manager.</param>
        public virtual void Paint(Graphics canvas, IDockManager manager)
        {
            if (Draw != null)
                DoCustomPaint(canvas, manager);
            else
                DoDefaultPaint(canvas, manager);
        }
    }
}
