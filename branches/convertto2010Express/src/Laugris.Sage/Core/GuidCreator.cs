//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Laugris.Sage
{
    /// <summary>
    /// Generates new GUID
    /// </summary>
    public static class GuidCreator
    {
        /// <summary>
        /// Initializes a new instance of the "System.Guid" class
        /// </summary>
        /// <returns>A new System.Guid object</returns>
        public static Guid NewGuid()
        {
            Guid val = Guid.Empty;
            int hresult = 0;
            hresult = NativeMethods.CoCreateGuid(ref val);
            if (hresult != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), Language.GetString("ErrorCreateNewGuid", "Error creating new Guid"));
            }

            return val;
        }


    }
}
