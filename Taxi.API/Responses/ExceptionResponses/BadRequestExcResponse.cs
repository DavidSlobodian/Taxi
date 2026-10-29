using System.ComponentModel;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class BadRequestExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public BadRequestError ErrorData { get; set; } = new();

        public class BadRequestError
        {
            [DefaultValue(400)]
            public int ErrorCode { get; set; } = 400;

            [DefaultValue("Bad request")]
            public string Message { get; set; } = "Bad request";
        }
    }
}
