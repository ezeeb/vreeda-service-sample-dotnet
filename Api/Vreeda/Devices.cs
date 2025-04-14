namespace VreedaServiceSampleDotNet.Api.Vreeda;

using Models;
using Services;

public class PatchDeviceRequestModel
{
    public string deviceId { get; set; } = String.Empty;
    public DeviceRequestModel request { get; set; } = new();
}

public static class Devices
{
    public static void MapDevicesEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapGet("/list-devices",
            async (IVreedaApiClient apiService, IServiceState serviceState, HttpContext context) =>
            {
                // get user id
                var userId = context.Session.GetString("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    return OperationResult.Unauthorized().ToResult();
                }

                // check if user has state
                if (!await serviceState.HasUserGranted(userId, CancellationToken.None))
                {
                    return OperationResult.Unauthorized().ToResult();
                }

                // get token
                var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
                if (userContext.ApiAccessTokens == null ||
                    string.IsNullOrEmpty(userContext.ApiAccessTokens.AccessToken))
                {
                    return OperationResult.Unauthorized("Access token not found").ToResult();
                }

                // fetch list of devices
                var devicesResult = await apiService.ListDevicesAsync(userContext.ApiAccessTokens.AccessToken);

                return !devicesResult.Success ? OperationResult.Error($"Failed to fetch devices: {devicesResult.ErrorMessage}").ToResult() : OperationResult.Ok(devicesResult.Data).ToResult();
            });

        routes.MapPatch("/patch-device",
            async (IVreedaApiClient apiService, IServiceState serviceState, HttpContext context) =>
            {
                // get user id
                var userId = context.Session.GetString("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    return OperationResult.Unauthorized().ToResult();
                }

                // check if user has state
                if (!await serviceState.HasUserGranted(userId, CancellationToken.None))
                {
                    return OperationResult.Unauthorized().ToResult();
                }

                // get token
                var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
                if (userContext.ApiAccessTokens == null ||
                    string.IsNullOrEmpty(userContext.ApiAccessTokens.AccessToken))
                {
                    return OperationResult.Unauthorized("Access token not found").ToResult();
                }

                // get request body
                var requestBody = await context.Request.ReadFromJsonAsync<PatchDeviceRequestModel>();
                if (requestBody == null)
                {
                    return OperationResult.BadRequest("Invalid request body").ToResult();
                }

                if (string.IsNullOrWhiteSpace(requestBody.deviceId))
                {
                    return OperationResult.BadRequest("Device ID is required").ToResult();
                }

                // patch device
                var response = await apiService.PatchDeviceAsync(
                    userContext.ApiAccessTokens.AccessToken,
                    requestBody.deviceId,
                    new DevicesRequestModel { { requestBody.deviceId, requestBody.request } }
                );

                return !response.Success ? OperationResult.Error($"Failed to update device: {response.ErrorMessage}").ToResult() : OperationResult.Ok(response.Data).ToResult();
            });
    }
}