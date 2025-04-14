namespace VreedaServiceSampleDotNet.Services;

using System.Text.Json;

/// <summary>
/// Simple class for standardized API responses
/// </summary>
public record OperationResult
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private bool Success { get; }
    private int StatusCode { get; }
    private object? Data { get; }
    private string? ErrorMessage { get; }

    private OperationResult(bool success, int statusCode, object? data, string? errorMessage)
    {
        Success = success;
        StatusCode = statusCode;
        Data = data;
        ErrorMessage = errorMessage;
    }

    // Successful response
    public static OperationResult Ok(object? data = null) => new(true, 200, data, null);

    // Unauthorized
    public static OperationResult Unauthorized(string message = "Unauthorized") => new(false, 401, null, message);

    // Bad request
    public static OperationResult BadRequest(string message = "Bad Request") => new(false, 400, null, message);

    // Not found
    public static OperationResult NotFound(string message = "Resource not found") => new(false, 404, null, message);

    // Internal server error
    public static OperationResult Error(string message = "Internal Server Error") => new(false, 500, null, message);

    // Conversion to IResult for ASP.NET Core
    public IResult ToResult()
    {
        return Success
            ? Results.Json(new { success = Success, data = Data }, _options, statusCode: StatusCode)
            : Results.Json(new { success = Success, error = ErrorMessage }, _options, statusCode: StatusCode);
    }
}