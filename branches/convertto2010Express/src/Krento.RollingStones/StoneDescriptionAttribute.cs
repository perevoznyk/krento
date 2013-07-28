using System;
using System.Collections.Generic;
using System.Text;

namespace Krento.RollingStones
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class StoneDescriptionAttribute : System.Attribute
    {
        private string description;

        public StoneDescriptionAttribute(string description)
        {
            this.description = description;
        }

        public string Description
        {
            get { return description; }
        }
    }
}
