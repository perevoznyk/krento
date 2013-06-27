using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.IO;

namespace Krento.RollingStones
{
    public class HistoryEntry : IEquatable<HistoryEntry>, IDisposable
    {
        private MemIniFile iniFile;
        private string caption;
        private string description;
        private string logoFile;

        public HistoryEntry(string fileName)
        {
            this.iniFile = new MemIniFile(FileOperations.StripFileName(fileName));
            iniFile.Load();
            TranslationId = iniFile.ReadString("Settings", "TranslationId", string.Empty);

            if (!string.IsNullOrEmpty(TranslationId))
                caption = SR.Keys.GetString(TranslationId);
            else
                caption = string.Empty;

            if (string.IsNullOrEmpty(caption))
            {
                if (TextHelper.SameText(GlobalConfig.HomeCircleName, fileName))
                    caption = SR.DefaultRingName;
                else
                    caption = Path.GetFileNameWithoutExtension(fileName);
            }

            description = iniFile.ReadString("Settings", "Description", string.Empty);
            logoFile = iniFile.ReadString("Settings", "Logo", string.Empty);

        }

        ~HistoryEntry()
        {
            Dispose(false);
        }

        public MemIniFile IniFile
        {
            get { return iniFile; }
        }

        public string Caption
        {
            get { return caption; }
        }

        public string TranslationId { get; set; }

        public string FileName
        {
            get
            {
                if (this.iniFile == null)
                    return null;
                else
                    return this.iniFile.FileName;
            }
        }

        public void Save()
        {
            if (iniFile != null)
            {
                iniFile.WriteString("Settings", "Logo", logoFile);
                iniFile.WriteString("Settings", "Description", description);
                iniFile.Save();
            }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string LogoFile
        {
            get { return logoFile; }
            set { logoFile = value; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) return base.Equals(obj);
            if (obj is string)
                return TextHelper.SameText(FileName, (string)obj);
            else
                if (obj is HistoryEntry)
                    return Equals(obj as HistoryEntry);
                else
                    throw new InvalidCastException("The 'obj' argument is not a history entry object.");
        }


        #region IEquatable<HistoryEntry> Members

        public bool Equals(HistoryEntry other)
        {
            return (TextHelper.SameText(this.FileName, other.FileName));
        }

        #endregion

        #region IDisposable Members

        protected void Dispose(bool disposing)
        {
            if (iniFile != null)
                iniFile.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
