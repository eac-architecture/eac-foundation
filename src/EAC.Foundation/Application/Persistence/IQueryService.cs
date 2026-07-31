namespace EAC.Foundation.Application.Persistence;

/// <summary>
/// Defines identity-based Query Side operations for a read model.
/// </summary>
/// <typeparam name="TReadModel">Read model or projection type.</typeparam>
/// <typeparam name="TId">Read model identifier type.</typeparam>
public interface IQueryService<TReadModel, in TId>
    where TId : notnull
{
    /// <summary>Finds a read model by identity.</summary>
    /// <param name="id">Read model identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The read model when found; otherwise, <see langword="null"/>.</returns>
    Task<TReadModel?> FindAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>Determines whether a matching read model exists.</summary>
    /// <param name="id">Read model identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><see langword="true"/> when a matching read model exists.</returns>
    Task<bool> ExistsAsync(
        TId id,
        CancellationToken cancellationToken = default);
}
