namespace VreedaServiceSampleDotNet.Api.Auth;

using Models;

public static class Login
{
    public static void MapLoginEndpoint(this RouteGroupBuilder routes)
    {
        routes.MapGet("/login", (AppSettings appSettings) =>
        {
            var azureAdB2COptions = appSettings.AzureAdB2C;

            // Read values from configuration
            var tenantName = azureAdB2COptions.TenantName;
            var clientId = azureAdB2COptions.ClientId;
            var hostUrl = appSettings.HostUrl;
            var policy = azureAdB2COptions.PrimaryUserFlow;

            var redirectUri = $"{hostUrl}/api/auth/callback/azure-ad-b2c";
            var authority = $"https://{tenantName}.b2clogin.com/{tenantName}.onmicrosoft.com/{policy}";

            // Create the authorization URL
            var authorizationUrl = $"{authority}/oauth2/v2.0/authorize?" +
                                   $"client_id={clientId}&" +
                                   $"response_type=code&" +
                                   $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                                   $"response_mode=query&" +
                                   $"scope={Uri.EscapeDataString(clientId + " offline_access openid")}&" +
                                   $"state=12345";

            // Redirect to the authorization URL
            return Results.Redirect(authorizationUrl);
        });
    }
}