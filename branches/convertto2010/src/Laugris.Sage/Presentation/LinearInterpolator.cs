using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the rate of change is constant
    /// </summary>
    public class LinearInterpolator : Interpolator
    {
        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            return input;
        }

        #endregion
    }
}
