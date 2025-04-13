namespace VreedaServiceSampleDotNet.Api.User;

using Services;

public static class Granted
{
    public static void MapGrantedEndpoint(this RouteGroupBuilder routes)
    {
        routes.MapGet("/granted", async (HttpContext context, IServiceState serviceState) =>
        {
            // get user id
            var userId = context.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }

            // check if user has state
            if (! await serviceState.HasUserContext(userId, CancellationToken.None))
            {
                return Results.Json(new { error = "Unauthorized" }, statusCode: 401);
            }
            
            // get tokens
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
            if (userContext.ApiAccessTokens == null)
            {
                return Results.Json(new { granted = false, message = "Tokens are missing" }, statusCode: 401);
            }

            // check if tokens are expired
            var now = DateTime.UtcNow;
            var accessTokenExpired = userContext.ApiAccessTokens.AccessTokenExpiration <= now;
            var refreshTokenExpired = userContext.ApiAccessTokens.RefreshTokenExpiration <= now;

            if (accessTokenExpired || refreshTokenExpired)
            {
                return Results.Json(new { granted = false, message = "Tokens are expired" }, statusCode: 401);
            }

            // access is granted
            return Results.Json(new { granted = true, message = "Access granted" });
        }); 
    }
}