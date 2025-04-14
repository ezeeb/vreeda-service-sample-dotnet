namespace VreedaServiceSampleDotNet.Services;

using System.Net.Http.Headers;
using System.Text.Json;
using Models;

public class VreedaApiClient(IHttpClientFactory httpClientFactory, AppSettings appSettings)
    : IVreedaApiClient
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("VreedaApiClient");
    private readonly string _baseUrl = appSettings.VreedaApi.BaseUrl;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private async Task<CallResult<TResponse>> ApiFetchAsync<TRequest, TResponse>(
        string path, string token, HttpMethod method, TRequest? body)
        where TResponse : class
    {
        try
        {
            var url = $"{_baseUrl}{path}";

            using var request = new HttpRequestMessage(method, url);

            // Set standard headers
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Add body
            if (body != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
            {
                request.Content = JsonContent.Create(body, options: _jsonOptions);
            }

            // Send request
            var response = await _httpClient.SendAsync(request);

            // Check success status
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return CallResult<TResponse>.Failed(
                    $"API error: {(int)response.StatusCode} {response.ReasonPhrase}. Details: {errorContent}",
                    (int)response.StatusCode);
            }

            // Deserialize response using System.Text.Json
            var contentStream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<TResponse>(contentStream, _jsonOptions);
            return data == null
                ? CallResult<TResponse>.Failed("Failed to deserialize response", 500)
                : CallResult<TResponse>.Succeeded(data, (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            return CallResult<TResponse>.Failed($"HTTP request error: {ex.Message}",
                ex.StatusCode.HasValue ? (int)ex.StatusCode : 500);
        }
        catch (JsonException ex)
        {
            return CallResult<TResponse>.Failed($"JSON deserialization error: {ex.Message}", 500);
        }
        catch (Exception ex)
        {
            return CallResult<TResponse>.Failed($"Unexpected error: {ex.Message}", 500);
        }
    }

    public async Task<CallResult<DevicesResponseModel>> ListDevicesAsync(string token)
    {
        return await ApiFetchAsync<object, DevicesResponseModel>("/1.0/Device", token, HttpMethod.Get, null);
    }

    public async Task<CallResult<DevicesResponseModel>> PatchDeviceAsync(string token, string deviceId,
        DevicesRequestModel request)
    {
        return await ApiFetchAsync<object, DevicesResponseModel>($"/1.0/Device/{deviceId}", token, HttpMethod.Patch,
            request);
    }
}