using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the rate of change starts out slowly and then accelerates.
    /// </summary>
    public class AccelerateInterpolator : Interpolator
    {
        private float factor;
        private float doubleFactor;

        public AccelerateInterpolator()
        {
            factor = 1.0f;
            doubleFactor = 2.0f;
        }

        public AccelerateInterpolator(float factor)
        {
            this.factor = factor;
            doubleFactor = factor * 2;
        }

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            if (factor == 1.0f)
            {
                return input * input;
            }
            else
            {
                return (float)Math.Pow(input, doubleFactor);
            }
        }

        #endregion
    }
}
