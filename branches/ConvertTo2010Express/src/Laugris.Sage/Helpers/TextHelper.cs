using System;

namespace Laugris.Sage
{
    public static class TextHelper
    {
        /// <summary>
        /// Compares two strings
        /// </summary>
        /// <param name="firstSting">The first sting.</param>
        /// <param name="secondString">The second string.</param>
        /// <returns>true if the text is the same</returns>
        public static bool SameText(string firstSting, string secondString)
        {
            return (string.CompareOrdinal(firstSting, secondString) == 0);
        }
    }
}
