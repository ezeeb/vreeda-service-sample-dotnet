namespace VreedaServiceSampleDotNet.Models;

using System.Text.Json.Serialization;

public class CodeTokenRepsonse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    /*[JsonPropertyName("id_token")]
    public string IdToken { get; set; }*/
    
    /*[JsonPropertyName("token_type")]
    public string TokenType { get; set; }*/
    
    /*[JsonPropertyName("not_before")]
    public long NotBefore { get; set; }*/
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    /*[JsonPropertyName("expires_on")]
    public long ExpiresOn { get; set; }*/
    
    /*[JsonPropertyName("resource")]
    public string Resource { get; set; }*/
    
    /*[JsonPropertyName("id_token_expires_in")]
    public int IdTokenExpiresIn { get; set; }*/
    
    /*[JsonPropertyName("profile_iInfo")]
    public string ProfileInfo { get; set; }*/
    
    /*[JsonPropertyName("scope")]
    public string Scope { get; set; }*/
    
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
    
    [JsonPropertyName("refresh_token_expires_in")]
    public int RefreshTokenExpiresIn { get; set; }
}