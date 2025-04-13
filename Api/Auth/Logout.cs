namespace VreedaServiceSampleDotNet.Api.Auth;

using Models;

public static class Logout
{
    public static void MapLogoutEndpoint(this RouteGroupBuilder routes)
    {
        routes.MapGet("/logout", (HttpContext context, AppSettings appSettings) =>
        {
            var tenantName = appSettings.AzureAdB2C.TenantName;
            var policy = appSettings.AzureAdB2C.PrimaryUserFlow;
            var hostUrl = appSettings.HostUrl;
            
            var authority = $"https://{tenantName}.b2clogin.com/{tenantName}.onmicrosoft.com/{policy}";
            var redirectUri = $"{hostUrl}/";
            var logoutUrl = $"{authority}/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString(redirectUri)}";
            
            var session = context.Session;

            session.Clear();
            
            return Results.Redirect(logoutUrl);
        });
    }
}