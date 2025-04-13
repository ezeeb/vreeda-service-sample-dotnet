namespace VreedaServiceSampleDotNet.Services;

using Models;

public interface IServiceState
{
    public Task<bool> HasUserContext(string userId, CancellationToken cancellationToken);

    public Task<bool> HasUserGranted(string userId, CancellationToken cancellationToken);

    public Task<UserContext> GetOrCreateUserContext(string userId, CancellationToken cancellationToken);
    
    public Task<bool> UpsertApiAccessTokens(UserContext context, CancellationToken cancellationToken);

    public Task<bool> RevokeGrant(string userId, CancellationToken cancellationToken);

    public Task<bool> UpsertConfiguration(UserContext context, CancellationToken cancellationToken);
    
    public Task<IEnumerable<UserContext>> FindExpiredAccessTokens(DateTime threshold,
        CancellationToken cancellationToken);
}

