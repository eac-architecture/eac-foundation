using EAC.Foundation.SharedKernel.Domain;

namespace EAC.Foundation.Domain;

/// <summary>
/// Exposes the ordered domain events raised by an aggregate.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>Gets a read-only view of the pending domain events.</summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>Returns a snapshot of the pending events and clears them from the aggregate.</summary>
    IReadOnlyCollection<IDomainEvent> DequeueDomainEvents();
}
