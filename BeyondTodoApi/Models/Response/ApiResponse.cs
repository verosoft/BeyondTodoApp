namespace BeyondTodoApi.Models.Response;

public class ApiResponse<T>(bool success, string? message = null, T? data = default)
{
    public bool Success { get; set; } = success;
    public string? Message { get; set; } = message;
    public T? Data { get; set; } = data;

    public static ApiResponse<T> SuccessResponse(T data, string? message = "Operation successful.")
    {
        return new ApiResponse<T>(true, message, data);
    }

    public static ApiResponse<T> ErrorResponse(string message = "Operation failed.", T? data = default)
    {
        return new ApiResponse<T>(false, message, data);
    }
}
