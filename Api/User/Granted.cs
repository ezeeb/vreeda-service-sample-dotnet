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
                return OperationResult.Unauthorized().ToResult();
            }

            // check if user has state
            if (! await serviceState.HasUserContext(userId, CancellationToken.None))
            {
                return OperationResult.Unauthorized().ToResult();
            }
            
            // get tokens
            var userContext = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);
            if (userContext.ApiAccessTokens == null)
            {
                return OperationResult.Unauthorized("Tokens are missing").ToResult();
            }

            // check if tokens are expired
            var now = DateTime.UtcNow;
            var accessTokenExpired = userContext.ApiAccessTokens.AccessTokenExpiration <= now;
            var refreshTokenExpired = userContext.ApiAccessTokens.RefreshTokenExpiration <= now;

            if (accessTokenExpired || refreshTokenExpired)
            {
                return OperationResult.Unauthorized("Tokens are expired").ToResult();
            }

            // access is granted
            return OperationResult.Ok(new { granted = true, message = "Access granted" }).ToResult();
        }); 
    }
}