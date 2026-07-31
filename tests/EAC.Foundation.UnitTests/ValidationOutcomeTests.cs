using EAC.Foundation.Application.Validation;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ValidationOutcomeTests
{
    [Fact(DisplayName = "Creates a valid outcome without failures")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidContainsNoFailures()
    {
        var outcome = ValidationOutcome.Valid();

        Assert.True(outcome.IsValid);
        Assert.Empty(outcome.Failures);
        Assert.False(outcome.Failures is ValidationFailure[]);
    }

    [Fact(DisplayName = "Creates an invalid outcome with ordered immutable failures")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void InvalidPreservesOrderedImmutableFailures()
    {
        var source = new List<ValidationFailure>
        {
            CreateFailure("policy.startDate"),
            CreateFailure("policy.endDate"),
        };

        var outcome = ValidationOutcome.Invalid(source);

        source.Reverse();
        source.Add(CreateFailure("policy.number"));

        Assert.False(outcome.IsValid);
        Assert.Equal(["policy.startDate", "policy.endDate"], outcome.Failures.Select(failure => failure.Field));
        Assert.IsAssignableFrom<IReadOnlyCollection<ValidationFailure>>(outcome.Failures);
        Assert.False(outcome.Failures is ValidationFailure[]);
    }

    [Fact(DisplayName = "Rejects an invalid outcome without failures")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void InvalidRejectsEmptyFailures()
    {
        Assert.Throws<ArgumentException>(() => ValidationOutcome.Invalid([]));
    }

    [Fact(DisplayName = "Rejects a null invalid outcome sequence")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void InvalidRejectsNullFailures()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationOutcome.Invalid(null!));
    }

    [Fact(DisplayName = "Rejects an invalid outcome containing a null failure")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void InvalidRejectsNullFailureItem()
    {
        IEnumerable<ValidationFailure> failures = [null!];

        Assert.Throws<ArgumentException>(() => ValidationOutcome.Invalid(failures));
    }

    private static ValidationFailure CreateFailure(string field) =>
        new(field, "policy.invalid-value", "The value is invalid.");
}
