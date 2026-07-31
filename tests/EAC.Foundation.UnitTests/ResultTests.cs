using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ResultTests
{
    private static readonly Error ExpectedError =
        Error.Conflict("document.invalid-state", "Safe description.");

    [Fact(DisplayName = "Creates a successful Result without an error")]
    [Trait("Rule", "EAC-CONF-FOUND-003")]
    public void SuccessContainsNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact(DisplayName = "Creates a failed Result with the supplied error")]
    [Trait("Rule", "EAC-CONF-FOUND-003")]
    public void FailureContainsExactlyTheSuppliedError()
    {
        var result = Result.Failure(ExpectedError);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(ExpectedError, result.Error);
    }

    [Fact(DisplayName = "Rejects a null error when creating a failed Result")]
    [Trait("Rule", "EAC-CONF-FOUND-003")]
    public void FailureRejectsNullError()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    }

    [Fact(DisplayName = "Executes only the success branch for a successful Result")]
    [Trait("Rule", "EAC-CONF-FOUND-005")]
    public void SuccessfulMatchExecutesOnlySuccessBranch()
    {
        var successCalls = 0;
        var failureCalls = 0;

        var value = Result.Success().Match(
            () =>
            {
                successCalls++;
                return "success";
            },
            _ =>
            {
                failureCalls++;
                return "failure";
            });

        Assert.Equal("success", value);
        Assert.Equal(1, successCalls);
        Assert.Equal(0, failureCalls);
    }

    [Fact(DisplayName = "Executes only the failure branch for a failed Result")]
    [Trait("Rule", "EAC-CONF-FOUND-005")]
    public void FailedMatchExecutesOnlyFailureBranch()
    {
        var successCalls = 0;
        var failureCalls = 0;

        var value = Result.Failure(ExpectedError).Match(
            () =>
            {
                successCalls++;
                return "success";
            },
            error =>
            {
                failureCalls++;
                Assert.Same(ExpectedError, error);
                return "failure";
            });

        Assert.Equal("failure", value);
        Assert.Equal(0, successCalls);
        Assert.Equal(1, failureCalls);
    }

    [Fact(DisplayName = "Rejects null Match callbacks for Result")]
    [Trait("Rule", "EAC-CONF-FOUND-005")]
    public void MatchRejectsNullCallbacks()
    {
        var success = Result.Success();

        Assert.Throws<ArgumentNullException>(() => success.Match<string>(null!, _ => "failure"));
        Assert.Throws<ArgumentNullException>(() => success.Match(() => "success", null!));
    }
}
