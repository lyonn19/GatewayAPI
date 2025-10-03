namespace GatewayApi.Common.Results;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; } = string.Empty;
    public int StatusCode { get; }

    protected Result(bool isSuccess, string error, int statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Success() => new(true, string.Empty, 200);

    public static Result Failure(string error, int statusCode = 400) =>
        new(false, error, statusCode);

    public static Result NotFound(string resource = "Resource") =>
        Failure($"{resource} not found", 404);

    public static Result Unauthorized(string message = "Unauthorized") =>
        Failure(message, 401);

    public static Result Forbidden(string message = "Forbidden") =>
        Failure(message, 403);
}

/// <summary>
/// Represents the result of an operation with a value.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public string Error { get; } = string.Empty;
    public int StatusCode { get; }
    public T? Value { get; }

    private Result(bool isSuccess, string error, int statusCode, T? value)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, string.Empty, 200, value);

    public static Result<T> Failure(string error, int statusCode = 400) =>
        new(false, error, statusCode, default);

    public static Result<T> NotFound(string resource = "Resource") =>
        Failure($"{resource} not found", 404);

    /// <summary>
    /// Implicitly converts a value to a successful Result.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);
}
