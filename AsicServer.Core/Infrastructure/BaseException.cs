using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AsicServer.Infrastructure
{
    public class BaseException : Exception
    {
        public readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> Errors;

        public BaseException()
        {
        }

        public BaseException(string message) : base(message)
        {
        }

        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BaseException(IEnumerable<KeyValuePair<string, IEnumerable<string>>> errors)
        {
            this.Errors = errors;
        }

        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}
