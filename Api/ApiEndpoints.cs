using VreedaServiceSampleDotNet.Api.Auth;
using VreedaServiceSampleDotNet.Api.Jobs;
using VreedaServiceSampleDotNet.Api.User;
using VreedaServiceSampleDotNet.Api.Vreeda;

namespace VreedaServiceSampleDotNet.Api;

public static class ApiEndpoints
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var apiEndpoints = app.MapGroup("/api");
        
        apiEndpoints.MapAuthEndpoints();
        apiEndpoints.MapUserEndpoints();
        apiEndpoints.MapVreedaEndpoints();
        apiEndpoints.MapJobsEndpoints();
    }
}