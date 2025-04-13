namespace VreedaServiceSampleDotNet.Api.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this RouteGroupBuilder group)
    {
        var authEndpoints = group.MapGroup("/auth");
        
        authEndpoints.MapLoginEndpoint();
        authEndpoints.MapLogoutEndpoint();
        authEndpoints.MapCallbackEndpoint();
        authEndpoints.MapSessionEndpoint();
    }
}