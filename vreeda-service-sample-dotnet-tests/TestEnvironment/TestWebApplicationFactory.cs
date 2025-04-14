using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VreedaServiceSampleDotNet.Services;
using VreedaServiceSampleDotNet.Models;
using MongoDB.Driver;
using VreedaServiceSampleDotNet.Api;

namespace vreeda_service_sample_dotnet_tests.TestEnvironment
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IServiceState> MockServiceState { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Set up IServiceState mock
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IServiceState));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton(_ => MockServiceState.Object);

                // Replace IMongoClient with a mock or in-memory version
                var mongoDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IMongoClient));

                if (mongoDescriptor != null)
                {
                    services.Remove(mongoDescriptor);
                    // Optional: Register mock for MongoDB, if needed
                }

                // Enable session in vreeda-service-sample-dotnet-tests
                services.AddDistributedMemoryCache();
                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                // AppSettings for vreeda-service-sample-dotnet-tests
                services.AddSingleton(new AppSettings
                {
                    HostUrl = "http://localhost",
                    MongoDBOptions = new Database
                    {
                        ConnectionString = "mongodb://localhost:27017",
                        DbName = "vreeda_test"
                    }
                });

                // Ensure that controllers are available and loaded correctly
                services.AddControllers();
            });

            // Configure middleware
            builder.Configure(app =>
            {
                // Add base middleware
                app.UseRouting();

                // Enable test session
                app.UseSession();

                // Insert TestSessionMiddleware after the session middleware
                app.UseTestAuthentication();

                // Use the standard endpoint configuration as in Program.cs
                app.UseEndpoints(endpoints => { endpoints.MapApiEndpoints(); });
            });
        }

        // Ensure that mocks are properly prepared for vreeda-service-sample-dotnet-tests
        public TestWebApplicationFactory PrepareBasicMocks()
        {
            // Ensure that HasUserContext always returns true, unless explicitly overridden
            MockServiceState
                .Setup(s => s.HasUserContext(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Ensure that GetOrCreateUserContext returns a valid UserContext
            MockServiceState
                .Setup(s => s.GetOrCreateUserContext(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserContext { UserId = "test-user", Configuration = new UserConfiguration() });

            return this;
        }
    }
}