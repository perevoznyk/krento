using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Laugris.Sage
{
    [Serializable]
    public class EngineException : Exception
    {
        public EngineException()
        {
        }

        public EngineException(string message)
            : base(message)
        {
        }

        protected EngineException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public EngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
