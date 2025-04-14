using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Moq;
using vreeda_service_sample_dotnet_tests.TestEnvironment;
using VreedaServiceSampleDotNet.Models;
using Xunit;

namespace vreeda_service_sample_dotnet_tests;

public class ConfigurationEndpointTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory = factory.PrepareBasicMocks();

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public async System.Threading.Tasks.Task GetConfiguration_ShouldReturnUnauthorized_WhenNoUserIdInSession()
    {
        // Arrange
        var client = _factory.CreateClient();

        // We do not set the X-Test-UserId header, so no session ID is set

        // Act
        var response = await client.GetAsync("/api/user/configuration");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetConfiguration_ShouldReturnConfiguration_WhenUserIsAuthorized()
    {
        // Arrange
        var userId = "test-user";
        var userContext = new UserContext
        {
            UserId = userId,
            Configuration = new UserConfiguration
            {
                Devices = ["device1", "device2"]
            }
        };

        _factory.MockServiceState
            .Setup(s => s.HasUserContext(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _factory.MockServiceState
            .Setup(s => s.GetOrCreateUserContext(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userContext);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationMiddleware.TestUserIdHeader, userId);

        // Act
        var response = await client.GetAsync("/api/user/configuration");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(_options);
        Assert.True(content.GetProperty("success").GetBoolean());
        Assert.Equal(2, content.GetProperty("data").GetProperty("devices").GetArrayLength());
    }

    [Fact]
    public async System.Threading.Tasks.Task PostConfiguration_ShouldUpdateConfiguration_WhenValid()
    {
        // Arrange
        var userId = "test-user";
        var userContext = new UserContext
        {
            UserId = userId
        };

        _factory.MockServiceState
            .Setup(s => s.HasUserContext(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _factory.MockServiceState
            .Setup(s => s.GetOrCreateUserContext(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userContext);

        _factory.MockServiceState
            .Setup(s => s.UpsertConfiguration(It.IsAny<UserContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationMiddleware.TestUserIdHeader, userId);

        var config = new UserConfiguration
        {
            Devices = ["device1", "device2"]
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/user/configuration", config, _options);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(_options);
        Assert.True(content.GetProperty("success").GetBoolean());
        Assert.Equal(2, content.GetProperty("data").GetProperty("devices").GetArrayLength());

        // Verify that UpsertConfiguration was called
        _factory.MockServiceState.Verify(
            s => s.UpsertConfiguration(It.Is<UserContext>(uc => uc.Configuration != null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}