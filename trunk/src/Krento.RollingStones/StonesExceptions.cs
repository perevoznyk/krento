using System;
using System.Collections.Generic;
using System.Text;

namespace Krento.RollingStones
{
    [Serializable]
    public class StoneConstructorException : Exception
    {
       public StoneConstructorException(string message) : base(message) { }
       public StoneConstructorException(string message, Exception innerException) : base(message, innerException) { }

    }

    [Serializable]
    public class StoneSettingsException : Exception
    {
        public StoneSettingsException(string message) : base(message) { }
        public StoneSettingsException(string message, Exception innerException) : base(message, innerException) { }

    }

}
