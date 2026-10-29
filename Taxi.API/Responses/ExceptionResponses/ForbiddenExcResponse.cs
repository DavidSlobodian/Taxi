using System.ComponentModel;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class ForbiddenExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public ForbiddenError ErrorData { get; set; } = new();

        public class ForbiddenError
        {
            [DefaultValue(403)]
            public int ErrorCode { get; set; } = 403;

            [DefaultValue("Forbidden")]
            public string Message { get; set; } = "Forbidden";
        }
    }
}
