namespace VreedaServiceSampleDotNet.Services;

/// <summary>
/// Result of an API call
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public record CallResult<T> where T : class
{
    public bool Success { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private CallResult(bool success, T? data, string? errorMessage, int statusCode)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static CallResult<T> Succeeded(T data, int statusCode = 200) => new(true, data, null, statusCode);

    public static CallResult<T> Failed(string errorMessage, int statusCode = 500) =>
        new(false, null, errorMessage, statusCode);
}