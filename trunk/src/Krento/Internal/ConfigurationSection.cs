using System.ComponentModel;
using Laugris.Sage;

namespace Krento
{
    internal class ConfigurationSection : IConfiguration
    {
        private KeyValueCollection iniSection;

        public ConfigurationSection(KeyValueCollection iniSection)
        {
            this.iniSection = iniSection;
        }

        #region IConfiguration Members

        public void Clear()
        {
            iniSection.Clear();
        }

        public void ClearProperty(string name)
        {
            if (iniSection[name] != null)
                iniSection.Remove(name);
        }

        public string GetProperty(string name)
        {
            return iniSection.Value(name);
        }

        public string GetProperty(string name, string defaultValue)
        {
            string value = iniSection.Value(name);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return value;
        }

        public bool HasProperty(string name)
        {
            return iniSection.HasKey(name);
        }

        public void SetProperty(string name, string value)
        {
            iniSection[name] = value;
            this.OnPropertyChanged(name);
        }

        public void SetProperty(string name, string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                iniSection[name] = defaultValue;
            else
                iniSection[name] = value;

            this.OnPropertyChanged(name);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
