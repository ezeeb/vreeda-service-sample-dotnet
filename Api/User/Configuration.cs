namespace VreedaServiceSampleDotNet.Api.User;

using Models;
using Services;

public static class Configuration
{
    public static void MapConfigurationEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapGet("/configuration", async (HttpContext context, IServiceState serviceState) =>
        {
            // Check if the session contains a user ID
            var userId = context.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                return OperationResult.Unauthorized().ToResult();
            }

            // Retrieve user context
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);

            // Return configuration
            return OperationResult.Ok(userContext.Configuration ?? new UserConfiguration()).ToResult();
        });

        routes.MapPost("/configuration", async (HttpContext context, IServiceState serviceState) =>
        {
            // Check if the session contains a user ID
            var userId = context.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                return OperationResult.Unauthorized().ToResult();
            }

            if (!await serviceState.HasUserContext(userId, CancellationToken.None))
            {
                return OperationResult.Unauthorized().ToResult();
            }

            // Read JSON data from the request
            var body = await context.Request.ReadFromJsonAsync<UserConfiguration>();
            if (body?.Devices == null)
            {
                return OperationResult.BadRequest("Invalid or missing configuration").ToResult();
            }

            // Validate devices array
            foreach (var device in body.Devices)
            {
                if (string.IsNullOrWhiteSpace(device))
                {
                    return OperationResult.BadRequest("Device IDs cannot be empty").ToResult();
                }
            }

            // Retrieve and update user context
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
            userContext.Configuration = body;

            var updated = await serviceState.UpsertConfiguration(userContext, CancellationToken.None);
            return !updated ? OperationResult.Error("Failed to update configuration").ToResult() : OperationResult.Ok(body).ToResult();
        });
    }
}