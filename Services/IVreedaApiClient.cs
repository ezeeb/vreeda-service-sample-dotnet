namespace VreedaServiceSampleDotNet.Services;

using Models;

public interface IVreedaApiClient
{
    Task<DevicesResponseModel> ListDevicesAsync(string token);
    Task<DevicesResponseModel> PatchDeviceAsync(string token, string deviceId, DevicesRequestModel request);
}