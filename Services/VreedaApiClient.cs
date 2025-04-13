namespace VreedaServiceSampleDotNet.Services;

using System.Net.Http.Headers;
using Models;

public class VreedaApiClient(IHttpClientFactory httpClientFactory, AppSettings appSettings)
    : IVreedaApiClient
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("VreedaApiClient");
    private readonly string _baseUrl = appSettings.VreedaApi.BaseUrl;

    private async Task<TResponse> ApiFetchAsync<TRequest, TResponse>(string path, string token, HttpMethod method, TRequest? body)
    {
        var url = $"{_baseUrl}{path}";

        using var request = new HttpRequestMessage(method, url);

        // Set standard headers
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add body
        if (body != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
        {
            request.Content = JsonContent.Create(body);
        }

        // Send request
        var response = await _httpClient.SendAsync(request);

        // Ensure success status
        response.EnsureSuccessStatusCode();

        // Deserialize response
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
    
    public async Task<DevicesResponseModel> ListDevicesAsync(string token)
    {
        return await ApiFetchAsync<object, DevicesResponseModel>("/1.0/Device", token, HttpMethod.Get, null);
    }

    public async Task<DevicesResponseModel> PatchDeviceAsync(string token, string deviceId, DevicesRequestModel request)
    {
        return await ApiFetchAsync<object, DevicesResponseModel>($"/1.0/Device/{deviceId}", token, HttpMethod.Patch, request);
    }
}