namespace VreedaServiceSampleDotNet.Models;

using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class UserContext
{
    public string UserId { get; init; }

    public ApiAccessTokens? ApiAccessTokens { get; set; }

    public UserConfiguration? Configuration { get; set; }
        
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

[BsonIgnoreExtraElements]
public class ApiAccessTokens
{
    public string? AccessToken { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime AccessTokenExpiration { get; set; }

    public string? RefreshToken { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime RefreshTokenExpiration { get; set; }
}

[BsonIgnoreExtraElements]
public class UserConfiguration
{
    public string[]? devices { get; set; } = [];
}