using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Laugris.Sage
{
    [Serializable]
    public class ThemeException : Exception
    {
        public ThemeException()
        {
        }
        public ThemeException(string message)
            : base(message)
        {
        }
        public ThemeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ThemeException(SerializationInfo info,
         StreamingContext context)
            : base(info, context)
        {
        }

    }
}
