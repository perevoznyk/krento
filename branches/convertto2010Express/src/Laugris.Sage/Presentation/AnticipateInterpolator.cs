using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An interpolator where the change starts backward then flings forward.
    /// </summary>
    class AnticipateInterpolator : Interpolator
    {
        private float tension;

        public AnticipateInterpolator()
        {
            tension = 2.0f;
        }

        public AnticipateInterpolator(float tension)
        {
            this.tension = tension;
        }

        #region Interpolator Members

        public float GetInterpolation(float input)
        {
            return input * input * ((tension + 1) * input - tension);
        }

        #endregion
    }
}
