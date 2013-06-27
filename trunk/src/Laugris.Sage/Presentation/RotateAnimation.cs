using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    /// <summary>
    /// An animation that controls the rotation of an object. This rotation takes
    /// place int the X-Y plane. You can specify the point to use for the center of
    /// the rotation, where (0,0) is the top left point. If not specified, (0,0) is
    /// the default rotation point.
    /// </summary>
    public class RotateAnimation : Animation
    {
        private float mFromDegrees;
        private float mToDegrees;
        private float mPivotX;
        private float mPivotY;

        public RotateAnimation(float fromDegrees, float toDegrees)
        {
            TransformationType = TransformationType.Angle;
            mFromDegrees = fromDegrees;
            mToDegrees = toDegrees;
            mPivotX = 0.0f;
            mPivotY = 0.0f;
        }

        public RotateAnimation(float fromDegrees, float toDegrees, float pivotX, float pivotY)
        {
            TransformationType =  TransformationType.Angle;
            mFromDegrees = fromDegrees;
            mToDegrees = toDegrees;

            mPivotX = pivotX;
            mPivotY = pivotY;
        }


        protected override void ApplyTransformation(float interpolatedTime, Transformation transformation)
        {
            float degrees = mFromDegrees + ((mToDegrees - mFromDegrees) * interpolatedTime);

            if (mPivotX == 0.0f && mPivotY == 0.0f)
            {
                transformation.Matrix.Rotate(degrees);
            }
            else
            {
                
                transformation.Matrix.RotateAt(degrees, new PointF(mPivotX, mPivotY));
                transformation.OffsetX = transformation.Matrix.OffsetX;
                transformation.OffsetY = transformation.Matrix.OffsetY;

            }
        }

    }
}
