namespace VreedaServiceSampleDotNet.Api.User;

using Services;

public static class Revoke
{
    public static void MapRevokeEndpoint(this RouteGroupBuilder routes)
    {
        routes.MapDelete("/revoke", async (HttpContext context, IServiceState serviceState) =>
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
                // Delete user context
                var result = await serviceState.RevokeGrant(userId, CancellationToken.None);

                if (!result)
                {
                    return Results.Json(new { error = "User context not found" }, statusCode: 404);
                }

                // Delete session
                context.Session.Clear();

                return Results.Json(new { message = "User revoked and logged out" }, statusCode: 200);
            }
            catch (Exception)
            {
                return Results.Json(new { error = "Failed to revoke user" }, statusCode: 500);
            }
        });
    }
}