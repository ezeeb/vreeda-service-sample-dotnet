namespace VreedaServiceSampleDotNet.Services;

using MongoDB.Driver;
using Models;

public class ServiceStateMongoDb : BaseMongoDbCollection<UserContext, string>, IServiceState
{
    private const string CollectionName = "users";

    public ServiceStateMongoDb(IMongoClient client, string databaseName, ILogger<ServiceStateMongoDb> logger)
        : base(client, databaseName, CollectionName)
    {
        logger.LogInformation(
            $"Initialized service state repository using collection '{CollectionName}' in database '{databaseName}'");
    }

    protected override FilterDefinition<UserContext> IdFilter(string uniqueId)
    {
        return Builders<UserContext>.Filter.Eq(c => c.UserId, uniqueId);
    }

    public async Task<UserContext> GetOrCreateUserContext(string userId, CancellationToken cancellationToken)
    {
        var user = await GetById(userId, cancellationToken);
        return user ?? new UserContext
            { UserId = userId, Configuration = new UserConfiguration(), CreatedAt = DateTime.UtcNow };
    }

    public async Task<bool> HasUserContext(string userId, CancellationToken cancellationToken)
    {
        var user = await GetById(userId, cancellationToken);
        return user != null;
    }

    public async Task<bool> HasUserGranted(string userId, CancellationToken cancellationToken)
    {
        var user = await GetById(userId, cancellationToken);
        return user?.ApiAccessTokens?.AccessTokenExpiration > DateTime.UtcNow && 
               user?.ApiAccessTokens?.RefreshTokenExpiration > DateTime.UtcNow;
    }

    public async Task<bool> RevokeGrant(string userId, CancellationToken cancellationToken)
    {
        var update = Builders<UserContext>.Update
            .Unset(u => u.ApiAccessTokens)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);
        return await Upsert(userId, update, cancellationToken);
    }

    public async Task<bool> UpsertApiAccessTokens(UserContext context, CancellationToken cancellationToken)
    {
        var update = Builders<UserContext>.Update
            .Set(u => u.UserId, context.UserId)
            .Set(u => u.ApiAccessTokens, context.ApiAccessTokens)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        return await Upsert(context.UserId, update, cancellationToken);
    }

    public async Task<bool> UpsertConfiguration(UserContext context, CancellationToken cancellationToken)
    {
        var update = Builders<UserContext>.Update
            .Set(u => u.Configuration, context.Configuration)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        return await Upsert(context.UserId, update, cancellationToken);
    }

    public async Task<IEnumerable<UserContext>> FindExpiredAccessTokens(DateTime threshold,
        CancellationToken cancellationToken)
    {
        var filter = Builders<UserContext>.Filter.Lte(c => c.ApiAccessTokens!.AccessTokenExpiration, threshold);
        return await GetManyBy(filter, cancellationToken);
    }
}