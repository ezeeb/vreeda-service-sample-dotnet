namespace VreedaServiceSampleDotNet.Api.Vreeda;

public static class VreedaEndpoints
{
    public static void MapVreedaEndpoints(this RouteGroupBuilder group)
    {
        var userEndpoints = group.MapGroup("/vreeda");

        userEndpoints.MapDevicesEndpoints();
    }
}