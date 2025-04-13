namespace VreedaServiceSampleDotNet.Services;

using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public abstract class BaseMongoDbCollection<TDocument, TKey>
{
    private readonly IMongoCollection<TDocument> _collection;

    protected BaseMongoDbCollection(IMongoClient client, string databaseName, string collectionName)
    {
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<TDocument>(collectionName);
    }

    protected abstract FilterDefinition<TDocument> IdFilter(TKey id);

    protected async Task<TDocument?> GetById(TKey id, CancellationToken cancellationToken)
    {
        return await _collection.Find(IdFilter(id)).FirstOrDefaultAsync(cancellationToken);
    }

    protected async Task<IEnumerable<TDocument>> GetManyBy(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
    {
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    protected async Task<bool> Upsert(TKey id, UpdateDefinition<TDocument> update, CancellationToken cancellationToken)
    {
        var result = await _collection.UpdateOneAsync(
            IdFilter(id),
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken
        );
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
