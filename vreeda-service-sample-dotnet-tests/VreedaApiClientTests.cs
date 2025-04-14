using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using VreedaServiceSampleDotNet.Models;
using VreedaServiceSampleDotNet.Services;
using Xunit;

namespace vreeda_service_sample_dotnet_tests;

/// <summary>
/// vreeda-service-sample-dotnet-tests for the VreedaApiClient
/// </summary>
public class VreedaApiClientTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly AppSettings _appSettings;

    public VreedaApiClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(x => x.CreateClient("VreedaApiClient"))
            .Returns(httpClient);

        _appSettings = new AppSettings
        {
            VreedaApi = new VreedaApiOptions
            {
                BaseUrl = "https://api.example.com"
            }
        };
    }

    [Fact]
    public async System.Threading.Tasks.Task ListDevicesAsync_ShouldReturnSuccessResult_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedResponse = new DevicesResponseModel
        {
            // Mocked response data
        };
        
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResponse)
            });

        var client = new VreedaApiClient(_mockHttpClientFactory.Object, _appSettings);

        // Act
        var result = await client.ListDevicesAsync("test-token");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
        Assert.Null(result.ErrorMessage);
        
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get &&  req.RequestUri != null &&
                req.RequestUri.ToString().Contains("/1.0/Device")),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task PatchDeviceAsync_ShouldReturnSuccessResult_WhenApiCallSucceeds()
    {
        // Arrange
        var deviceId = "device-123";
        var request = new DevicesRequestModel
        {
            // Request data
        };
        
        var expectedResponse = new DevicesResponseModel
        {
            // Mocked response data
        };
        
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResponse)
            });

        var client = new VreedaApiClient(_mockHttpClientFactory.Object, _appSettings);

        // Act
        var result = await client.PatchDeviceAsync("test-token", deviceId, request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(200, result.StatusCode);
        Assert.Null(result.ErrorMessage);
        
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Patch && req.RequestUri != null &&
                req.RequestUri.ToString().Contains($"/1.0/Device/{deviceId}")),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task ApiCall_ShouldReturnFailureResult_WhenRequestFails()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ReasonPhrase = "Internal Server Error"
            });

        var client = new VreedaApiClient(_mockHttpClientFactory.Object, _appSettings);

        // Act
        var result = await client.ListDevicesAsync("test-token");

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("500", result.ErrorMessage);
    }
    
    [Fact]
    public async System.Threading.Tasks.Task ApiCall_ShouldReturnFailureResult_WhenNetworkError()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error", null, HttpStatusCode.BadGateway));

        var client = new VreedaApiClient(_mockHttpClientFactory.Object, _appSettings);

        // Act
        var result = await client.ListDevicesAsync("test-token");

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(502, result.StatusCode); // BadGateway
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Network error", result.ErrorMessage);
    }
}