using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AsicServer.Infrastructure
{
    public partial class BaseResponse<T>
    {
        public bool Success { get; set; }
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static BaseResponse<T> GetSuccessResponse(dynamic data)
        {
            return new BaseResponse<T>()
            {
                Success = true,
                Data = data
            };
        }

        public static BaseResponse<T> GetErrorResponse(Dictionary<string, IEnumerable<string>> errors)
        {
            return new BaseResponse<T>()
            {
                Success = false,
                Errors = errors
            };
        }

        public static BaseResponse<T> GetErrorResponse(Dictionary<string, IEnumerable<string>> errors, HttpStatusCode statusCode)
        {
            return new BaseResponse<T>()
            {
                Success = false,
                Errors = errors,
                StatusCode = statusCode
            };
        }

        public static BaseResponse<T> GetErrorResponse(Dictionary<string, IEnumerable<string>> errors, HttpStatusCode statusCode, dynamic data)
        {
            return new BaseResponse<T>()
            {
                Success = false,
                Errors = errors,
                StatusCode = statusCode,
                Data = data
            };
        }

    }
}
