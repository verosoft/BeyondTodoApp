#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace BeyondTodoDomain;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    [MaybeNull]
    public T Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, [MaybeNull] T value, string? error)
    {
        if (isSuccess && error != null)
        {
            throw new InvalidOperationException("A successful result cannot have an error.");
        }

        if (!isSuccess && error == null)
        {
            throw new InvalidOperationException("A failure result must have an error message.");
        }

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(string error) => new(false, default, error);
}
