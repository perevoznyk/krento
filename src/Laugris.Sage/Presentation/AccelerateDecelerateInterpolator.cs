using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the rate of change starts and ends slowly but accelerates through the middle.
    /// </summary>
    class AccelerateDecelerateInterpolator : Interpolator
    {
        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            return (float)(Math.Cos((input + 1) * Math.PI) / 2.0f) + 0.5f;
        }

        #endregion
    }
}
