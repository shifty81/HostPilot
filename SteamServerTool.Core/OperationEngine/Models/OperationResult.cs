namespace SteamServerTool.Core.OperationEngine.Models;

public sealed class OperationResult
{
    public static OperationResult Success(string? message = null, object? data = null) => new()
    {
        SuccessValue = true,
        Message = message,
        Data = data,
    };

    public static OperationResult Failure(string message, Exception? exception = null, bool retryable = false, object? data = null) => new()
    {
        SuccessValue = false,
        Message = message,
        Exception = exception,
        Retryable = retryable,
        Data = data,
    };

    public bool SuccessValue { get; init; }
    public string? Message { get; init; }
    public bool Retryable { get; init; }
    public Exception? Exception { get; init; }
    public object? Data { get; init; }
}
