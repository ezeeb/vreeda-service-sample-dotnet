namespace VreedaServiceSampleDotNet.Api.Jobs;

public static class JobsEndpoints
{
    public static void MapJobsEndpoints(this RouteGroupBuilder group)
    {
        var jobEndpoints = group.MapGroup("/jobs");
        
        jobEndpoints.MapRefreshTokensEndpoint();
    }
}