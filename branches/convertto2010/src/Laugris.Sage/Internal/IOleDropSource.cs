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
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000121-0000-0000-C000-000000000046")]
    public interface IOleDropSource
    {
        [PreserveSig]
        int OleQueryContinueDrag(int fEscapePressed, [In, MarshalAs(UnmanagedType.U4)] int grfKeyState);
        [PreserveSig]
        int OleGiveFeedback([In, MarshalAs(UnmanagedType.U4)] int dwEffect);
    }
}
