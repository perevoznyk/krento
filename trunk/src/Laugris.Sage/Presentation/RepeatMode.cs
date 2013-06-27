using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    /// <summary>
    /// Animation repeat mode
    /// </summary>
    public enum RepeatMode
    {
        /// <summary>
        /// The animation restarts from the beginning.
        /// </summary>
        Restart,
        /// <summary>
        /// The animation plays backward (and then forward again).
        /// </summary>
        Reverse
    }
}
