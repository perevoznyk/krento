using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Reflection;

namespace Laugris.Sage
{
    public sealed partial class SR
    {

        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;
            string result;
            result = Keys.GetString(key);
            if (string.IsNullOrEmpty(result))
            {
                Type t = typeof(SR);
                PropertyInfo info = t.GetProperty(key);
                if (info != null)
                    result = (string)info.GetValue(null, BindingFlags.Static, null, null, null);
            }
            return result;
        }

        public static string GetCaption(string key)
        {
            if (!string.IsNullOrEmpty(Laugris.Sage.Language.CultureFile))
            {
                string result = Laugris.Sage.Language.GetString(key);
                if (string.IsNullOrEmpty(result))
                    result = string.Empty;
                return result;
            }
            else
                return string.Empty;
        }

        public static string GetCaption(string section, string key)
        {
            if (!string.IsNullOrEmpty(Laugris.Sage.Language.CultureFile))
            {
                string result = Laugris.Sage.Language.GetString(section, key);
                if (string.IsNullOrEmpty(result))
                    result = string.Empty;
                return result;
            }
            else
                return string.Empty;
        }

    }

}
