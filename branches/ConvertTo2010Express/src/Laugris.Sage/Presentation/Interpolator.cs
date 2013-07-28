using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator defines the rate of change of an animation. This allows
    /// the basic animation effects (alpha, scale, translate, rotate) to be 
    /// accelerated, decelerated, repeated, etc.
    /// </summary>
    public interface Interpolator
    {
        float GetInterpolation(float input);
    }
}
