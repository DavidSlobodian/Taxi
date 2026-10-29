using System.ComponentModel;

namespace Taxi.API.Responses.ExceptionResponses
{
    public class DataSavingExcResponse
    {
        [DefaultValue(false)]
        public bool Success { get; set; } = false;

        public object? Data { get; set; } = null;

        public DSError ErrorData { get; set; } = new();

        public class DSError
        {
            [DefaultValue(500)]
            public int ErrorCode { get; set; } = 500;

            [DefaultValue("Error saving data")]
            public string Message { get; set; } = "Error saving data";
        }
    }
}
