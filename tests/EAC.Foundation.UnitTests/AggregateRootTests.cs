using EAC.Foundation.Domain;
using EAC.Foundation.SharedKernel.Domain;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class AggregateRootTests
{
    [Fact(DisplayName = "Preserves the insertion order of raised domain events")]
    [Trait("Rule", "EAC-CONF-DOM-004")]
    public void RaisedEventsPreserveInsertionOrder()
    {
        var aggregate = new TestAggregate("aggregate-100");
        var first = CreateEvent("5b965f20-04c4-4a39-b50f-a71610332e8c", 1);
        var second = CreateEvent("3994b184-bb22-4d0c-9b55-e22491515bfb", 2);

        aggregate.Record(first);
        aggregate.Record(second);

        Assert.Equal([first, second], aggregate.DomainEvents);
    }

    [Fact(DisplayName = "Exposes pending domain events as a read-only view")]
    [Trait("Rule", "EAC-CONF-DOM-004")]
    public void DomainEventsExposeAReadOnlyView()
    {
        var aggregate = new TestAggregate("aggregate-100");
        var domainEvent = CreateEvent("2bf498e9-2d61-407f-b6be-2fc4e81e8533", 1);
        aggregate.Record(domainEvent);
        var collection = Assert.IsAssignableFrom<ICollection<IDomainEvent>>(aggregate.DomainEvents);

        Assert.True(collection.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => collection.Add(domainEvent));
        Assert.Single(aggregate.DomainEvents);
    }

    [Fact(DisplayName = "Dequeues an immutable snapshot and clears pending events")]
    [Trait("Rule", "EAC-CONF-DOM-004")]
    public void DequeueReturnsReadOnlySnapshotAndClearsPendingEvents()
    {
        var aggregate = new TestAggregate("aggregate-100");
        var first = CreateEvent("b88a4337-d33e-4b06-9965-aa1425672982", 1);
        var second = CreateEvent("3c9ec160-0818-493a-84b2-556dafbc7cf2", 2);
        aggregate.Record(first);
        aggregate.Record(second);

        var snapshot = aggregate.DequeueDomainEvents();
        var snapshotCollection = Assert.IsAssignableFrom<ICollection<IDomainEvent>>(snapshot);

        Assert.Equal([first, second], snapshot);
        Assert.Empty(aggregate.DomainEvents);
        Assert.True(snapshotCollection.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => snapshotCollection.Remove(first));
    }

    [Fact(DisplayName = "Keeps a dequeued snapshot isolated from later events")]
    [Trait("Rule", "EAC-CONF-DOM-004")]
    public void SnapshotIsNotAffectedByEventsRaisedAfterDequeue()
    {
        var aggregate = new TestAggregate("aggregate-100");
        var first = CreateEvent("1b963c92-cb7c-429f-88a7-e03f2d69fb4a", 1);
        var second = CreateEvent("0f709e03-2cb3-40f8-bc3b-fe09d7073a57", 2);
        aggregate.Record(first);

        var snapshot = aggregate.DequeueDomainEvents();
        aggregate.Record(second);

        Assert.Equal([first], snapshot);
        Assert.Equal([second], aggregate.DomainEvents);
    }

    [Fact(DisplayName = "Returns an empty snapshot when no events are pending")]
    [Trait("Rule", "EAC-CONF-DOM-004")]
    public void DequeueOnEmptyAggregateReturnsEmptySnapshot()
    {
        var aggregate = new TestAggregate("aggregate-100");

        var snapshot = aggregate.DequeueDomainEvents();

        Assert.Empty(snapshot);
        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact(DisplayName = "Rejects a null domain event without changing state")]
    [Trait("Rule", "EAC-CONF-DOM-004")]
    public void RaisingNullEventIsRejectedWithoutChangingCollection()
    {
        var aggregate = new TestAggregate("aggregate-100");

        Assert.Throws<ArgumentNullException>(() => aggregate.Record(null!));
        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact(DisplayName = "Supports non-Guid aggregate identifiers and entity semantics")]
    [Trait("Rule", "EAC-CONF-DOM-003")]
    public void AggregateSupportsNonGuidIdentifierAndEntitySemantics()
    {
        var left = new TestAggregate("aggregate-100");
        var right = new TestAggregate("aggregate-100");

        Assert.IsAssignableFrom<IAggregateRoot>(left);
        Assert.IsAssignableFrom<IHasDomainEvents>(left);
        Assert.Equal(left, right);
        Assert.Equal("aggregate-100", left.Id);
    }

    [Fact(DisplayName = "Creates a transient aggregate through the materializer constructor")]
    [Trait("Rule", "EAC-CONF-DOM-003")]
    public void MaterializerConstructorCreatesTransientAggregate()
    {
        var aggregate = new MaterializedAggregate();

        Assert.True(aggregate.IsTransient);
        Assert.Empty(aggregate.DomainEvents);
    }

    private static TestDomainEvent CreateEvent(string eventId, int sequence) =>
        new(
            Guid.Parse(eventId),
            new DateTimeOffset(2026, 7, 20, 21, sequence, 0, TimeSpan.Zero),
            sequence);

    private sealed class TestAggregate(string id) : AggregateRoot<string>(id)
    {
        public void Record(IDomainEvent domainEvent) => RaiseDomainEvent(domainEvent);
    }

    private sealed class MaterializedAggregate : AggregateRoot<string>;

    private sealed record TestDomainEvent(
        Guid EventId,
        DateTimeOffset OccurredAtUtc,
        int Sequence) : IDomainEvent;
}
