using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the change flings forward and overshoots the last value then comes back
    /// </summary>
    public class OvershootInterpolator : Interpolator
    {
        private const float Tension = 2.0f;

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            input -= 1.0f;
            return input * input * ((Tension + 1) * input + Tension) + 1.0f;

        }

        #endregion
    }
}
