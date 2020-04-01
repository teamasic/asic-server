using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AsicServer.Infrastructure
{
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = @"text/plain";
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
        public BaseException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }

        public BaseException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }
        public BaseException(HttpStatusCode statusCode, string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.StatusCode = statusCode;
        }
    }
}
