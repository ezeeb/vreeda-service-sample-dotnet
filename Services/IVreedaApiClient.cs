namespace VreedaServiceSampleDotNet.Services;

using Models;

/// <summary>
/// Client for accessing the Vreeda IoT platform API
/// </summary>
public interface IVreedaApiClient
{
    /// <summary>
    /// Lists all available devices for the user
    /// </summary>
    /// <param name="token">Access token for the Vreeda API</param>
    /// <returns>Response with list of devices</returns>
    Task<CallResult<DevicesResponseModel>> ListDevicesAsync(string token);

    /// <summary>
    /// Updates a device with new properties
    /// </summary>
    /// <param name="token">Access token for the Vreeda API</param>
    /// <param name="deviceId">Unique identifier of the device to update</param>
    /// <param name="request">Request with updated device properties</param>
    /// <returns>Response with updated device</returns>
    Task<CallResult<DevicesResponseModel>> PatchDeviceAsync(string token, string deviceId,
        DevicesRequestModel request);
}