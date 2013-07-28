using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Laugris.Sage
{
    [Serializable]
    public class InvocationException : Exception
    {
        public InvocationException()
        {
        }
        public InvocationException(string message)
            : base(message)
        {
        }
        public InvocationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvocationException(SerializationInfo info,
         StreamingContext context)
            : base(info, context)
        {
        }

    }
}
