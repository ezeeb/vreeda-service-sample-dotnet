namespace VreedaServiceSampleDotNet.Api.User;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this RouteGroupBuilder group)
    {
        var userEndpoints = group.MapGroup("/user");
        
        userEndpoints.MapGrantedEndpoint();
        userEndpoints.MapRevokeEndpoint();
        userEndpoints.MapConfigurationEndpoints();
    }
}