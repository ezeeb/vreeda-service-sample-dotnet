namespace VreedaServiceSampleDotNet.Api.Auth;

using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

public static class Callback
{
    public static void MapCallbackEndpoint(this RouteGroupBuilder routes)
    {
        // Callback route for Azure AD B2C
        routes.MapGet("/callback/azure-ad-b2c", async (HttpContext context, [FromServices] AppSettings appSettings, [FromServices] IServiceState serviceState, [FromServices] IHttpClientFactory httpClientFactory) =>
        {
            // Get authorization code from query
            var code = context.Request.Query["code"].ToString();
            if (string.IsNullOrEmpty(code))
            {
                return Results.BadRequest("Authorization code not found.");
            }

            // Configuration values
            var tenantName = appSettings.AzureAdB2C.TenantName;
            var clientId = appSettings.AzureAdB2C.ClientId;
            var clientSecret = appSettings.AzureAdB2C.ClientSecret;
            var redirectUri = $"{appSettings.HostUrl}/api/auth/callback/azure-ad-b2c";
            var policy = appSettings.AzureAdB2C.PrimaryUserFlow;
            var authority = $"https://{tenantName}.b2clogin.com/{tenantName}.onmicrosoft.com/{policy}";

            // Token request
            var tokenUrl = $"{authority}/oauth2/v2.0/token";
            var client = httpClientFactory.CreateClient("AdB2cClient");
            var tokenRequestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "code", code },
                { "redirect_uri", redirectUri }
            });

            HttpResponseMessage tokenResponse;
            try
            {
                tokenResponse = await client.PostAsync(tokenUrl, tokenRequestContent);
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }

            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                //logger.LogError("Token request failed: {Error}", errorContent);
                return Results.Problem($"Failed to obtain tokens: {errorContent}", null, (int)tokenResponse.StatusCode);
            }

            var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<CodeTokenRepsonse>(tokenResponseJson);
            if (tokenData == null)
            {
                return Results.Problem("Failed to deserialize token response.", null, 500);
            }

            // Decode access token to extract user details
            var accessToken = tokenData.AccessToken;
            var decodedAccessToken = JwtPayload.Base64UrlDeserialize(accessToken.Split('.')[1]); // Simplified parsing
            var userId = decodedAccessToken["sub"].ToString();
            
            if (userId == null)
            {
                return Results.Problem("Failed to get userId from token.", null, 500);
            }

            // Save tokens to session
            context.Session.SetString("access_token", accessToken);
            context.Session.SetString("user_id", userId);
            context.Session.SetString("name", decodedAccessToken["name"].ToString() ?? string.Empty);
            context.Session.SetString("email", decodedAccessToken["email"].ToString() ?? string.Empty);
            
            var existingUser = await serviceState.GetOrCreateUserContext(userId, default);
            existingUser.ApiAccessTokens = new ApiAccessTokens
            {
                AccessToken = accessToken,
                RefreshToken = tokenData.RefreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn),
                RefreshTokenExpiration = DateTime.UtcNow.AddSeconds(tokenData.RefreshTokenExpiresIn)
            };
            await serviceState.UpsertApiAccessTokens(existingUser, CancellationToken.None);

            return Results.Redirect("/");
        });
    }
}