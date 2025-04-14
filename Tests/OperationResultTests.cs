using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VreedaServiceSampleDotNet.Services;
using Xunit;

namespace vreeda_service_sample_dotnet_tests;

public class OperationResultTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    // Helper method to create a complete HttpContext
    private HttpContext CreateHttpContext()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IJsonHelper, SystemTextJsonHelper>();
        
        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };
        httpContext.Response.Body = new MemoryStream();
        
        return httpContext;
    }
    
    [Fact]
    public async System.Threading.Tasks.Task Ok_ShouldReturnSuccessResult()
    {
        // Arrange
        var data = new { id = 1, name = "Test" };
        var result = OperationResult.Ok(data);
        var httpContext = CreateHttpContext();
        
        // Act
        await result.ToResult().ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal(200, httpContext.Response.StatusCode);
        
        // Read response body
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseJson = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseJson, _options);
        
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.Equal(1, response.GetProperty("data").GetProperty("id").GetInt32());
        Assert.Equal("Test", response.GetProperty("data").GetProperty("name").GetString());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task Unauthorized_ShouldReturnUnauthorizedResult()
    {
        // Arrange
        var result = OperationResult.Unauthorized("Test error");
        var httpContext = CreateHttpContext();
        
        // Act
        await result.ToResult().ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal(401, httpContext.Response.StatusCode);
        
        // Read response body
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseJson = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseJson, _options);
        
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Test error", response.GetProperty("error").GetString());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task BadRequest_ShouldReturnBadRequestResult()
    {
        // Arrange
        var result = OperationResult.BadRequest("Invalid data");
        var httpContext = CreateHttpContext();
        
        // Act
        await result.ToResult().ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal(400, httpContext.Response.StatusCode);
        
        // Read response body
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseJson = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseJson, _options);
        
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Invalid data", response.GetProperty("error").GetString());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task NotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var result = OperationResult.NotFound("Resource not found");
        var httpContext = CreateHttpContext();
        
        // Act
        await result.ToResult().ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal(404, httpContext.Response.StatusCode);
        
        // Read response body
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseJson = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseJson, _options);
        
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Resource not found", response.GetProperty("error").GetString());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task Error_ShouldReturnErrorResult()
    {
        // Arrange
        var result = OperationResult.Error("Internal server error");
        var httpContext = CreateHttpContext();
        
        // Act
        await result.ToResult().ExecuteAsync(httpContext);
        
        // Assert
        Assert.Equal(500, httpContext.Response.StatusCode);
        
        // Read response body
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseJson = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseJson, _options);
        
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Internal server error", response.GetProperty("error").GetString());
    }
}

// Helper class for the JSON serializer
public interface IJsonHelper
{
    string Serialize(object value, JsonSerializerOptions options);
}

public class SystemTextJsonHelper : IJsonHelper
{
    public string Serialize(object value, JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(value, options);
    }
}