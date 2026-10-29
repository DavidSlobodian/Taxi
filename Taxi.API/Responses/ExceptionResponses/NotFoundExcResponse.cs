using System.ComponentModel;
using Taxi.Domain.Exceptions;
using Taxi.Domain.Responses;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class NotFoundExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public NotFoundError ErrorData { get; set; } = new();

        public class NotFoundError
        {
            [DefaultValue(404)]
            public int ErrorCode { get; set; } = 404;

            [DefaultValue("The object is not found")]
            public string Message { get; set; } = "The object is not found";
        }
    }
}
