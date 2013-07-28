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
using System.Drawing;

namespace Laugris.Sage
{
    public static class ColorUtils
    {
        public static int WhiteKey
        {
            get { return ColorTranslator.ToWin32(Color.FromArgb(0, 255, 255, 255)); }
        }

        public static int BlackKey
        {
            get { return ColorTranslator.ToWin32(Color.FromArgb(0)); }
        }

        public static Color DragColor
        {
            get { return Color.FromArgb(0xff, 0, 0x55); }
        }

        public static int Win32DragColor
        {
            get { return ColorTranslator.ToWin32(DragColor); }
        }

        public static Color Darker(Color color, byte percent)
        {
            int r;
            int g;
            int b;

            r = color.R;
            g = color.G;
            b = color.B;

            r = r - MathUtils.MulDiv(r, percent, 100);
            g = g - MathUtils.MulDiv(g, percent, 100);
            b = b - MathUtils.MulDiv(b, percent, 100);

            return Color.FromArgb(255, r, g, b);
        }
        public static Color Lighter(Color color, byte percent)
        {
            int r;
            int g;
            int b;

            r = color.R;
            g = color.G;
            b = color.B;

            r = r + MathUtils.MulDiv(255 - r, percent, 100); //Percent% closer to white
            g = g + MathUtils.MulDiv(255 - g, percent, 100);
            b = b + MathUtils.MulDiv(255 - b, percent, 100);

            return Color.FromArgb(255, r, g, b);
        }

        public static bool SameColors(Color color1, Color color2)
        {
            return (color1.ToArgb() == color2.ToArgb());
        }

        /// <summary>
        /// Compares two colors without taking alpha channel into account
        /// </summary>
        /// <param name="color1">The first color to compare</param>
        /// <param name="color2">The second color to compare</param>
        /// <returns>true, if the colors are the same, otherwise false</returns>
        public static bool SameColorsNoAlpha(Color color1, Color color2)
        {
            byte r1, g1, b1;
            byte r2, g2, b2;

            r1 = color1.R;
            r2 = color2.R;

            g1 = color1.G;
            g2 = color2.G;

            b1 = color1.B;
            b2 = color2.B;

            if (r1 != r2)
                return false;

            if (g1 != g2)
                return false;

            if (b1 != b2)
                return false;

            return true;
        }
    }
}
