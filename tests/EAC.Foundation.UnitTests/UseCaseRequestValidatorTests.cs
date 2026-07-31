using EAC.Foundation.Application.Validation;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class UseCaseRequestValidatorTests
{
    [Fact(DisplayName = "Preserves the UseCase request, outcome and cancellation token through validation")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public async Task ValidatorContractPreservesRequestOutcomeAndCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var expectedOutcome = ValidationOutcome.Valid();
        var validator = new TestValidator(expectedOutcome);
        var useCaseRequest = new TestUseCaseRequest("POL-0001");

        var outcome = await validator.ValidateAsync(useCaseRequest, cancellation.Token);

        Assert.Same(expectedOutcome, outcome);
        Assert.Same(useCaseRequest, validator.ObservedUseCaseRequest);
        Assert.Equal(cancellation.Token, validator.ObservedCancellationToken);
    }

    private sealed record TestUseCaseRequest(string PolicyNumber);

    private sealed class TestValidator(ValidationOutcome outcome) :
        IUseCaseRequestValidator<TestUseCaseRequest>
    {
        public TestUseCaseRequest? ObservedUseCaseRequest { get; private set; }

        public CancellationToken ObservedCancellationToken { get; private set; }

        public ValueTask<ValidationOutcome> ValidateAsync(
            TestUseCaseRequest useCaseRequest,
            CancellationToken cancellationToken = default)
        {
            ObservedUseCaseRequest = useCaseRequest;
            ObservedCancellationToken = cancellationToken;
            return ValueTask.FromResult(outcome);
        }
    }
}
