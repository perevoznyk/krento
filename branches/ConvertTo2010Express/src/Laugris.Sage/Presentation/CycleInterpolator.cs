using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// Repeats the animation for a specified number of cycles. The rate of change follows a sinusoidal pattern.
    /// </summary>
    public class CycleInterpolator : Interpolator
    {
        private float cycles;

        public CycleInterpolator(float cycles)
        {
            this.cycles = cycles;
        }

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            return (float)(Math.Sin(2 * cycles * Math.PI * input));
        }

        #endregion
    }
}
