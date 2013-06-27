namespace Laugris.Sage
{
    using System;
    using System.Windows.Forms;

    internal interface ISupportOleDropSource
    {
        void OnGiveFeedback(GiveFeedbackEventArgs gfbevent);
        void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent);
    }
}

