using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public interface IDrawableCallback
    {
        /// <summary>
        /// Called when the drawable needs to be redrawn.  A view at this point
        /// should invalidate itself (or at least the part of itself where the
        /// drawable appears).
        /// </summary>
        /// <param name="who">The drawable that is requesting the update.</param>
        void InvalidateDrawable(Drawable who);
    }
}
