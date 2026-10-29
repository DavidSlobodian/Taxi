using System.ComponentModel;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class UnauthorizedExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public UnauthError ErrorData { get; set; } = new();

        public class UnauthError
        {
            [DefaultValue(401)]
            public int ErrorCode { get; set; } = 401;

            [DefaultValue("Unauthorized access")]
            public string Message { get; set; } = "Unauthorized access";
        }
    }
}
