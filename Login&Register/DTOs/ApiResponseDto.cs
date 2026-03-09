namespace Login_Register.DTOs;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponseDto<T> SuccessResponse(string message, T? data = default)
    => new () { Success = true, Message = message, Data = data };

    public static ApiResponseDto<T> FailureResponse(string message)
    => new () { Success = false, Message = message };
}