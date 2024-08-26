using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Utility
{
    public class Response<T>
    {

        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public Response(bool success, string message, T result, int validationStatus)
        {
            Success = success;
            Message = message;
            Result = result;
        }

        public Response()
        {
            Success = true;
            Message = string.Empty;
            Result = default(T);
        }
    }
}
