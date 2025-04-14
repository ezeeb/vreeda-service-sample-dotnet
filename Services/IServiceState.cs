namespace VreedaServiceSampleDotNet.Services;

using Models;

/// <summary>
/// Service for managing user context and API access tokens
/// </summary>
public interface IServiceState
{
    /// <summary>
    /// Checks if a user context exists for the specified user ID
    /// </summary>
    /// <param name="userId">Unique identifier of the user</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if a user context exists, otherwise False</returns>
    public Task<bool> HasUserContext(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the user has granted access to the Vreeda API
    /// </summary>
    /// <param name="userId">Unique identifier of the user</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if access has been granted, otherwise False</returns>
    public Task<bool> HasUserGranted(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the user context or creates a new one if none exists
    /// </summary>
    /// <param name="userId">Unique identifier of the user</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>User context</returns>
    public Task<UserContext> GetOrCreateUserContext(string userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates the API access tokens for a user
    /// </summary>
    /// <param name="context">User context with updated tokens</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if update was successful, otherwise False</returns>
    public Task<bool> UpsertApiAccessTokens(UserContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Revokes a user's access rights and deletes their context
    /// </summary>
    /// <param name="userId">Unique identifier of the user</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if deletion was successful, otherwise False</returns>
    public Task<bool> RevokeGrant(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a user's configuration
    /// </summary>
    /// <param name="context">User context with updated configuration</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if update was successful, otherwise False</returns>
    public Task<bool> UpsertConfiguration(UserContext context, CancellationToken cancellationToken);
    
    /// <summary>
    /// Finds users with expired access tokens
    /// </summary>
    /// <param name="threshold">Time from which tokens are considered expired</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of user contexts with expired tokens</returns>
    public Task<IEnumerable<UserContext>> FindExpiredAccessTokens(DateTime threshold,
        CancellationToken cancellationToken);
}

