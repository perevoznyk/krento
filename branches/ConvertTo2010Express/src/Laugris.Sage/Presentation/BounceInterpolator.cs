using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the change bounces at the end.
    /// </summary>
    public class BounceInterpolator : Interpolator
    {
        private static float bounce(float t)
        {
            return t * t * 8.0f;
        }

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            input *= 1.1226f;
            if (input < 0.3535f) return bounce(input);
            else if (input < 0.7408f) return bounce(input - 0.54719f) + 0.7f;
            else if (input < 0.9644f) return bounce(input - 0.8526f) + 0.9f;
            else return bounce(input - 1.0435f) + 0.95f;
        }

        #endregion
    }
}
