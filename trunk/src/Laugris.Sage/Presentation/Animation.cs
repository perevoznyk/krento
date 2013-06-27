using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    /// <summary>
    /// Abstraction for an Animation that can be applied to Visuals
    /// </summary>
    public abstract class Animation
    {
        private Interpolator interpolator;
        private IAnimationListener listener;
        private int startTime = -1;
        private bool more = true;
        private int duration;
        private int startOffset;
        private int repeatCount;
        private bool started;
        private bool oneMoreTime = true;
        private bool ended;
        private bool cycleFlip;
        private bool fillBefore = true;
        private bool fillAfter;
        private bool fillEnabled;
        private int repeated;
        private RepeatMode mRepeatMode = RepeatMode.Restart;
        private bool initialized;
        private Rectangle boundsRect;
        private TransformationType transformationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        protected Animation()
        {
            EnsureInterpolator();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="interpolator">The interpolator.</param>
        protected Animation(Interpolator interpolator)
        {
            this.interpolator = interpolator;
        }

        protected void EnsureInterpolator()
        {
            if (interpolator == null)
            {
                interpolator = new AccelerateDecelerateInterpolator();
            }
        }

        public Visual Target { get; set; }

        public int Tag { get; set; }

        public TransformationType TransformationType
        {
            get { return transformationType; }
            set { transformationType = value; }
        }

        /// <summary>
        /// The time in milliseconds at which the animation must start;
        /// </summary>
        /// <param name="startTimeMillis">The start time millis.</param>
        public void SetStartTime(int startTimeMillis)
        {
            startTime = startTimeMillis;
            started = ended = false;
            cycleFlip = false;
            repeated = 0;
            more = true;
        }


        public void Start()
        {
            SetStartTime(-1);
        }

        public void StartNow()
        {
            SetStartTime(SystemClock.UptimeMillis());
        }

        /// <summary>
        /// The delay in milliseconds after which the animation must start. When the
        /// start offset is > 0, the start time of the animation is startTime + startOffset.
        /// </summary>
        /// <value>The start offset.</value>
        public int StartOffset
        {
            get { return startOffset; }
            set { startOffset = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public void Detach()
        {
            if (started && !ended)
            {
                ended = true;
                if (listener != null)
                    listener.OnAnimationEnd(this);
            }
        }

        public void SetAnimationListener(IAnimationListener listener)
        {
            this.listener = listener;
        }

        public void RestrictDuration(int durationMillis)
        {
            // If we start after the duration, then we just won't run.
            if (startOffset > durationMillis)
            {
                startOffset = durationMillis;
                duration = 0;
                repeatCount = 0;
                return;
            }

            long dur = duration + startOffset;
            if (dur > durationMillis)
            {
                duration = durationMillis - startOffset;
                dur = durationMillis;
            }
            // If the duration is 0 or less, then we won't run.
            if (duration <= 0)
            {
                duration = 0;
                repeatCount = 0;
                return;
            }
            // Reduce the number of repeats to keep below the maximum duration.
            // The comparison between mRepeatCount and duration is to catch
            // overflows after multiplying them.
            if (repeatCount < 0 || repeatCount > durationMillis
                    || (dur * repeatCount) > durationMillis)
            {
                // Figure out how many times to do the animation.  Subtract 1 since
                // repeat count is the number of times to repeat so 0 runs once.
                repeatCount = (int)(durationMillis / dur) - 1;
                if (repeatCount < 0)
                {
                    repeatCount = 0;
                }
            }
        }

        public void ScaleCurrentDuration(float scale)
        {
            duration = (int)(duration * scale);
        }

        public RepeatMode RepeatMode
        {
            get { return mRepeatMode; }
            set { mRepeatMode = value; }
        }

        public int RepeatCount
        {
            get { return repeatCount; }
            set { repeatCount = value; }
        }

        public bool FillEnabled
        {
            get { return fillEnabled; }
            set { fillEnabled = value; }
        }

        public bool FillBefore
        {
            get { return fillBefore; }
            set { fillBefore = value; }
        }

        public bool FillAfter
        {
            get { return fillAfter; }
            set { fillAfter = value; }
        }

        public bool WillChangeBounds()
        {
            return ((transformationType & TransformationType.Bounds) == TransformationType.Bounds);
        }

        public bool WillChangeAngle()
        {
            return ((transformationType & TransformationType.Angle) == TransformationType.Angle);
        }

        public bool WillChangeOffset()
        {
            return ((transformationType & TransformationType.Offset) == TransformationType.Offset);
        }

        public bool WillChangeAlpha()
        {
            return ((transformationType & TransformationType.Alpha) == TransformationType.Alpha);
        }

        public int ComputeDurationHint()
        {
            return (StartOffset + Duration) * (RepeatCount + 1);
        }

        public bool HasStarted()
        {
            return started;
        }

        public bool HasEnded()
        {
            return ended;
        }


        public void Cancel()
        {
            if (started && !ended)
            {
                if (listener != null)
                    listener.OnAnimationEnd(this);
                ended = true;
            }
            // Make sure we move the animation to the end
            startTime = int.MinValue;
            more = oneMoreTime = false;
        }

        /// <summary>
        /// Gets or sets the interpolator.
        /// </summary>
        /// <value>The interpolator.</value>
        public Interpolator Interpolator
        {
            get { return interpolator; }
            set { interpolator = value; }
        }

        public void Reset()
        {
            cycleFlip = false;
            repeated = 0;
            more = true;
            oneMoreTime = true;
            initialized = false;
        }

        public bool IsInitialized()
        {
            return initialized;
        }

        public virtual void Initialize(Rectangle boundsRect)
        {
            Reset();
            this.boundsRect = boundsRect;
            initialized = true;
        }

        public Rectangle BoundsRect
        {
            get { return boundsRect; }
        }

        /// <summary>
        /// Applies the transformation.
        /// </summary>
        /// <param name="interpolatedTime">The interpolated time.</param>
        /// <param name="t">The t.</param>
        protected virtual void ApplyTransformation(float interpolatedTime, Transformation transformation)
        {
        }


        /// <summary>
        /// Gets the transformation to apply at a specified point in time. Implementations of this
        /// method should always replace the specified Transformation or document they are doing
        /// otherwise.
        /// </summary>
        /// <param name="currentTime">Where we are in the animation. This is wall clock time.</param>
        /// <param name="outTransformation">A tranformation object that is provided by the
        /// caller and will be filled in by the animation.</param>
        /// <returns>True if the animation is still running</returns>
        public bool GetTransformation(int currentTime, Transformation outTransformation)
        {
            if (startTime == -1)
            {
                startTime = currentTime;
            }

            int currentStartOffset = startOffset;
            int currentDuration = duration;
            float normalizedTime;
            if (currentDuration != 0)
            {
                normalizedTime = ((float)(currentTime - (startTime + currentStartOffset))) /
                        (float)currentDuration;
            }
            else
            {
                // time is a step-change with a zero duration
                normalizedTime = currentTime < startTime ? 0.0f : 1.0f;
            }

            bool expired = normalizedTime >= 1.0f;
            more = !expired;

            if (!fillEnabled) normalizedTime = Math.Max(Math.Min(normalizedTime, 1.0f), 0.0f);

            if ((normalizedTime >= 0.0f || fillBefore) && (normalizedTime <= 1.0f || fillAfter))
            {
                if (!started)
                {
                    if (listener != null)
                    {
                        listener.OnAnimationStart(this);
                    }
                    started = true;
                }

                if (fillEnabled) normalizedTime = Math.Max(Math.Min(normalizedTime, 1.0f), 0.0f);

                if (cycleFlip)
                {
                    normalizedTime = 1.0f - normalizedTime;
                }

                float interpolatedTime = interpolator.GetInterpolation(normalizedTime);
                ApplyTransformation(interpolatedTime, outTransformation);
            }

            if (expired)
            {
                if (repeatCount == repeated)
                {
                    if (!ended)
                    {
                        ended = true;
                        if (listener != null)
                        {
                            listener.OnAnimationEnd(this);
                        }
                    }
                }
                else
                {
                    if (repeatCount > 0)
                    {
                        repeated++;
                    }

                    if (mRepeatMode == RepeatMode.Reverse)
                    {
                        cycleFlip = !cycleFlip;
                    }

                    startTime = -1;
                    more = true;
                    if (listener != null)
                    {
                        listener.OnAnimationRepeat(this);
                    }
                }
            }

            if (!more && oneMoreTime)
            {
                oneMoreTime = false;
                return true;
            }

            return more;
        }

    }
}
