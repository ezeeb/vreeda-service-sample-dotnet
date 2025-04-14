using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace vreeda_service_sample_dotnet_tests.TestEnvironment
{
    /// <summary>
    /// Middleware for authentication in vreeda-service-sample-dotnet-tests
    /// Allows adding a default user for integration tests
    /// </summary>
    public class TestAuthenticationMiddleware(RequestDelegate next)
    {
        public const string TestUserIdHeader = "X-Test-UserId";

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the X-Test-UserId header is present
            if (context.Request.Headers.TryGetValue(TestUserIdHeader, out var userId) && !string.IsNullOrEmpty(userId))
            {
                // Create a test user identity with the UserId from the header
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, $"TestUser-{userId}"),
                    new Claim("userId", userId.ToString()),
                    new Claim("role", "TestUser")
                };

                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);

                // Add user to the context
                context.User = principal;
                context.Session.SetString("user_id", userId.ToString());
            }

            // Continue with the next middleware in the pipeline
            await next(context);
        }
    }

    // Extension method for the Application Builder
    public static class TestAuthenticationMiddlewareExtensions
    {
        public static void UseTestAuthentication(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<TestAuthenticationMiddleware>();
        }
    }
}