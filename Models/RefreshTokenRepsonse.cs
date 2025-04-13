namespace VreedaServiceSampleDotNet.Models;

public class RefreshTokenRepsonse
{
    public string access_token { get; set; } //+
    public int expires_in { get; set; } //+
    public string refresh_token { get; set; } //+
    public int refresh_token_expires_in { get; set; } //+
}