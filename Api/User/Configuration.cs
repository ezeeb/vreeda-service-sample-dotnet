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
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }
            
            // Retrieve user context
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);

            // Return configuration
            return Results.Json(userContext.Configuration ?? new UserConfiguration(), statusCode: 200);
        });
        
        routes.MapPost("/configuration", async (HttpContext context, IServiceState serviceState) =>
        {
            // Check if the session contains a user ID
            var userId = context.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }

            if (! await serviceState.HasUserContext(userId, CancellationToken.None))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }

            try
            {
                // Read JSON data from the request
                var body = await context.Request.ReadFromJsonAsync<UserConfiguration>();
                if (body?.devices == null)
                {
                    return Results.Json(new { error = "Invalid or missing configuration" }, statusCode: 400);
                }

                // Retrieve and update user context
                var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
                userContext.Configuration = body;

                var updated = await serviceState.UpsertConfiguration(userContext, CancellationToken.None);
                if (!updated)
                {
                    return Results.Json(new { error = "Failed to update configuration" }, statusCode: 500);
                }

                return Results.Json(body, statusCode: 200);
            }
            catch (Exception)
            {
                return Results.Json(new { error = "Failed to update configuration" }, statusCode: 500);
            }
        });
    }
}