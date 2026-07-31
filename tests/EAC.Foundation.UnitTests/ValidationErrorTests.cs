using EAC.Foundation.Application.Validation;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ValidationErrorTests
{
    [Fact(DisplayName = "Preserves validation failure values including a request-level failure")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationFailurePreservesApprovedValues()
    {
        var failure = new ValidationFailure(
            string.Empty,
            "policy.invalid-period",
            "The policy period is invalid.");

        Assert.Empty(failure.Field);
        Assert.Equal("policy.invalid-period", failure.Code);
        Assert.Equal("The policy period is invalid.", failure.Message);
    }

    [Fact(DisplayName = "Uses structural equality for validation failures")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void EqualValidationFailuresUseStructuralEquality()
    {
        var first = CreateFailure("policy.startDate");
        var second = CreateFailure("policy.startDate");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory(DisplayName = "Rejects an invalid validation failure field")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("\t")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationFailureRejectsInvalidField(string? field)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ValidationFailure(field!, "policy.required", "The value is required."));
    }

    [Theory(DisplayName = "Rejects an invalid validation failure code")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Policy.Required")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationFailureRejectsInvalidCode(string? code)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ValidationFailure("policy.number", code!, "The value is required."));
    }

    [Theory(DisplayName = "Rejects an invalid validation failure message")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationFailureRejectsInvalidMessage(string? message)
    {
        var exception = Assert.ThrowsAny<ArgumentException>(
            () => new ValidationFailure("policy.number", "policy.required", message!));

        Assert.Equal("message", exception.ParamName);
    }

    [Fact(DisplayName = "Creates a validation error with ordered immutable failures")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationErrorPreservesOrderedImmutableFailures()
    {
        var source = new List<ValidationFailure>
        {
            CreateFailure("policy.startDate"),
            CreateFailure("policy.endDate"),
        };

        var error = new ValidationError(
            "policy.invalid",
            "The policy is invalid.",
            source);

        source.Reverse();
        source.Add(CreateFailure("policy.number"));

        Assert.Equal("policy.invalid", error.Code);
        Assert.Equal("The policy is invalid.", error.Description);
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Equal(["policy.startDate", "policy.endDate"], error.Failures.Select(failure => failure.Field));
        Assert.IsAssignableFrom<IReadOnlyCollection<ValidationFailure>>(error.Failures);
        Assert.False(error.Failures is ValidationFailure[]);
    }

    [Fact(DisplayName = "Rejects a validation error without failures")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationErrorRejectsEmptyFailures()
    {
        Assert.Throws<ArgumentException>(
            () => new ValidationError("policy.invalid", "The policy is invalid.", []));
    }

    [Fact(DisplayName = "Rejects a null validation failure collection")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationErrorRejectsNullFailures()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ValidationError("policy.invalid", "The policy is invalid.", null!));
    }

    [Fact(DisplayName = "Rejects a validation error containing a null failure")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationErrorRejectsNullFailureItem()
    {
        IReadOnlyCollection<ValidationFailure> failures = [null!];

        Assert.Throws<ArgumentException>(
            () => new ValidationError("policy.invalid", "The policy is invalid.", failures));
    }

    private static ValidationFailure CreateFailure(string field) =>
        new(field, "policy.invalid-value", "The value is invalid.");
}
