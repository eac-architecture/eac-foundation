using EAC.Foundation.SharedKernel.Domain;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class DomainEventTests
{
    [Fact(DisplayName = "Preserves domain event identity and occurrence time")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void ConsumerEventPreservesRequiredIdentityAndOccurrenceTime()
    {
        var eventId = Guid.Parse("14bb0d92-71b8-4c40-91a5-4b31f9f18f45");
        var occurredAt = new DateTimeOffset(2026, 7, 20, 18, 30, 0, TimeSpan.Zero);

        IDomainEvent domainEvent = new TestDomainEvent(eventId, occurredAt);

        Assert.Equal(eventId, domainEvent.EventId);
        Assert.Equal(occurredAt, domainEvent.OccurredAtUtc);
    }

    private sealed record TestDomainEvent(Guid EventId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
}
