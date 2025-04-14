namespace VreedaServiceSampleDotNet.Api.Auth;

using Services;

public static class Session
{
    public static void MapSessionEndpoint(this RouteGroupBuilder routes)
    {
        routes.MapGet("/session", (HttpContext context) =>
        {
            var session = context.Session;

            // Check if "user_id" exists in the session
            if (string.IsNullOrEmpty(session.GetString("user_id")))
            {
                return OperationResult.Ok(new { loggedIn = false }).ToResult();
            }

            // Return user details if session exists
            return OperationResult.Ok(new
            {
                loggedIn = true,
                user = new
                {
                    id = session.GetString("user_id"),
                    name = session.GetString("name"),
                    email = session.GetString("email")
                }
            }).ToResult();
        });
    }
}