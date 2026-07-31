using EAC.Foundation.Application.Commands;
using EAC.Foundation.Application.Pipeline;
using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class PipelineBehaviorTests
{
    [Fact(DisplayName = "Passes the UseCase request and cancellation token to the next operation")]
    [Trait("Rule", "EAC-CONF-APP-011")]
    public async Task BehaviorContractCanContinueThePipeline()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var request = new CreateDocumentCommand();
        var expectedResponse = Result<Guid>.Success(
            Guid.Parse("910fb021-d9d1-47d2-9cdc-09f9df848fdc"));
        var behavior = new RecordingBehavior<CreateDocumentCommand, Result<Guid>>();
        CancellationToken observedNextToken = default;

        var response = await behavior.ExecuteAsync(
            request,
            nextToken =>
            {
                observedNextToken = nextToken;
                return Task.FromResult(expectedResponse);
            },
            cancellation.Token);

        Assert.Same(expectedResponse, response);
        Assert.Same(request, behavior.ObservedUseCaseRequest);
        Assert.Equal(cancellation.Token, behavior.ObservedCancellationToken);
        Assert.Equal(cancellation.Token, observedNextToken);
    }

    [Fact(DisplayName = "Allows a Pipeline Behavior to return without invoking the next operation")]
    [Trait("Rule", "EAC-CONF-APP-011")]
    public async Task BehaviorContractCanShortCircuitThePipeline()
    {
        var expectedResponse = Result<Guid>.Failure(
            Error.Forbidden("document.forbidden", "The operation is not allowed."));
        var behavior = new ShortCircuitBehavior<CreateDocumentCommand, Result<Guid>>(expectedResponse);
        var nextCalls = 0;

        var response = await behavior.ExecuteAsync(
            new CreateDocumentCommand(),
            _ =>
            {
                nextCalls++;
                return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
            },
            TestContext.Current.CancellationToken);

        Assert.Same(expectedResponse, response);
        Assert.Equal(0, nextCalls);
    }

    private sealed record CreateDocumentCommand : ICommand<Guid>;

    private sealed class RecordingBehavior<TUseCaseRequest, TUseCaseResponse> :
        IPipelineBehavior<TUseCaseRequest, TUseCaseResponse>
        where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
    {
        public TUseCaseRequest? ObservedUseCaseRequest { get; private set; }

        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<TUseCaseResponse> ExecuteAsync(
            TUseCaseRequest useCaseRequest,
            UseCaseContinuation<TUseCaseResponse> continuation,
            CancellationToken cancellationToken = default)
        {
            ObservedUseCaseRequest = useCaseRequest;
            ObservedCancellationToken = cancellationToken;
            return continuation(cancellationToken);
        }
    }

    private sealed class ShortCircuitBehavior<TUseCaseRequest, TUseCaseResponse>(
        TUseCaseResponse response) : IPipelineBehavior<TUseCaseRequest, TUseCaseResponse>
        where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
    {
        public Task<TUseCaseResponse> ExecuteAsync(
            TUseCaseRequest useCaseRequest,
            UseCaseContinuation<TUseCaseResponse> continuation,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(response);
    }
}
