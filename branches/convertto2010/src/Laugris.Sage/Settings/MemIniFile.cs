using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Permissions;

namespace Laugris.Sage
{
    public class MemIniFile : IDisposable
    {
        KeyObjectValueCollection sections = new KeyObjectValueCollection();
        private string fileName;
        private Encoding encoding;
        private const char divider = '=';
        private const char openSection = '[';
        private const char closeSection = ']';

        public MemIniFile(string fileName) : this(fileName, false, false)
        {
        }

        public MemIniFile(string fileName, bool loadDirectly)
            : this(fileName, loadDirectly, false)
        {
        }

        public MemIniFile(string fileName, bool loadDirectly, bool readOnly)
        {
            this.fileName = fileName;
            try
            {
                if (FileOperations.FileExists(fileName))
                    encoding = NativeMethods.GetFileEncoding(fileName);
                else
                {
                    NativeMethods.CreateUnicodeFile(fileName);
                    encoding = Encoding.Unicode;
                }
            }
            catch
            {
                //if we can't create unicode file, lets use ASCII version
                encoding = Encoding.ASCII;
            }
            
            this.ReadOnly = readOnly;
            if (loadDirectly)
                Load();
        }

        ~MemIniFile()
        {
            Dispose(false);
        }

        public void AddSection(string sectionName)
        {
            if (SectionExists(sectionName))
                return;
            else
                sections.Add(sectionName);
        }

        public void Merge(string extraFile)
        {
            string s;
            string currentSection = null;
            string[] content;
            Encoding extraEncoding;

            if (string.IsNullOrEmpty(extraFile))
                return;

            if (!FileOperations.FileExists(extraFile))
                return;

            extraEncoding = NativeMethods.GetFileEncoding(extraFile);

            using (StreamReader reader = new StreamReader(extraFile, extraEncoding, false, 512))
            {
                while ((s = reader.ReadLine()) != null)
                {
                    //s = s.Trim();

                    if (string.IsNullOrEmpty(s))
                        continue;

                    if (s[0] == ';')
                        continue;

                    if ((s[0] == openSection) && (s[s.Length - 1] == closeSection))
                    {
                        currentSection = s.Substring(1, s.Length - 2);
                        sections.Add(currentSection);
                    }
                    else
                    {
                        if (currentSection == null)
                            continue;

                        int j = s.IndexOf(divider);
                        if (j > 0)
                        {
                            content = s.Split(new char[1] { divider }, 2);
                            if (content.Length == 2)
                                sections[currentSection].Add(content[0].Trim(), content[1].Trim());
                        }
                    }

                }
            }
        }

