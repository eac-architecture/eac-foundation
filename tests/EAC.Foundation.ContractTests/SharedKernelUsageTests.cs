using EAC.Foundation.SharedKernel.Domain;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class SharedKernelUsageTests
{
    [Fact(DisplayName = "Supports the approved SharedKernel consumer contract")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void ConsumerCanUseTheApprovedSharedKernelContract()
    {
        var eventId = Guid.Parse("8cbe7c82-58f5-4a12-80e4-5d65f4611033");
        var occurredAt = new DateTimeOffset(2026, 7, 20, 19, 0, 0, TimeSpan.Zero);

        var result = Publish(eventId, occurredAt, canPublish: true);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventId, result.Value?.EventId);
        Assert.Equal(occurredAt, result.Value?.OccurredAtUtc);
    }

    private static Result<DocumentPublished> Publish(
        Guid eventId,
        DateTimeOffset occurredAtUtc,
        bool canPublish)
    {
        if (!canPublish)
        {
            return Result<DocumentPublished>.Failure(
                Error.Conflict("document.invalid-state", "The document cannot be published."));
        }

        IDomainEvent domainEvent = new DocumentPublished(eventId, occurredAtUtc, Guid.Empty);
        return Result<DocumentPublished>.Success((DocumentPublished)domainEvent);
    }

    private sealed record DocumentPublished(
        Guid EventId,
        DateTimeOffset OccurredAtUtc,
        Guid DocumentId) : IDomainEvent;
}
