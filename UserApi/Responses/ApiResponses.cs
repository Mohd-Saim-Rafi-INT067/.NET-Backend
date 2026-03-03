namespace UserApi.Responses
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string Error { get; set; } = string.Empty;

        public ApiResponse(int statusCode, bool status, string message,string error = null, object? data = null)
        {
            StatusCode = statusCode;
            Status = status;
            Message = message;
            Error = error;
            Data = data;
        }
    }
}