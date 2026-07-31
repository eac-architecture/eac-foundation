using EAC.Foundation.Domain;

namespace EAC.Foundation.Application.Persistence;

/// <summary>
/// Defines a Command Side repository that loads and tracks aggregate roots only.
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type.</typeparam>
/// <typeparam name="TId">Aggregate identifier type.</typeparam>
public interface IRepository<TAggregate, in TId>
    where TAggregate : class, IAggregateRoot, IEntity<TId>
    where TId : notnull
{
    /// <summary>Loads an aggregate root for command execution.</summary>
    /// <param name="id">Aggregate identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The aggregate root when found; otherwise, <see langword="null"/>.</returns>
    Task<TAggregate?> FindAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new aggregate root to the local persistence context.</summary>
    /// <param name="aggregate">Aggregate root to add.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A value task representing the add operation.</returns>
    ValueTask AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>Marks an aggregate root for removal from the local persistence context.</summary>
    /// <param name="aggregate">Aggregate root to remove.</param>
    void Remove(TAggregate aggregate);
}
