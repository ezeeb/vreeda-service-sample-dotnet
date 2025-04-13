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
        routes.MapGet("/list-devices", async (IVreedaApiClient apiService, IServiceState serviceState, HttpContext context) =>
        {
            // get user id
            var userId = context.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }
            
            // check if user has state
            if (! await serviceState.HasUserGranted(userId, CancellationToken.None))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }

            // get token
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
            if (userContext.ApiAccessTokens == null || string.IsNullOrEmpty(userContext.ApiAccessTokens.AccessToken))
            {
                return Results.Json(new { error = "Access token not found" }, statusCode: 401);
            }

            try
            {
                // fetch list of devices
                var devices = await apiService.ListDevicesAsync(userContext.ApiAccessTokens.AccessToken);
                return Results.Json(devices, statusCode: 200);
            }
            catch (Exception)
            {
                return Results.Json(new { error = "Failed to fetch devices" }, statusCode: 500);
            }
        });
        
        routes.MapPatch("/patch-device", async (IVreedaApiClient apiService, IServiceState serviceState, HttpContext context) =>
        {
            // get user id
            var userId = context.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }

            // check if user has state
            if (! await serviceState.HasUserGranted(userId, CancellationToken.None))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }
            
            // get token
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
            if (userContext.ApiAccessTokens == null || string.IsNullOrEmpty(userContext.ApiAccessTokens.AccessToken))
            {
                return Results.Json(new { error = "Access token not found" }, statusCode: 401);
            }

            // get request body
            var requestBody = await context.Request.ReadFromJsonAsync<PatchDeviceRequestModel>();
            if (requestBody == null)
            {
                return Results.Json(new { error = "Invalid request body" }, statusCode: 400);
            }

            try
            {
                // patch device
                var response = await apiService.PatchDeviceAsync(userContext.ApiAccessTokens.AccessToken, requestBody.deviceId, new DevicesRequestModel {{requestBody.deviceId, requestBody.request}});
                return Results.Json(response, statusCode: 200);
            }
            catch (Exception)
            {
                return Results.Json(new { error = "Failed to update device" }, statusCode: 500);
            }
        });
    }
}