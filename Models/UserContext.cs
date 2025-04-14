namespace VreedaServiceSampleDotNet.Models;

using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a user's context including access rights and configuration
/// </summary>
[BsonIgnoreExtraElements]
public class UserContext
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Access tokens for the Vreeda API
    /// </summary>
    public ApiAccessTokens? ApiAccessTokens { get; set; }

    /// <summary>
    /// User-specific configuration settings
    /// </summary>
    public UserConfiguration? Configuration { get; set; }
    
    /// <summary>
    /// Creation timestamp of the user context
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Last update timestamp of the user context
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Contains the access tokens and their validity periods for API authentication
/// </summary>
[BsonIgnoreExtraElements]
public class ApiAccessTokens
{
    /// <summary>
    /// Token for accessing the Vreeda API
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Expiration timestamp of the access token
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime AccessTokenExpiration { get; set; }

    /// <summary>
    /// Token for refreshing the access token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Expiration timestamp of the refresh token
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime RefreshTokenExpiration { get; set; }
}

/// <summary>
/// User-specific configuration
/// </summary>
[BsonIgnoreExtraElements]
public class UserConfiguration
{
    /// <summary>
    /// List of device IDs associated with the user
    /// </summary>
    public string[]? Devices { get; set; } = [];
}