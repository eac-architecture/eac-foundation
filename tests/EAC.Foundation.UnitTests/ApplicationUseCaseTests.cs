using EAC.Foundation.Application.Commands;
using EAC.Foundation.Application.Queries;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ApplicationUseCaseTests
{
    [Fact(DisplayName = "Propagates cancellation through a command use case without a value")]
    [Trait("Rule", "EAC-CONF-APP-003")]
    public async Task CommandWithoutValueUseCaseReceivesCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var useCase = new PublishDocumentUseCase();

        var result = await useCase.ExecuteAsync(new PublishDocumentCommand(), cancellation.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(cancellation.Token, useCase.ObservedCancellationToken);
    }

    [Fact(DisplayName = "Propagates cancellation through a command use case with a value")]
    [Trait("Rule", "EAC-CONF-APP-003")]
    public async Task CommandWithValueUseCaseReceivesCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var expectedDocumentId = Guid.Parse("6d63aa8d-7348-4845-8c60-938652c35571");
        var useCase = new CreateDocumentUseCase(expectedDocumentId);

        var result = await useCase.ExecuteAsync(new CreateDocumentCommand(), cancellation.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedDocumentId, result.Value);
        Assert.Equal(cancellation.Token, useCase.ObservedCancellationToken);
    }

    [Fact(DisplayName = "Propagates cancellation through a query use case")]
    [Trait("Rule", "EAC-CONF-APP-003")]
    public async Task QueryUseCaseReceivesCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var expectedDocument = new DocumentSummary(
            Guid.Parse("977677ae-ea1f-4cfb-90be-999600cdf3bc"),
            "Architecture decision record");
        var useCase = new FindDocumentUseCase(expectedDocument);

        var result = await useCase.ExecuteAsync(new FindDocumentQuery(), cancellation.Token);

        Assert.True(result.IsSuccess);
        Assert.Same(expectedDocument, result.Value);
        Assert.Equal(cancellation.Token, useCase.ObservedCancellationToken);
    }

    private sealed record PublishDocumentCommand : ICommand;

    private sealed record CreateDocumentCommand : ICommand<Guid>;

    private sealed record FindDocumentQuery : IQuery<DocumentSummary>;

    private sealed record DocumentSummary(Guid Id, string Title);

    private sealed class PublishDocumentUseCase : ICommandUseCase<PublishDocumentCommand>
    {
        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<Result> ExecuteAsync(
            PublishDocumentCommand request,
            CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(Result.Success());
        }
    }

    private sealed class CreateDocumentUseCase(Guid documentId) :
        ICommandUseCase<CreateDocumentCommand, Guid>
    {
        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<Result<Guid>> ExecuteAsync(
            CreateDocumentCommand request,
            CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(Result<Guid>.Success(documentId));
        }
    }

    private sealed class FindDocumentUseCase(DocumentSummary document) :
        IQueryUseCase<FindDocumentQuery, DocumentSummary>
    {
        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<Result<DocumentSummary>> ExecuteAsync(
            FindDocumentQuery request,
            CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(Result<DocumentSummary>.Success(document));
        }
    }
}
