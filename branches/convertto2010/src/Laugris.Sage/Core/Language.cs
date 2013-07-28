using System;
using System.Globalization;

namespace Laugris.Sage
{
    /// <summary>
    /// Class that manage the internationalization of Laugris
    /// </summary>
    public sealed class Language
    {
        private static CultureInfo culture = CultureInfo.CurrentUICulture;
        private static string cultureFile;
        private static MemIniFile iniFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        private Language()
        {
        }

        /// <summary>
        /// Gets or sets the current UI culture of Laugris.
        /// </summary>
        /// <value>The current UI culture of Laugris.</value>
        public static CultureInfo Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        /// <summary>
        /// Gets or sets the culture file.
        /// </summary>
        /// <value>The culture file.</value>
        public static string CultureFile
        {
            get { return cultureFile; }
            set
            {
                cultureFile = value;

                if (iniFile != null)
                {
                    iniFile.Dispose();
                    iniFile = null;
                }
                if (!string.IsNullOrEmpty(cultureFile))
                {
                    iniFile = new MemIniFile(cultureFile);
                    iniFile.Load();
                    if (culture.Name == "en-US")
                        iniFile.DeleteKey("strings", "Translator");
                }
            }
        }

        /// <summary>
        /// Closes this instance and disposes the translation file.
        /// </summary>
        public static void Close()
        {
            if (iniFile != null)
            {
                iniFile.Dispose();
                iniFile = null;
            }
        }

        public static void Merge(string extraFile)
        {
            if (iniFile != null)
                iniFile.Merge(extraFile);
        }

        public static string GetString(string key)
        {
            if (iniFile != null)
            {
                string text = iniFile.ReadString("strings", key, string.Empty);
                text = text.Replace(@"\n", Environment.NewLine);
                return text;
            }
            else
                return string.Empty;
        }

        public static string GetString(string key, string defaultValue)
        {
            string text;

            if (iniFile != null)
            {
                text = iniFile.ReadString("strings", key, defaultValue);
                if (string.IsNullOrEmpty(text))
                    text = defaultValue;
            }
            else
                text = defaultValue;

            text = text.Replace(@"\n", Environment.NewLine);
            return text;
        }
        

        public static string GetString(string section, string key, string defaultValue)
        {
            string text;

            if (iniFile != null)
            {
                text = iniFile.ReadString(section, key, defaultValue);
                if (string.IsNullOrEmpty(text))
                    text = defaultValue;
            }
            else
                text = defaultValue;

            text = text.Replace(@"\n", Environment.NewLine);
            return text;
        }

    }
}

