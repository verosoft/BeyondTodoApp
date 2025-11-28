namespace BeyondTodoDomain;

public class Result<T>
{
    public T? Value { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }

    private Result(T? value, bool success, string? error)
    {
        Value = value;
        IsSuccess = success;
        Error = error;
    }

   
    public static Result<T> Success(T value) => new(value, true, null);

    public static Result<T> Failure(string error) => new(default, false, error);
}
