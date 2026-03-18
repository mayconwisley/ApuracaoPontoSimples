namespace ApuracaoPontoSimples.Application.Models;

public enum ServiceErrorType
{
    Validation,
    NotFound,
    Conflict
}

public sealed record ServiceResult(bool Success, ServiceErrorType? ErrorType, string? ErrorMessage)
{
    public static ServiceResult Ok() => new(true, null, null);

    public static ServiceResult Fail(ServiceErrorType errorType, string errorMessage)
        => new(false, errorType, errorMessage);
}

public sealed record ServiceResult<T>(bool Success, T? Value, ServiceErrorType? ErrorType, string? ErrorMessage)
{
    public static ServiceResult<T> Ok(T value) => new(true, value, null, null);

    public static ServiceResult<T> Fail(ServiceErrorType errorType, string errorMessage)
        => new(false, default, errorType, errorMessage);
}
