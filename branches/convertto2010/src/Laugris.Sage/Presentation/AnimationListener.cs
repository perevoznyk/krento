using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// An animation listener receives notifications from an animation.
    /// Notifications indicate animation related events, such as the end or the
    /// repetition of the animation.
    /// </summary>
    public interface IAnimationListener
    {
        /// <summary>
        /// Notifies the start of the animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        void OnAnimationStart(Animation animation);
        /// <summary>
        /// Notifies the end of the animation. This callback is not invoked
        /// for animations with repeat count set to -1.
        /// </summary>
        /// <param name="animation">The animation.</param>
        void OnAnimationEnd(Animation animation);
        /// <summary>
        /// Notifies the repetition of the animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        void OnAnimationRepeat(Animation animation);
    }
}
