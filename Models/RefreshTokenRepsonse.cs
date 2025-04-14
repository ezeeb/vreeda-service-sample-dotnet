namespace VreedaServiceSampleDotNet.Models;

using System.Text.Json.Serialization;

public class RefreshTokenRepsonse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
    
    [JsonPropertyName("refresh_token_expires_in")]
    public int RefreshTokenExpiresIn { get; set; }
}