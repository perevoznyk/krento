using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;

namespace Laugris.Sage
{
    /// <summary>
    /// An animation that controls the scale of an object. You can specify the point
    /// to use for the center of scaling.
    /// </summary>
    public class ScaleAnimation : Animation
    {
        private float mFromX;
        private float mToX;
        private float mFromY;
        private float mToY;
        private float mPivotX;
        private float mPivotY;


        public ScaleAnimation(float fromX, float toX, float fromY, float toY)
        {
            TransformationType = TransformationType.Bounds;
            mFromX = fromX;
            mToX = toX;
            mFromY = fromY;
            mToY = toY;
        }

        public ScaleAnimation(float fromX, float toX, float fromY, float toY,
                float pivotX, float pivotY)
        {
            TransformationType = TransformationType.Bounds;
            mFromX = fromX;
            mToX = toX;
            mFromY = fromY;
            mToY = toY;

            mPivotX = pivotX;
            mPivotY = pivotY;
        }


        protected override void ApplyTransformation(float interpolatedTime, Transformation transformation)
        {
            float sx = 1.0f;
            float sy = 1.0f;

            if (mFromX != 1.0f || mToX != 1.0f)
            {
                sx = mFromX + ((mToX - mFromX) * interpolatedTime);
            }
            if (mFromY != 1.0f || mToY != 1.0f)
            {
                sy = mFromY + ((mToY - mFromY) * interpolatedTime);
            }

            if (mPivotX == 0 && mPivotY == 0)
            {
                transformation.Matrix.Scale(sx, sy);
            }
            else
            {
                transformation.Matrix.Scale(sx, sy, MatrixOrder.Append);
                transformation.OffsetX = mPivotX * (1 - sx);
                transformation.OffsetY = mPivotY * (1 - sy);
            }
        }

    }
}
