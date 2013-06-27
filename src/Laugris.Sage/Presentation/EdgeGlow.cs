using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    public class EdgeGlow
    {
        // Time it will take the effect to fully recede in ms
        private static int RECEDE_TIME = 1000;

        // Time it will take before a pulled glow begins receding
        private static int PULL_TIME = 167;

        // Time it will take for a pulled glow to decay to partial strength before release
        private static int PULL_DECAY_TIME = 1000;

        private static float MAX_ALPHA = 0.8f;
        private static float HELD_EDGE_SCALE_Y = 0.5f;

        private static float MAX_GLOW_HEIGHT = 3.0f;

        private static float PULL_GLOW_BEGIN = 1.0f;
        private static float PULL_EDGE_BEGIN = 0.6f;

        // Minimum velocity that will be absorbed
        private static int MIN_VELOCITY = 100;

        private static float EPSILON = 0.001f;

        private Drawable edgeDraw;
        private Drawable glowDraw;
        private int mWidth;
        private int mHeight;

        private float mEdgeAlpha;
        private float mEdgeScaleY;
        private float mGlowAlpha;
        private float mGlowScaleY;

        private float mEdgeAlphaStart;
        private float mEdgeAlphaFinish;
        private float mEdgeScaleYStart;
        private float mEdgeScaleYFinish;
        private float mGlowAlphaStart;
        private float mGlowAlphaFinish;
        private float mGlowScaleYStart;
        private float mGlowScaleYFinish;

        private long mStartTime;
        private float mDuration;

        private Interpolator mInterpolator;

        private const int STATE_IDLE = 0;
        private const int STATE_PULL = 1;
        private const int STATE_ABSORB = 2;
        private const int STATE_RECEDE = 3;
        private const int STATE_PULL_DECAY = 4;

        // How much dragging should effect the height of the edge image.
        // Number determined by user testing.
        private const int PULL_DISTANCE_EDGE_FACTOR = 5;

        // How much dragging should effect the height of the glow image.
        // Number determined by user testing.
        private const int PULL_DISTANCE_GLOW_FACTOR = 5;
        private const float PULL_DISTANCE_ALPHA_GLOW_FACTOR = 0.8f;

        private const int VELOCITY_EDGE_FACTOR = 8;
        private const int VELOCITY_GLOW_FACTOR = 16;

        private int mState = STATE_IDLE;

        private float mPullDistance;

        public EdgeGlow(Visual edge, Visual glow)
        {
            edgeDraw = edge;
            glowDraw = glow;

            mInterpolator = new DecelerateInterpolator();
        }

        public void SetSize(int width, int height)
        {
            mWidth = width;
            mHeight = height;
        }

        public bool IsFinished()
        {
            return mState == STATE_IDLE;
        }

        public void Finish()
        {
            mState = STATE_IDLE;
        }

        /// <summary>
        /// Call when the object is pulled by the user.
        /// </summary>
        /// <param name="deltaDistance">Change in distance since the last call</param>
        public void OnPull(float deltaDistance)
        {
            long now = SystemClock.UptimeMillis();
            if (mState == STATE_PULL_DECAY && now - mStartTime < mDuration)
            {
                return;
            }
            if (mState != STATE_PULL)
            {
                mGlowScaleY = PULL_GLOW_BEGIN;
            }
            mState = STATE_PULL;

            mStartTime = now;
            mDuration = PULL_TIME;

            mPullDistance += deltaDistance;
            float distance = Math.Abs(mPullDistance);

            mEdgeAlpha = mEdgeAlphaStart = Math.Max(PULL_EDGE_BEGIN, Math.Min(distance, MAX_ALPHA));
            mEdgeScaleY = mEdgeScaleYStart = Math.Max(
                    HELD_EDGE_SCALE_Y, Math.Min(distance * PULL_DISTANCE_EDGE_FACTOR, 1.0f));

            mGlowAlpha = mGlowAlphaStart = Math.Min(MAX_ALPHA,
                    mGlowAlpha +
                    (Math.Abs(deltaDistance) * PULL_DISTANCE_ALPHA_GLOW_FACTOR));

            float glowChange = Math.Abs(deltaDistance);
            if (deltaDistance > 0 && mPullDistance < 0)
            {
                glowChange = -glowChange;
            }
            if (mPullDistance == 0)
            {
                mGlowScaleY = 0;
            }

            // Do not allow glow to get larger than MAX_GLOW_HEIGHT.
            mGlowScaleY = mGlowScaleYStart = Math.Min(MAX_GLOW_HEIGHT, Math.Max(
                    0, mGlowScaleY + glowChange * PULL_DISTANCE_GLOW_FACTOR));

            mEdgeAlphaFinish = mEdgeAlpha;
            mEdgeScaleYFinish = mEdgeScaleY;
            mGlowAlphaFinish = mGlowAlpha;
            mGlowScaleYFinish = mGlowScaleY;
        }

        /**
         * Call when the object is released after being pulled.
         */
        public void OnRelease()
        {
            mPullDistance = 0;

            if (mState != STATE_PULL && mState != STATE_PULL_DECAY)
            {
                return;
            }

            mState = STATE_RECEDE;
            mEdgeAlphaStart = mEdgeAlpha;
            mEdgeScaleYStart = mEdgeScaleY;
            mGlowAlphaStart = mGlowAlpha;
            mGlowScaleYStart = mGlowScaleY;

            mEdgeAlphaFinish = 0.0f;
            mEdgeScaleYFinish = 0.0f;
            mGlowAlphaFinish = 0.0f;
            mGlowScaleYFinish = 0.0f;

            mStartTime = SystemClock.UptimeMillis();
            mDuration = RECEDE_TIME;
        }

        /**
         * Call when the effect absorbs an impact at the given velocity.
         *
         * @param velocity Velocity at impact in pixels per second.
         */
        public void OnAbsorb(int velocity)
        {
            mState = STATE_ABSORB;
            velocity = Math.Max(MIN_VELOCITY, Math.Abs(velocity));

            mStartTime = SystemClock.UptimeMillis();
            mDuration = 0.1f + (velocity * 0.03f);

            // The edge should always be at least partially visible, regardless
            // of velocity.
            mEdgeAlphaStart = 0.0f;
            mEdgeScaleY = mEdgeScaleYStart = 0.0f;
            // The glow depends more on the velocity, and therefore starts out
            // nearly invisible.
            mGlowAlphaStart = 0.5f;
            mGlowScaleYStart = 0.0f;

            // Factor the velocity by 8. Testing on device shows this works best to
            // reflect the strength of the user's scrolling.
            mEdgeAlphaFinish = Math.Max(0, Math.Min(velocity * VELOCITY_EDGE_FACTOR, 1));
            // Edge should never get larger than the size of its asset.
            mEdgeScaleYFinish = Math.Max(
                    HELD_EDGE_SCALE_Y, Math.Min(velocity * VELOCITY_EDGE_FACTOR, 1.0f));

            // Growth for the size of the glow should be quadratic to properly
            // respond
            // to a user's scrolling speed. The faster the scrolling speed, the more
            // intense the effect should be for both the size and the saturation.
            mGlowScaleYFinish = Math.Min(0.025f + (velocity * (velocity / 100) * 0.00015f), 1.75f);
            // Alpha should change for the glow as well as size.
            mGlowAlphaFinish = Math.Max(
                    mGlowAlphaStart, Math.Min(velocity * VELOCITY_GLOW_FACTOR * .00001f, MAX_ALPHA));
        }


        /**
         * Draw into the provided canvas. Assumes that the canvas has been rotated
         * accordingly and the size has been set. The effect will be drawn the full
         * width of X=0 to X=width, emitting from Y=0 and extending to some factor <
         * 1.f of height.
         *
         * @param canvas Canvas to draw into
         * @return true if drawing should continue beyond this frame to continue the
         *         animation
         */
        public bool Draw(Graphics canvas)
        {
            Update();

            int edgeHeight = edgeDraw.Height;
            int glowHeight = glowDraw.Height;

            float distScale = (float)mHeight / mWidth;

            glowDraw.Alpha = (byte)(Math.Max(0, Math.Min(mGlowAlpha, 1)) * 255);
            // Width of the image should be 3 * the width of the screen.
            // Should start off screen to the left.
            glowDraw.SetBounds(-mWidth, 0, mWidth * 2, (int)Math.Min(
                    glowHeight * mGlowScaleY * distScale * 0.6f, mHeight * MAX_GLOW_HEIGHT));
            glowDraw.Draw(canvas);

            edgeDraw.Alpha = (byte)(Math.Max(0, Math.Min(mEdgeAlpha, 1)) * 255);
            edgeDraw.SetBounds(0, 0, mWidth, (int)(edgeHeight * mEdgeScaleY));
            edgeDraw.Draw(canvas);

            return mState != STATE_IDLE;
        }

        private void Update()
        {
            long time = SystemClock.UptimeMillis();
            float t = Math.Min((time - mStartTime) / mDuration, 1.0f);

            float interp = mInterpolator.GetInterpolation(t);

            mEdgeAlpha = mEdgeAlphaStart + (mEdgeAlphaFinish - mEdgeAlphaStart) * interp;
            mEdgeScaleY = mEdgeScaleYStart + (mEdgeScaleYFinish - mEdgeScaleYStart) * interp;
            mGlowAlpha = mGlowAlphaStart + (mGlowAlphaFinish - mGlowAlphaStart) * interp;
            mGlowScaleY = mGlowScaleYStart + (mGlowScaleYFinish - mGlowScaleYStart) * interp;

            if (t >= 1.0f - EPSILON)
            {
                switch (mState)
                {
                    case STATE_ABSORB:
                        mState = STATE_RECEDE;
                        mStartTime = SystemClock.UptimeMillis();
                        mDuration = RECEDE_TIME;

                        mEdgeAlphaStart = mEdgeAlpha;
                        mEdgeScaleYStart = mEdgeScaleY;
                        mGlowAlphaStart = mGlowAlpha;
                        mGlowScaleYStart = mGlowScaleY;

                        // After absorb, the glow and edge should fade to nothing.
                        mEdgeAlphaFinish = 0.0f;
                        mEdgeScaleYFinish = 0.0f;
                        mGlowAlphaFinish = 0.0f;
                        mGlowScaleYFinish = 0.0f;
                        break;
                    case STATE_PULL:
                        mState = STATE_PULL_DECAY;
                        mStartTime = SystemClock.UptimeMillis();
                        mDuration = PULL_DECAY_TIME;

                        mEdgeAlphaStart = mEdgeAlpha;
                        mEdgeScaleYStart = mEdgeScaleY;
                        mGlowAlphaStart = mGlowAlpha;
                        mGlowScaleYStart = mGlowScaleY;

                        // After pull, the glow and edge should fade to nothing.
                        mEdgeAlphaFinish = 0.0f;
                        mEdgeScaleYFinish = 0.0f;
                        mGlowAlphaFinish = 0.0f;
                        mGlowScaleYFinish = 0.0f;
                        break;
                    case STATE_PULL_DECAY:
                        // When receding, we want edge to decrease more slowly
                        // than the glow.
                        float factor = mGlowScaleYFinish != 0 ? 1
                                / (mGlowScaleYFinish * mGlowScaleYFinish)
                                : float.MaxValue;
                        mEdgeScaleY = mEdgeScaleYStart +
                            (mEdgeScaleYFinish - mEdgeScaleYStart) *
                                interp * factor;
                        break;
                    case STATE_RECEDE:
                        mState = STATE_IDLE;
                        break;
                }
            }
        }
    }
}