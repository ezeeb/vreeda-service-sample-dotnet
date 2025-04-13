namespace VreedaServiceSampleDotNet.Api.Auth;

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
                return Results.Json(new { logged_in = false });
            }

            // Return user details if session exists
            return Results.Json(new
            {
                loggedIn = true,
                user = new
                {
                    id = session.GetString("user_id"),
                    name = session.GetString("name"),
                    email = session.GetString("email")
                }
            });
        });
    }
}