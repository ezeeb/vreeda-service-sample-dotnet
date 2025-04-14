using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using VreedaServiceSampleDotNet.Models;
using VreedaServiceSampleDotNet.Services;
using Xunit;

namespace vreeda_service_sample_dotnet_tests;

/// <summary>
/// vreeda-service-sample-dotnet-tests for the ServiceStateMongoDB
/// </summary>
public class ServiceStateTests
{
    private readonly Mock<IMongoCollection<UserContext>> _mockUserCollection;
    private readonly Mock<IMongoClient> _mockClient;
    private readonly Mock<IMongoDatabase> _mockDatabase;
    private readonly Mock<ILogger<ServiceStateMongoDb>> _mockLogger;
    private UpdateResult _updateResult;

    public ServiceStateTests()
    {
        _mockUserCollection = new Mock<IMongoCollection<UserContext>>(MockBehavior.Loose);
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockClient = new Mock<IMongoClient>();
        _mockLogger = new Mock<ILogger<ServiceStateMongoDb>>();

        // Create simulated UpdateResult
        var mockUpdateResult = new Mock<UpdateResult>();
        mockUpdateResult.Setup(r => r.IsAcknowledged).Returns(true);
        mockUpdateResult.Setup(r => r.ModifiedCount).Returns(1);
        _updateResult = mockUpdateResult.Object;

        // Important: The collection name must match the one in ServiceStateMongoDB (users)
        _mockDatabase.Setup(d => d.GetCollection<UserContext>("users", null))
            .Returns(_mockUserCollection.Object);
        
        _mockClient.Setup(c => c.GetDatabase("testdb", null))
            .Returns(_mockDatabase.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task HasUserContext_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var userId = "test-user-id";
        var cursor = new Mock<IAsyncCursor<UserContext>>();
        cursor.Setup(c => c.Current).Returns(new List<UserContext>
        {
            new UserContext { UserId = userId }
        });
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockUserCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<UserContext>>(),
                It.IsAny<FindOptions<UserContext, UserContext>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        var serviceState = new ServiceStateMongoDb(_mockClient.Object, "testdb", _mockLogger.Object);

        // Act
        var result = await serviceState.HasUserContext(userId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task HasUserContext_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "non-existing-user";
        var cursor = new Mock<IAsyncCursor<UserContext>>();
        cursor.Setup(c => c.Current).Returns(new List<UserContext>());
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)  // There is a cursor move, but no results
            .ReturnsAsync(false);

        _mockUserCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<UserContext>>(),
                It.IsAny<FindOptions<UserContext, UserContext>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        var serviceState = new ServiceStateMongoDb(_mockClient.Object, "testdb", _mockLogger.Object);

        // Act
        var result = await serviceState.HasUserContext(userId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetOrCreateUserContext_ShouldReturnExistingContext_WhenUserExists()
    {
        // Arrange
        var userId = "existing-user-id";
        var existingContext = new UserContext 
        { 
            UserId = userId,
            Configuration = new UserConfiguration { Devices = ["device1"] }
        };
        
        var cursor = new Mock<IAsyncCursor<UserContext>>();
        cursor.Setup(c => c.Current).Returns(new List<UserContext> { existingContext });
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockUserCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<UserContext>>(),
                It.IsAny<FindOptions<UserContext, UserContext>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        var serviceState = new ServiceStateMongoDb(_mockClient.Object, "testdb", _mockLogger.Object);

        // Act
        var result = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(existingContext.Configuration.Devices, result.Configuration?.Devices);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetOrCreateUserContext_ShouldCreateNewContext_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "new-user-id";
        var cursor = new Mock<IAsyncCursor<UserContext>>();
        cursor.Setup(c => c.Current).Returns(new List<UserContext>());
        cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)  // The cursor moves, but there are no results
            .ReturnsAsync(false);

        _mockUserCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<UserContext>>(),
                It.IsAny<FindOptions<UserContext, UserContext>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        var serviceState = new ServiceStateMongoDb(_mockClient.Object, "testdb", _mockLogger.Object);

        // Act
        var result = await serviceState.GetOrCreateUserContext(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.NotNull(result.Configuration);
    }

    [Fact]
    public async System.Threading.Tasks.Task RevokeGrant_ShouldUnsetApiAccessTokens()
    {
        // Arrange
        var userId = "user-to-delete";
        
        // Setup for each UpdateOneAsync - important: use the correct UpdateResult
        _mockUserCollection
            .Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<UserContext>>(), 
                It.IsAny<UpdateDefinition<UserContext>>(), 
                It.IsAny<UpdateOptions>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_updateResult);

        var serviceState = new ServiceStateMongoDb(_mockClient.Object, "testdb", _mockLogger.Object);

        // Act
        var result = await serviceState.RevokeGrant(userId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async System.Threading.Tasks.Task UpsertConfiguration_ShouldUpdateConfiguration()
    {
        // Arrange
        var userId = "user-id";
        var userContext = new UserContext 
        { 
            UserId = userId,
            Configuration = new UserConfiguration { Devices = ["device1", "device2"] }
        };
        
        // Setup for each UpdateOneAsync - important: use the correct UpdateResult
        _mockUserCollection
            .Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<UserContext>>(), 
                It.IsAny<UpdateDefinition<UserContext>>(), 
                It.IsAny<UpdateOptions>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_updateResult);

        var serviceState = new ServiceStateMongoDb(_mockClient.Object, "testdb", _mockLogger.Object);

        // Act
        var result = await serviceState.UpsertConfiguration(userContext, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}