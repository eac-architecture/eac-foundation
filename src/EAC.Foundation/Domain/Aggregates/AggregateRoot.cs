using System.Collections.ObjectModel;
using EAC.Foundation.SharedKernel.Domain;

namespace EAC.Foundation.Domain;

/// <summary>
/// Provides optional ordered domain-event accumulation for an aggregate root.
/// </summary>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly ReadOnlyCollection<IDomainEvent> _domainEventsView;

    /// <summary>Initializes a transient aggregate for use by materializers.</summary>
    protected AggregateRoot()
    {
        _domainEventsView = _domainEvents.AsReadOnly();
    }

    /// <summary>Initializes an aggregate with its identifier.</summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    protected AggregateRoot(TId id)
        : base(id)
    {
        _domainEventsView = _domainEvents.AsReadOnly();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEventsView;

    /// <summary>Adds a domain event to the end of the pending sequence.</summary>
    /// <param name="domainEvent">The domain event raised by the aggregate.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="domainEvent"/> is <see langword="null"/>.
    /// </exception>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DequeueDomainEvents()
    {
        if (_domainEvents.Count == 0)
        {
            return Array.Empty<IDomainEvent>();
        }

        var snapshot = Array.AsReadOnly(_domainEvents.ToArray());
        _domainEvents.Clear();
        return snapshot;
    }
}
