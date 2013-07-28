using System;
using System.Windows.Forms;

namespace Laugris.Sage
{
    public delegate void KeyboardAction(Keys keys);

    public class ActionLoader : IDisposable
    {
        private MemIniFile iniFile;
        private KeysConverter converter;

        public ActionLoader(string fileName)
        {
            iniFile = new MemIniFile(fileName);
            iniFile.Load();
            converter = new KeysConverter();
        }

        ~ActionLoader()
        {
            Dispose(false);
        }

        public string KeysToString(Keys keys)
        {
            try
            {
                return converter.ConvertToString(keys);
            }
            catch
            {
                return converter.ConvertToString(Keys.None);
            }
        }

        public Keys StringToKeys(string value)
        {
            try
            {
                return (Keys)converter.ConvertFromString(value);
            }
            catch
            {
                return Keys.None;
            }
        }

        public Keys[] GetActionKeys(string actionName)
        {
            try
            {
                string[] keyValues = iniFile.GetSectionValues(actionName);
                if (keyValues == null)
                    return new Keys[0];
                else
                {
                    Keys[] result = new Keys[keyValues.Length];
                    for (int i = 0; i < keyValues.Length; i++)
                    {
                        result[i] = StringToKeys(keyValues[i]);
                    }
                    keyValues = null;
                    return result;
                }
            }
            catch
            {
                return new Keys[0];
            }
        }


        public void AddActionKeys(string actionName, Keys[] keys)
        {
            int keyCount;
            if (string.IsNullOrEmpty(actionName))
                return;
            if (keys == null)
                return;
            if (iniFile.SectionExists(actionName))
                keyCount = iniFile.Section(actionName).Count;
            else
                keyCount = 0;

            for (int i = 0; i < keys.Length; i++)
            {
                iniFile.WriteString(actionName, "Key" + (keyCount + i + 1).ToString(), KeysToString(keys[i]));
            }
        }

        public void AddActionKey(string actionName, Keys key)
        {
            int keyCount;
            if (string.IsNullOrEmpty(actionName))
                return;
            if (iniFile.SectionExists(actionName))
                keyCount = iniFile.Section(actionName).Count;
            else
                keyCount = 0;
            iniFile.WriteString(actionName, "Key" + (keyCount + 1).ToString(), KeysToString(key));
        }

        public void Save()
        {
            iniFile.Save();
        }

        public void ClearAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
                return;
            if (iniFile.SectionExists(actionName))
                iniFile.Section(actionName).Clear();
        }

        #region IDisposable Members

        protected void Dispose(bool disposing)
        {
            if (iniFile != null)
            {
                iniFile.Dispose();
                iniFile = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
