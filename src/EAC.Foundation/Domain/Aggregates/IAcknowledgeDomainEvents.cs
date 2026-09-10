using EAC.Foundation.SharedKernel.Domain;

namespace EAC.Foundation.Domain;

/// <summary>Removes only domain events included in a successfully committed snapshot.</summary>
public interface IAcknowledgeDomainEvents : IHasDomainEvents
{
    /// <summary>Acknowledges the supplied event instances while preserving other pending events.</summary>
    /// <param name="domainEvents">The exact event instances persisted by the commit boundary.</param>
    void AcknowledgeDomainEvents(IReadOnlyCollection<IDomainEvent> domainEvents);
}
