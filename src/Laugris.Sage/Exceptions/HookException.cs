using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Laugris.Sage
{
    [Serializable]
    public class HookException : Exception
    {
        public HookException()
        {
        }

        public HookException(string message)
            : base(message)
        {
        }

        protected HookException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public HookException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
