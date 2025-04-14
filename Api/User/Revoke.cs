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
                return OperationResult.Unauthorized().ToResult();
            }

            if (!await serviceState.HasUserContext(userId, CancellationToken.None))
            {
                return OperationResult.Unauthorized().ToResult();
            }

            // Delete user context
            var result = await serviceState.RevokeGrant(userId, CancellationToken.None);

            if (!result)
            {
                return OperationResult.NotFound("User context not found").ToResult();
            }

            // Delete session
            context.Session.Clear();

            return OperationResult.Ok(new { message = "User revoked and logged out" }).ToResult();
        });
    }
}