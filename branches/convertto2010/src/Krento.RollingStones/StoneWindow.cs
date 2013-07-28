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
using System.Windows.Forms;
using Laugris.Sage;

namespace Krento.RollingStones
{
    /// <summary>
    /// The visual part of the stone
    /// </summary>
    internal sealed class StoneWindow : LayeredWindow
    {
        private RollingStoneBase stone;
        internal static IntPtr Defer = IntPtr.Zero;


        public StoneWindow(RollingStoneBase stone):base()
        {
            this.stone = stone;
            Text = "Krento Stone";
            Name = "StoneWindow";
        }

        public override void Hide()
        {
            base.Hide();
        }


        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {

                case NativeMethods.CM_UPDATE_ALL:
                    {
                        stone.UpdateInternal(false);
                        return;
                    }
                case NativeMethods.CM_UPDATE_REDRAW:
                    {
                        stone.UpdateInternal(true);
                        return;
                    }
                case NativeMethods.CM_UPDATE_FRAMES:
                    {
                        ImageAnimator.UpdateFrames();
                        return;
                    }
                case NativeMethods.WM_MOUSEACTIVATE:
                    {
                        m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                        return;
                    }
                case NativeMethods.WM_ACTIVATE:
                    {
                        int action = ParamConvertor.LoWord(m.WParam);
                        if (action == NativeMethods.WA_CLICKACTIVE)
                        {
                            m.Result = (IntPtr)1;
                            return;
                        }

                        break;
                    }
            }

            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            stone = null;
            base.Dispose(disposing);
        }

        public static void BeginDeferWindowPos(int numWindows)
        {
            Defer = NativeMethods.BeginDeferWindowPos(numWindows);
        }

        public static void EndDeferWindowPos()
        {
            NativeMethods.EndDeferWindowPos(Defer);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            stone.OnKeyDown(e);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            stone.OnKeyUp(e);
            base.OnKeyUp(e);
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            NativeMethods.NotifyWinEvent(3, this.Handle, 0, 0);
            base.OnMouseEnter(e);
        }

        protected override void HandleTimerTick(int timerNumber)
        {
            stone.HandleTimerTick(timerNumber);
            base.HandleTimerTick(timerNumber);
        }


        public void DeferWindowPos(LightWindow window)
        {
            IntPtr hWndInsertAfter;
            if (window == null)
                hWndInsertAfter = IntPtr.Zero;
            else
                hWndInsertAfter = window.Handle;

            NativeMethods.DeferWindowPos(Defer, this.Handle, hWndInsertAfter, Position.X, Position.Y, 0, 0, (int)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE));
        }

        public override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            stone.OnDragDrop(e);
        }

        public override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            stone.OnDragEnter(e);
        }

        public override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            stone.OnDragLeave(e);
        }

        public override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            stone.OnDragOver(e);
        }
    }
}
