using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An animation that controls the alpha level of an object.
    /// Useful for fading things in and out.
    /// </summary>
    public class AlphaAnimation : Animation
    {
        private float mFromAlpha;
        private float mToAlpha;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaAnimation"/> class.
        /// </summary>
        /// <param name="fromAlpha">From alpha.</param>
        /// <param name="toAlpha">To alpha.</param>
        public AlphaAnimation(float fromAlpha, float toAlpha)
        {
            TransformationType = TransformationType.Alpha;
            mFromAlpha = fromAlpha;
            mToAlpha = toAlpha;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation transformation)
        {
            float alpha = mFromAlpha;
            transformation.Alpha = alpha + ((mToAlpha - alpha) * interpolatedTime);
        }
    }
}
