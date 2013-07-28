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

namespace Laugris.Sage
{
    public static class MathUtils
    {
        public const double PiDiv180 = 1.74532925199433E-2;

        public static int MulDiv(int nNumber, int nNumerator, int nDenominator)
        {
            return (nNumber * nNumerator) / nDenominator;
        }

        public static byte MaxByte(int value)
        {
            return (value > 255) ? (byte)255 : (byte)value;
        }

    }
}
