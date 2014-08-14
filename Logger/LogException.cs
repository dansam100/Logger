using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Logger.Core
{
    [Serializable]
    public class LogException : ApplicationException
    {
        public LogException()
        {
        }

        public LogException(string message)
            : base(message)
        {
        }

        protected LogException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public LogException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
