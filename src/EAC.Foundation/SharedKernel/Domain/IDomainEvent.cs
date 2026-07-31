namespace EAC.Foundation.SharedKernel.Domain;

/// <summary>
/// Identifies an immutable fact that occurred within a domain model.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Gets the unique identifier of this event instance.</summary>
    Guid EventId { get; }

    /// <summary>Gets the instant at which the domain fact occurred.</summary>
    DateTimeOffset OccurredAtUtc { get; }
}
