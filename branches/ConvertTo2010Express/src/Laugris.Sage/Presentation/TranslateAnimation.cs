using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An animation that controls the position of an object.
    /// Changes the object coordinates
    /// </summary>
    public class TranslateAnimation : Animation
    {
        private float mFromXDelta;
        private float mToXDelta;
        private float mFromYDelta;
        private float mToYDelta;

        public TranslateAnimation(float fromXDelta, float toXDelta, float fromYDelta, float toYDelta)
        {
            TransformationType = TransformationType.Offset;
            mFromXDelta = fromXDelta;
            mToXDelta = toXDelta;
            mFromYDelta = fromYDelta;
            mToYDelta = toYDelta;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation transformation)
        {
            float dx = mFromXDelta;
            float dy = mFromYDelta;
            if (mFromXDelta != mToXDelta)
            {
                dx = mFromXDelta + ((mToXDelta - mFromXDelta) * interpolatedTime);
            }
            if (mFromYDelta != mToYDelta)
            {
                dy = mFromYDelta + ((mToYDelta - mFromYDelta) * interpolatedTime);
            }
            transformation.OffsetX = dx;
            transformation.OffsetY = dy;
        }

    }
}
