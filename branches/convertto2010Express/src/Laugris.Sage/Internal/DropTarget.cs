//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Windows.Forms;

namespace Laugris.Sage
{
    internal class DropTarget : IOleDropTarget
    {
        private System.Windows.Forms.IDataObject lastDataObject;
        private DragDropEffects lastEffect;
        private IDropTarget owner;

        public DropTarget(IDropTarget owner)
        {
            this.owner = owner;
        }

        public void ClearOwner()
        {
            this.owner = null;
        }

        private DragEventArgs CreateDragEventArgs(object pDataObj, int grfKeyState, POINT pt, int pdwEffect)
        {
            System.Windows.Forms.IDataObject data = null;

            if (pDataObj == null)
            {
                data = this.lastDataObject;
            }
            else if (pDataObj is System.Windows.Forms.IDataObject)
            {
                data = (System.Windows.Forms.IDataObject)pDataObj;
            }
            else if (pDataObj is System.Runtime.InteropServices.ComTypes.IDataObject)
            {
                data = new DataObject(pDataObj);
            }
            else
            {
                TraceDebug.Trace("CreateDragEventArgs returns null");
                return null;
            }
            
            DragEventArgs args = new DragEventArgs(data, grfKeyState, pt.x, pt.y, (DragDropEffects)pdwEffect, this.lastEffect);
            this.lastDataObject = data;
            return args;
        }

        private static int GetX(long pt)
        {
            return (int)(((ulong)pt) & 0xffffffffL);
        }

        private static int GetY(long pt)
        {
            return (int)(((ulong)(pt >> 0x20)) & 0xffffffffL);
        }

        int IOleDropTarget.OleDragEnter(object pDataObj, int grfKeyState, long pt, ref int pdwEffect)
        {
            TraceDebug.Trace("OleDragEnter");
            POINT pointl = new POINT();
            pointl.x = DropTarget.GetX(pt);
            pointl.y = DropTarget.GetY(pt);

            // this shouldn't happen, but seems to occasionally, so rather than an Assertion that
            // will cause a GPF, we'll try and handle it nicely....
            if (lastDataObject != null)
            {        
                // Drag leave wasn't called, so call it now...
                this.owner.OnDragLeave(EventArgs.Empty);
                lastDataObject = null;
                this.lastEffect = DragDropEffects.None;
            }

            DragEventArgs e = this.CreateDragEventArgs(pDataObj, grfKeyState, pointl, pdwEffect);
            if (e != null)
            {
                this.owner.OnDragEnter(e);
                pdwEffect = (int)e.Effect;
                this.lastEffect = e.Effect;
            }
            else
            {
                pdwEffect = 0;
            }
            return 0;
        }

        int IOleDropTarget.OleDragLeave()
        {
            TraceDebug.Trace("OleDragLeave");
            this.owner.OnDragLeave(EventArgs.Empty);
            lastDataObject = null;
            this.lastEffect = DragDropEffects.None;
            return 0;
        }

        int IOleDropTarget.OleDragOver(int grfKeyState, long pt, ref int pdwEffect)
        {
            TraceDebug.Trace("OleDragOver");
            POINT pointl = new POINT();
            pointl.x = DropTarget.GetX(pt);
            pointl.y = DropTarget.GetY(pt);
            DragEventArgs e = this.CreateDragEventArgs(lastDataObject, grfKeyState, pointl, pdwEffect);
            if (e != null)
            {
                this.owner.OnDragOver(e);
                pdwEffect = (int)e.Effect;
                this.lastEffect = e.Effect;
            }
            else
            {
                pdwEffect = 0;
            }
            return 0;
        }

        int IOleDropTarget.OleDrop(object pDataObj, int grfKeyState, long pt, ref int pdwEffect)
        {
            TraceDebug.Trace("OleDrop");
            POINT pointl = new POINT();
            pointl.x = DropTarget.GetX(pt);
            pointl.y = DropTarget.GetY(pt);
            DragEventArgs e = this.CreateDragEventArgs(pDataObj, grfKeyState, pointl, pdwEffect);
            if (e != null)
            {
                this.owner.OnDragDrop(e);
                pdwEffect = (int)e.Effect;
            }
            else
            {
                pdwEffect = 0;
            }
            this.lastEffect = DragDropEffects.None;
            this.lastDataObject = null;
            return 0;
        }
    }
}
