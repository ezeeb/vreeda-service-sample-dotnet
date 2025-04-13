namespace VreedaServiceSampleDotNet.Models;

public class AppSettings
{
    public AzureAdB2COptions AzureAdB2C { get; set; } = new();

    public VreedaApiOptions VreedaApi { get; set; } = new();
    
    public Database MongoDBOptions { get; set; } = new();

    public string HostUrl { get; set; } = string.Empty;

    public string ApiRefreshTokensJobKey { get; set; } = string.Empty;
}

public class AzureAdB2COptions
{
    public string TenantName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string PrimaryUserFlow { get; set; } = string.Empty;
}

public class VreedaApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class Database
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
}