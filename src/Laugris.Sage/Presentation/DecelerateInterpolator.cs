using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the rate of change starts out quickly and then decelerates.
    /// </summary>
    public class DecelerateInterpolator : Interpolator
    {
        private float factor = 1.0f;

        public DecelerateInterpolator()
        {
        }

        public DecelerateInterpolator(float factor)
        {
            this.factor = factor;
        }

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            if (factor == 1.0f)
            {
                return (float)(1.0f - (1.0f - input) * (1.0f - input));
            }
            else
            {
                return (float)(1.0f - Math.Pow((1.0f - input), 2 * factor));
            }
        }

        #endregion
    }
}
