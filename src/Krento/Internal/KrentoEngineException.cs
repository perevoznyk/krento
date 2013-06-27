using System;
using System.Runtime.Serialization;

namespace Krento
{
    [Serializable]
    public class KrentoEngineException : Exception
    {
        public KrentoEngineException() : base() { }
        public KrentoEngineException(string message) : base(message) { }
        public KrentoEngineException(string message, Exception innerException) : base(message, innerException) { }
        protected KrentoEngineException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }

    }

    [Serializable]
    public class SaveCurrentCircleException : Exception
    {
        public SaveCurrentCircleException() : base() { }
        public SaveCurrentCircleException(string message) : base(message) { }
        public SaveCurrentCircleException(string message, Exception innerException) : base(message, innerException) { }
        protected SaveCurrentCircleException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }

    }

    [Serializable]
    public class ChangeCircleException : Exception
    {
        public ChangeCircleException() : base() { }
        public ChangeCircleException(string message) : base(message) { }
        public ChangeCircleException(string message, Exception innerException) : base(message, innerException) { }
        protected ChangeCircleException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }

    }


    [Serializable]
    public class ManagerSettingsException : Exception
    {
        public ManagerSettingsException() : base() { }
        public ManagerSettingsException(string message) : base(message) { }
        public ManagerSettingsException(string message, Exception innerException) : base(message, innerException) { }
        protected ManagerSettingsException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }

    }

}
