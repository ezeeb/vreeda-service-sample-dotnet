using VreedaServiceSampleDotNet.Models;
using VreedaServiceSampleDotNet.Services;
using Xunit;

namespace vreeda_service_sample_dotnet_tests;

public class CallResultTests
{
    [Fact]
    public void Succeeded_ShouldCreateSuccessResult()
    {
        // Arrange
        var data = new DevicesResponseModel();
        
        // Act
        var result = CallResult<DevicesResponseModel>.Succeeded(data);
        
        // Assert
        Assert.True(result.Success);
        Assert.Same(data, result.Data);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(200, result.StatusCode);
    }
    
    [Fact]
    public void Succeeded_ShouldCreateSuccessResultWithCustomStatusCode()
    {
        // Arrange
        var data = new DevicesResponseModel();
        
        // Act
        var result = CallResult<DevicesResponseModel>.Succeeded(data, 201);
        
        // Assert
        Assert.True(result.Success);
        Assert.Same(data, result.Data);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(201, result.StatusCode);
    }
    
    [Fact]
    public void Failed_ShouldCreateFailureResult()
    {
        // Arrange
        var errorMessage = "Test error";
        
        // Act
        var result = CallResult<DevicesResponseModel>.Failed(errorMessage);
        
        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Equal(500, result.StatusCode);
    }
    
    [Fact]
    public void Failed_ShouldCreateFailureResultWithCustomStatusCode()
    {
        // Arrange
        var errorMessage = "Test error";
        
        // Act
        var result = CallResult<DevicesResponseModel>.Failed(errorMessage, 400);
        
        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Equal(400, result.StatusCode);
    }
}