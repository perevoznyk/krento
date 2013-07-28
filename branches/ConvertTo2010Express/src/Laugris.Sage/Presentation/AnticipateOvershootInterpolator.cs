using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the change starts backward then flings forward and overshoots
    /// the target value and finally goes back to the value.
    /// </summary>
    class AnticipateOvershootInterpolator : Interpolator
    {
        private float tension;

        public AnticipateOvershootInterpolator()
        {
            tension = 2.0f * 1.5f;
        }

        public AnticipateOvershootInterpolator(float tension)
        {
            this.tension = tension;
        }

        private static float Anticipate(float t, float s)
        {
            return t * t * ((s + 1) * t - s);
        }

        private static float Overshoot(float t, float s)
        {
            return t * t * ((s + 1) * t + s);
        }

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            if (input < 0.5f) return 0.5f * Anticipate(input * 2.0f, tension);
            else return 0.5f * (Overshoot(input * 2.0f - 2.0f, tension) + 2.0f);
        }

        #endregion
    }
}
