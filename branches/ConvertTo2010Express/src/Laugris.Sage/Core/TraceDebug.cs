//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Laugris.Sage
{
    public static class TraceDebug
    {
        /// <summary>
        /// Logs the debug information to console.
        /// </summary>
        /// <param name="debugMessage">The output text</param>
        [Conditional("DEBUG")]
        public static void Message(string debugMessage)
        {
            Console.WriteLine(debugMessage);
        }

        /// <summary>
        /// Shows Message box.
        /// </summary>
        /// <param name="debugMessage">The debug message.</param>
        [Conditional("DEBUG")]
        public static void MessageBox(string debugMessage)
        {
            RtlAwareMessageBox.Show(debugMessage, "Debug information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        /// <summary>
        /// Add text line to debugger output window
        /// </summary>
        /// <param name="debugMessage">The output text</param>
        [Conditional("DEBUG")]
        public static void Trace(string debugMessage)
        {
            InteropHelper.OutputDebugString(debugMessage);
        }

        [Conditional("DEBUG")]
        public static void Trace(string debugMessage, Exception ex)
        {
            if (!string.IsNullOrEmpty(debugMessage))
                InteropHelper.OutputDebugString(debugMessage);
            string message = string.Empty;
            while (ex.InnerException != null)
            {
                message = message + ex.Message + Environment.NewLine;
                ex = ex.InnerException;
            }
            message = message + ex.Message + Environment.NewLine;

            StackTrace st = new StackTrace(ex);
            message = message + "Call stack:";
            InteropHelper.OutputDebugString(message + Environment.NewLine + st.ToString());
        }

        [Conditional("DEBUG")]
        public static void Trace(Exception ex)
        {
            Trace(string.Empty, ex);
        }

        [Conditional("DEBUG")]
        public static void Trace(string debugMessage, params object[] args)
        {
            InteropHelper.OutputDebugString(string.Format(CultureInfo.InvariantCulture, debugMessage, args));
        }

    }
}
