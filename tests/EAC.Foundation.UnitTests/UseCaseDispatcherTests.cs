using EAC.Foundation.Application.Commands;
using EAC.Foundation.Application.Dispatching;
using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class UseCaseDispatcherTests
{
    [Fact(DisplayName = "Preserves the declared response and cancellation token through dispatch")]
    [Trait("Rule", "EAC-CONF-APP-010")]
    public async Task DispatcherContractPreservesResponseAndCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var expectedDocumentId = Guid.Parse("41ac939e-3ee7-4a47-ab05-02d77cf472c5");
        var expectedResponse = Result<Guid>.Success(expectedDocumentId);
        var dispatcher = new TestUseCaseDispatcher(expectedResponse);
        IUseCaseRequest<Result<Guid>> useCaseRequest = new CreateDocumentCommand();

        var response = await dispatcher.DispatchAsync(useCaseRequest, cancellation.Token);

        Assert.Same(expectedResponse, response);
        Assert.Same(useCaseRequest, dispatcher.ObservedUseCaseRequest);
        Assert.Equal(cancellation.Token, dispatcher.ObservedCancellationToken);
    }

    private sealed record CreateDocumentCommand : ICommand<Guid>;

    private sealed class TestUseCaseDispatcher(object response) : IUseCaseDispatcher
    {
        public object? ObservedUseCaseRequest { get; private set; }

        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<TUseCaseResponse> DispatchAsync<TUseCaseResponse>(
            IUseCaseRequest<TUseCaseResponse> useCaseRequest,
            CancellationToken cancellationToken = default)
        {
            ObservedUseCaseRequest = useCaseRequest;
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult((TUseCaseResponse)response);
        }
    }
}
