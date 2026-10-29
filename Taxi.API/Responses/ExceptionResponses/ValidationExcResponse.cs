using System.ComponentModel;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class ValidationExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public ValidError ErrorData { get; set; } = new();

        public class ValidError
        {
            [DefaultValue(422)]
            public int ErrorCode { get; set; } = 422;

            [DefaultValue("Validation error")]
            public string Message { get; set; } = "Validation error";
        }
    }
}
