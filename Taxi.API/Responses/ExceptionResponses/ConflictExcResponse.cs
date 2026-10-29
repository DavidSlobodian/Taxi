using System.ComponentModel;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class ConflictExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public ConflictError ErrorData { get; set; } = new();

        public class ConflictError
        {
            [DefaultValue(409)]
            public int ErrorCode { get; set; } = 409;

            [DefaultValue("Conflict")]
            public string Message { get; set; } = "Conflict";
        }
    }
}
