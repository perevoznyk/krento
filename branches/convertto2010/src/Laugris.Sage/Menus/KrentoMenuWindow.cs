//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using Accessibility;

namespace Laugris.Sage
{
    internal sealed class KrentoMenuWindow : LayeredWindow
    {
        private const int CM_BASE = 0xB000;
        private const int WM_MENUCLOSE = CM_BASE + 20;
        private const int ActivityTimer = 2;
        private Point mouseTrackPoint;
        private MenuAccessibleObject mao;

        private KrentoMenu menu;

        public KrentoMenuWindow(KrentoMenu menu)
            : base()
        {
            Text = "Krento Menu";
            Name = "KrentoMenu";
            this.menu = menu;
            this.CanDrag = false;
            this.TopMostWindow = true;
        }

        public void StartTrace()
        {
            StartTimer(ActivityTimer, 1000);
        }

        public void StopTrace()
        {
            StopTimer(ActivityTimer);
        }

        protected override void OnTimerTick(TimerEventArgs e)
        {
            if (e.TimerId == ActivityTimer)
            {
                IntPtr activeWindow = NativeMethods.GetForegroundWindow();
                if (activeWindow != this.Handle)
                {
                    if (menu.Active)
                        menu.CloseUp();
                }
            }

            base.OnTimerTick(e);
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            KrentoMenuItem item = menu.FindItemByShortCut(e.KeyData);
            if (item != null)
            {
                menu.CloseUp();
                menu.Execute(item);
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Home:
                    menu.SelectItem(0);
                    break;
                case Keys.End:
                    menu.SelectItem(menu.Items.Count - 1);
                    break;
                case Keys.Space:
                case Keys.Return:
                    menu.CloseUp();
                    menu.Execute();
                    break;
                case Keys.Up:
                    {
                        menu.DoArrowUp();
                        break;
                    }
                case Keys.Down:
                    {
                        menu.DoArrowDown();
                        break;
                    }
                case Keys.PageDown:
                    {
                        menu.DoPageDown();
                        break;
                    }
                case Keys.PageUp:
                    {
                        menu.DoPageUp();
                        break;
                    }
                case Keys.Apps:
                case Keys.Escape:
                    {
                        menu.CloseUp();
                        break;
                    }
            }
        }


        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (menu.DoMouseClick(e.X, e.Y))
            {
                this.PostMessageToSelf(WM_MENUCLOSE, (IntPtr)e.X, (IntPtr)e.Y);
            }
        }


        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (menu.Active)
            {
                if (mouseTrackPoint.Y < 0)
                {
                    mouseTrackPoint.X = e.X;
                    mouseTrackPoint.Y = e.Y;
                }
                else
                {
                    if ((Math.Abs(e.Y - mouseTrackPoint.Y) > 3) || (mouseTrackPoint.X < 0))
                    {
                        menu.DoMouseMove(e.X, e.Y);
                        mouseTrackPoint.X = e.X;
                        mouseTrackPoint.Y = e.Y;
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            menu.DoMouseLeave();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            menu.DoMouseWheel(e.Delta);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            mouseTrackPoint = new Point(-1, -1);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                StopTimer(ActivityTimer);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case WM_MENUCLOSE:
                    {
                        menu.CloseUp();
                        menu.Execute();
                    }
                    break;
                case NativeMethods.WM_ACTIVATEAPP:
                    if (IntPtr.Zero == m.WParam)
                    {
                        if (menu.Active)
                            menu.CloseUp();
                    }
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    menu.CloseUp();
                    m.Result = (IntPtr)1;
                    return;
                case NativeMethods.WM_GETOBJECT:
                    WmGetObject(ref m);
                    return;


            }
            base.WndProc(ref m);
        }

        private void WmGetObject(ref Message m)
        {
            AccessibleObject accessibilityObject = this.GetAccessibilityObject((int)((long)m.LParam));

            if (accessibilityObject != null)
            {
                Guid refiid = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");
                IAccessible o = accessibilityObject;

                if (o == null)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
                else
                {
                    IntPtr iUnknownForObject = Marshal.GetIUnknownForObject(accessibilityObject);
                    try
                    {
                        m.Result = NativeMethods.LresultFromObject(ref refiid, m.WParam, new HandleRef(this, iUnknownForObject));
                    }
                    finally
                    {
                        Marshal.Release(iUnknownForObject);
                    }
                }
                return;
            }
            base.WndProc(ref m);
        }

        public AccessibleObject AccessibilityObject
        {
            get
            {
                if (mao == null)
                {
                    mao = new MenuAccessibleObject(this.menu);
                }
                return mao;
            }
        }

        private AccessibleObject GetAccessibilityObject(int accObjId)
        {
            switch (accObjId)
            {
                case -4:
                    return this.AccessibilityObject;

                case 0:
                    return this.AccessibilityObject;
            }
            if (accObjId > 0)
            {
                return this.GetAccessibilityObjectById(accObjId);
            }
            return null;
        }

        private AccessibleObject GetAccessibilityObjectById(int accObjId)
        {
            return null;
        }

    }
}
