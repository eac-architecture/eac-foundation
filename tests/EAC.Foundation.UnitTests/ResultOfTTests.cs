using System.Globalization;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ResultOfTTests
{
    private static readonly Error ExpectedError =
        Error.NotFound("document.not-found", "Safe description.");

    [Fact(DisplayName = "Creates a successful generic Result with its value")]
    [Trait("Rule", "EAC-CONF-FOUND-004")]
    public void SuccessPreservesValueAndContainsNoError()
    {
        var value = new object();

        var result = Result<object>.Success(value);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Same(value, result.Value);
        Assert.Null(result.Error);
    }

    [Fact(DisplayName = "Accepts null as a valid nullable success value")]
    [Trait("Rule", "EAC-CONF-FOUND-004")]
    public void NullableSuccessPreservesNullAsAValidValue()
    {
        var result = Result<string?>.Success(null);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Null(result.Error);
    }

    [Fact(DisplayName = "Creates a failed generic Result without a success value")]
    [Trait("Rule", "EAC-CONF-FOUND-004")]
    public void FailureContainsErrorAndNoSuccessfulValue()
    {
        var result = Result<string>.Failure(ExpectedError);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Same(ExpectedError, result.Error);
    }

    [Fact(DisplayName = "Exposes only the default value for a failed value-type Result")]
    [Trait("Rule", "EAC-CONF-FOUND-004")]
    public void ValueTypeFailureExposesOnlyItsDefaultValue()
    {
        var result = Result<int>.Failure(ExpectedError);

        Assert.Equal(default, result.Value);
        Assert.Same(ExpectedError, result.Error);
    }

    [Fact(DisplayName = "Rejects a null error for a failed generic Result")]
    [Trait("Rule", "EAC-CONF-FOUND-004")]
    public void FailureRejectsNullError()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
    }

    [Fact(DisplayName = "Executes only the success branch for a successful generic Result")]
    [Trait("Rule", "EAC-CONF-FOUND-005")]
    public void SuccessfulMatchExecutesOnlySuccessBranch()
    {
        var successCalls = 0;
        var failureCalls = 0;

        var output = Result<int>.Success(42).Match(
            value =>
            {
                successCalls++;
                return value.ToString(CultureInfo.InvariantCulture);
            },
            _ =>
            {
                failureCalls++;
                return "failure";
            });

        Assert.Equal("42", output);
        Assert.Equal(1, successCalls);
        Assert.Equal(0, failureCalls);
    }

    [Fact(DisplayName = "Executes only the failure branch for a failed generic Result")]
    [Trait("Rule", "EAC-CONF-FOUND-005")]
    public void FailedMatchExecutesOnlyFailureBranch()
    {
        var successCalls = 0;
        var failureCalls = 0;

        var output = Result<int>.Failure(ExpectedError).Match(
            value =>
            {
                successCalls++;
                return value.ToString(CultureInfo.InvariantCulture);
            },
            error =>
            {
                failureCalls++;
                Assert.Same(ExpectedError, error);
                return "failure";
            });

        Assert.Equal("failure", output);
        Assert.Equal(0, successCalls);
        Assert.Equal(1, failureCalls);
    }

    [Fact(DisplayName = "Rejects null Match callbacks for a generic Result")]
    [Trait("Rule", "EAC-CONF-FOUND-005")]
    public void MatchRejectsNullCallbacks()
    {
        var success = Result<int>.Success(42);

        Assert.Throws<ArgumentNullException>(() => success.Match<string>(null!, _ => "failure"));
        Assert.Throws<ArgumentNullException>(
            () => success.Match(value => value.ToString(CultureInfo.InvariantCulture), null!));
    }
}
