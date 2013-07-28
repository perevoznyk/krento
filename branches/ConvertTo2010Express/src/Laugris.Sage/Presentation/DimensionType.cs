using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// Description of the Visual element size 
    /// </summary>
    public enum DimensionType
    {
        /// <summary>
        /// The specified dimension is an absolute number of pixels.
        /// </summary>
        Absolute,
        /// <summary>
        /// The specified dimension holds a float and should be multiplied by the
        /// height or width of the object being animated.
        /// </summary>
        RelativeToSelf,
        /// <summary>
        /// The specified dimension holds a float and should be multiplied by the
        /// height or width of the parent of the object being animated.
        /// </summary>
        RelativeToParent
    }
}
