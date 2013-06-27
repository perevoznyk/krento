//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Diagnostics;

namespace Krento
{
    /// <summary>
    /// Kills the current instance of the application, when not in the debug mode
    /// </summary>
    internal static class Killer
    {
        /// <summary>
        /// Kills application's process
        /// </summary>
        public static void KillSelf()
        {

#if !DEBUG
            Process.GetCurrentProcess().Kill();
#else
            Trace.WriteLine("Krento tried to kill himself");
            Process.GetCurrentProcess().Kill();
#endif
        }

    }
}
