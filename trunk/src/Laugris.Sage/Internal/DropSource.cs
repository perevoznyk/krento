namespace Laugris.Sage
{
    using System;
    using System.Windows.Forms;

    internal class DropSource : IOleDropSource
    {
        private const int DragDropSCancel = 0x40101;
        private const int DragDropSDrop = 0x40100;
        private const int DragDropSUseDefaultCursors = 0x40102;
        private ISupportOleDropSource peer;

        public DropSource(ISupportOleDropSource peer)
        {
            if (peer == null)
            {
                throw new ArgumentNullException("peer");
            }
            this.peer = peer;
        }

        public int OleGiveFeedback(int dwEffect)
        {
            GiveFeedbackEventArgs gfbevent = new GiveFeedbackEventArgs((DragDropEffects) dwEffect, true);
            this.peer.OnGiveFeedback(gfbevent);
            if (gfbevent.UseDefaultCursors)
            {
                return DragDropSUseDefaultCursors;
            }
            return 0;
        }

        public int OleQueryContinueDrag(int fEscapePressed, int grfKeyState)
        {
            QueryContinueDragEventArgs qcdevent = null;
            bool escapePressed = fEscapePressed != 0;
            DragAction cancel = DragAction.Continue;
            if (escapePressed)
            {
                cancel = DragAction.Cancel;
            }
            else if ((((grfKeyState & 1) == 0) && ((grfKeyState & 2) == 0)) && ((grfKeyState & 0x10) == 0))
            {
                cancel = DragAction.Drop;
            }
            qcdevent = new QueryContinueDragEventArgs(grfKeyState, escapePressed, cancel);
            this.peer.OnQueryContinueDrag(qcdevent);
            switch (qcdevent.Action)
            {
                case DragAction.Drop:
                    return DragDropSDrop;

                case DragAction.Cancel:
                    return DragDropSCancel;
            }
            return 0;
        }
    }
}

