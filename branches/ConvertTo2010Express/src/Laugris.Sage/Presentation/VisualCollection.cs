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
    public sealed class VisualCollection : List<UIElement>
    {
        private Dictionary<string, int> nameList;
        private UIElement mouseControl;

        public event EventHandler Invalidate;

        public VisualCollection()
        {
            this.nameList = new Dictionary<string, int>();
        }

        public UIElement MouseControl
        {
            get { return mouseControl; }
            set { mouseControl = value; }
        }

        public T Add<T>(T element) where T : UIElement
        {
            this.Add(element as UIElement);
            return element;
        }

        public new void Add(UIElement element)
        {
            if (!string.IsNullOrEmpty(element.Name))
            {
                nameList.Add(element.Name, this.Count);
            }
            element.Parent = this;
            element.Invalidate += new EventHandler(element_Invalidate);
            base.Add(element);
        }

        private void element_Invalidate(object sender, EventArgs e)
        {
            if (Invalidate != null)
                Invalidate(sender, e);
        }

        public void Repaint()
        {
            if (Invalidate != null)
                Invalidate(this, EventArgs.Empty);
        }

        public void Draw(Graphics graphics)
        {
            OnRender(graphics);
        }

        public void Render(Graphics graphics)
        {
            OnRender(graphics);
        }

        public void OnRender(Graphics graphics)
        {
            // Pass the graphics objects to all UI elements
            for (int i = 0; i < this.Count; i++)
            {
                UIElement element = this[i];
                if (element.Visible)
                {
                    element.Render(graphics);
                }
            }
        }

        public UIElement this[string name]
        {
            get
            {
                try
                {
                    if (nameList.ContainsKey(name))
                        return this[nameList[name]];
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace("The visual element [UIElement] with name " + name + " was not found " + ex.Message);
                    return null;
                }

            }
        }


        public void OnClick(MouseEventArgs e)
        {
            foreach (UIElement element in this)
            {
                if (element.Visible)
                {
                    if (element.HitTest(new Point(e.X, e.Y)))
                    {
                        element.OnClick(EventArgs.Empty);
                    }
                }
            }
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            foreach (UIElement element in this)
            {
                if (element.Visible)
                {
                    if (element.HitTest(new Point(e.X, e.Y)))
                    {
                        element.OnMouseDown(e);
                    }
                }
            }
        }

        public UIElement FindAtCursor(Point p)
        {
            foreach (UIElement element in this)
            {
                if (element.HitTest(p))
                {
                    return element;
                }
            }

            return null;
        }

        public void MouseLeave()
        {
            UIElement Target = null;
            if (MouseControl != Target)
            {
                if (MouseControl != null)
                    MouseControl.OnMouseLeave(EventArgs.Empty);
                MouseControl = Target;
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            UIElement Target = FindAtCursor(e.Location);
            if (MouseControl != Target)
            {
                if (MouseControl != null)
                    MouseControl.OnMouseLeave(EventArgs.Empty);

                if (Target != null)
                    Target.OnMouseEnter(EventArgs.Empty);

                MouseControl = Target;
            }

            foreach (UIElement element in this)
            {
                if (element.Visible)
                {
                    if (element.HitTest(new Point(e.X, e.Y)))
                    {
                        element.OnMouseMove(e);
                    }
                }
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            foreach (UIElement element in this)
            {
                if (element.Visible)
                {
                    if (element.HitTest(new Point(e.X, e.Y)))
                    {
                        element.OnMouseUp(e);
                    }
                }
            }
        }


    }
}
