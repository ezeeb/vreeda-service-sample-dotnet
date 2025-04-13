namespace VreedaServiceSampleDotNet.Api.Jobs;

using Models;
using Services;

public static class RefreshTokens
{
    public static void MapRefreshTokensEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/refresh-tokens", async (HttpContext context, IServiceState serviceState,
            IHttpClientFactory httpClientFactory, AppSettings appSettings, ILogger<Program> logger) =>
        {
            var query = context.Request.Query;
            var apiKey = query["key"];

            // check for api keys
            if (string.IsNullOrEmpty(apiKey) || apiKey != appSettings.ApiRefreshTokensJobKey)
            {
                return Results.Json(new { success = false, message = "Unauthorized" }, statusCode: 401);
            }

            try
            {
                var threshold = DateTime.UtcNow.AddMinutes(10);
                logger.LogInformation($"Token refresh job: checking for access token expiration before {threshold:O}");

                foreach (var user in await serviceState.FindExpiredAccessTokens(threshold, CancellationToken.None))
                {
                    try
                    {
                        var refreshToken = user.ApiAccessTokens?.RefreshToken;
                        if (string.IsNullOrEmpty(refreshToken))
                        {
                            logger.LogInformation($"Skipping user {user.UserId} due to missing refresh token.");
                            continue;
                        }

                        // Token-URL
                        var azureAdB2C = appSettings.AzureAdB2C;
                        var tokenUrl =
                            $"https://{azureAdB2C.TenantName}.b2clogin.com/{azureAdB2C.TenantName}.onmicrosoft.com/{azureAdB2C.PrimaryUserFlow}/oauth2/v2.0/token";

                        // Anfrage zum Abrufen neuer Tokens
                        var client = httpClientFactory.CreateClient("AdB2cClient");
                        var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "client_id", azureAdB2C.ClientId },
                            { "client_secret", azureAdB2C.ClientSecret },
                            { "grant_type", "refresh_token" },
                            { "refresh_token", refreshToken }
                        });

                        var response = await client.PostAsync(tokenUrl, requestContent);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            logger.LogInformation($"Failed to refresh token for user {user.UserId}: {error}");
                            continue;
                        }

                        var refreshTokenRepsonse = await response.Content.ReadFromJsonAsync<RefreshTokenRepsonse>();
                        if (refreshTokenRepsonse == null)
                        {
                            logger.LogInformation($"Failed to refresh token for user {user.UserId}: invalid response.");
                            continue;
                        }

                        // Neue Tokens berechnen
                        var newAccessTokenExpiration = DateTime.UtcNow.AddSeconds(refreshTokenRepsonse.expires_in);
                        var newRefreshTokenExpiration =
                            DateTime.UtcNow.AddSeconds(refreshTokenRepsonse.refresh_token_expires_in);

                        // Benutzerkontext aktualisieren
                        user.ApiAccessTokens.AccessToken = refreshTokenRepsonse.access_token;
                        user.ApiAccessTokens.RefreshToken = refreshTokenRepsonse.refresh_token;
                        var oldAccessTokenExpiration = user.ApiAccessTokens.AccessTokenExpiration;
                        var oldRefreshTokenExpiration = user.ApiAccessTokens.RefreshTokenExpiration;
                        user.ApiAccessTokens.AccessTokenExpiration = newAccessTokenExpiration;
                        user.ApiAccessTokens.RefreshTokenExpiration = newRefreshTokenExpiration;

                        await serviceState.UpsertApiAccessTokens(user, CancellationToken.None);

                        logger.LogInformation($"- refreshed tokens for user {user.UserId} @{DateTime.UtcNow}");
                        logger.LogInformation(
                            $"\textended access token from {oldAccessTokenExpiration} to {newAccessTokenExpiration}");
                        logger.LogInformation(
                            $"\textended refresh token from {oldRefreshTokenExpiration} to {newRefreshTokenExpiration}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error refreshing tokens for user {user.UserId}");
                    }
                }

                return Results.Json(new { success = true, message = "Token refresh completed successfully." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in token refresh job");
                return Results.Json(new { success = false, message = "Failed to refresh tokens." }, statusCode: 500);
            }
        });
    }
}