        public unsafe void Load()
        {
            string s;
            string currentSection = null;
            string[] content;


            sections.Clear();

            if (string.IsNullOrEmpty(fileName))
                return;

            if (!FileOperations.FileExists(fileName))
                return;

            TextFileReader reader = null;
            try
            {
                try
                {
                    reader = new TextFileReader(fileName, encoding);
                    while ((s = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(s))
                            continue;

                        if (s[0] == ';')
                            continue;

                        if ((s[0] == openSection) && (s[s.Length - 1] == closeSection))
                        {
                            currentSection = s.Substring(1, s.Length - 2);
                            sections.Add(currentSection);
                        }
                        else
                        {
                            if (currentSection == null)
                                continue;

                            int j = s.IndexOf(divider);
                            if (j > 0)
                            {
                                content = s.Split(new char[1] { divider }, 2);
                                if (content.Length == 2)
                                    sections[currentSection].Add(content[0].Trim(), content[1].Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace("MemIniFile.Load: " + ex.Message);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                
            }

        }


        public bool ReadOnly { get; set; }

        public void Save()
        {
            if (ReadOnly)
                return;

            if (string.IsNullOrEmpty(fileName))
                return;

            FileOperations.ClearFileAttributes(fileName);
            IntPtr file = IntPtr.Zero;
            try
            {
                try
                {
                    file = NativeMethods.FileCreateRewrite(fileName);
                    for (int i = 0; i < sections.Count; i++)
                    {
                        NativeMethods.FileWriteChar(file, openSection);
                        NativeMethods.FileWrite(file, sections.Key(i));
                        NativeMethods.FileWriteChar(file, closeSection);
                        NativeMethods.FileWriteNewLine(file);
                        for (int j = 0; j < sections[i].Count; j++)
                        {
                            if (!string.IsNullOrEmpty(sections[i].Value(j)))
                            {
                                NativeMethods.FileWrite(file, sections[i].Key(j));
                                NativeMethods.FileWriteChar(file, divider);
                                NativeMethods.FileWrite(file, sections[i].Value(j));
                                NativeMethods.FileWriteNewLine(file);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace("MemIniFile.Save: " + ex.Message);
                }
            }
            finally
            {
                if (file != IntPtr.Zero)
                    NativeMethods.FileClose(file);
            }
        }

        public void Clear()
        {
            sections.Clear();
        }

        /// <summary>
        /// Indicates whether a section exists in the ini file.
        /// </summary>
        /// <param name="section">The section name.</param>
        /// <returns><c>true</c> if the section exists, otherwise - <c>false</c></returns>
        public bool SectionExists(string section)
        {
            return sections.HasKey(section);
        }

        public string ReadString(string section, string ident, string defaultValue)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return defaultValue;
            string value = iniSection.Value(ident);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return value;
        }

        public string ReadString(string section, string ident)
        {
            return ReadString(section, ident, string.Empty);
        }

        public void WriteString(string section, string ident, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                KeyValueCollection iniSection = sections[section];
                if (iniSection == null)
                    iniSection = sections.Add(section);
                iniSection[ident] = value;
            }
        }

        public int ReadInteger(string section, string ident, int defaultValue)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return defaultValue;
            string value = iniSection.Value(ident);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            int result;
            if (int.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public void WriteInteger(string section, string ident, int value)
        {
            WriteString(section, ident, value.ToString(CultureInfo.InvariantCulture));
        }

        public bool ReadBool(string section, string ident, bool defaultValue)
        {
            int defaultInt;
            if (defaultValue)
                defaultInt = 1;
            else
                defaultInt = 0;
            return (ReadInteger(section, ident, defaultInt) == 1);
        }

        public void WriteBool(string section, string ident, bool value)
        {
            if (value)
                WriteString(section, ident, "1");
            else
                WriteString(section, ident, "0");
        }

        /// <summary>
        /// Contains the name of the ini file from which to read and to which to write information.
        /// </summary>
        /// <value>Read FileName to retrieve the name of the ini file that this object can be used 
        /// to access. The methods that read and write ini file information automatically handle opening and closing the file specified by FileName. FileName is read-only. The name of the ini file the object uses is supplied as the parameter for the constructor.</value>
        public string FileName
        {
            get { return this.fileName; }
        }

        public void UpdateFile()
        {
            Save();
        }

        public bool ValueExists(string section, string ident)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return false;
            return iniSection.HasKey(ident);
        }

        public void EraseSection(string section)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return;
            iniSection.Clear();
            sections.Remove(section);
        }

        public void DeleteKey(string section, string ident)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return;
            if (iniSection[ident] != null)
                iniSection.Remove(ident);
        }

        public string[] ReadSection(string section)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return null;
            return iniSection.AllKeys();
        }

        public KeyValueCollection Section(string section)
        {
            return sections[section];
        }

        public string[] ReadSections()
        {
            return sections.AllKeys();
        }

        public string[] GetSectionValues(string section)
        {
            KeyValueCollection iniSection = sections[section];
            if (iniSection == null)
                return null;
            return iniSection.AllValues();
        }

        public double ReadFloat(string section, string ident, double defaultValue)
        {
            NumberFormatInfo numberInfo = CultureInfo.InvariantCulture.NumberFormat;

            string floatString = ReadString(section, ident, String.Empty);
            double result = defaultValue;
            if (!String.IsNullOrEmpty(floatString))
                result = double.Parse(floatString, numberInfo);
            return result;
        }

        public void WriteFloat(string section, string ident, double value)
        {
            NumberFormatInfo numberInfo = CultureInfo.InvariantCulture.NumberFormat;
            string floatString = value.ToString(numberInfo);
            WriteString(section, ident, floatString);
        }

        public System.DateTime ReadDateTime(string section, string ident, DateTime defaultValue)
        {
            DateTimeFormatInfo dateInfo = CultureInfo.InvariantCulture.DateTimeFormat;
            string dateTimeString = ReadString(section, ident, String.Empty);
            DateTime result = defaultValue;
            if (!String.IsNullOrEmpty(dateTimeString))
                result = DateTime.Parse(dateTimeString, dateInfo);
            return result;
        }

        public void WriteDateTime(string section, string ident, DateTime value)
        {
            DateTimeFormatInfo dateInfo = CultureInfo.InvariantCulture.DateTimeFormat;
            string dateTimeString = value.ToString(dateInfo);
            WriteString(section, ident, dateTimeString);
        }

        public int ReadBinaryStream(string section, string ident, Stream value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            string stringResult = ReadString(section, ident, String.Empty);
            if (!String.IsNullOrEmpty(stringResult))
            {
                byte[] byteResult = Convert.FromBase64String(stringResult);
                value.Write(byteResult, (int)value.Position, byteResult.Length);
                return byteResult.Length;
            }
            else
                return 0;
        }

        public void WriteBinaryStream(string section, string ident, Stream value)
        {
            if (value == null)
                return;
            byte[] byteValue = new byte[value.Length - value.Position];
            value.Read(byteValue, (int)value.Position, (int)(value.Length - value.Position));
            string stringValue = Convert.ToBase64String(byteValue);
            WriteString(section, ident, stringValue);
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                sections.Dispose();
            else
                sections.Clear();

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
