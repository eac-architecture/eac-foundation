using EAC.Foundation.Application.Commands;
using EAC.Foundation.Application.Queries;
using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class UseCaseRequestTests
{
    [Fact(DisplayName = "Declares Result as the response of a command without a value")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void CommandWithoutValueDeclaresResultResponse()
    {
        ICommand command = new PublishDocumentCommand();

        IUseCaseRequest<Result> request = command;

        Assert.Same(command, request);
    }

    [Fact(DisplayName = "Declares generic Result as the response of a command with a value")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void CommandWithValueDeclaresGenericResultResponse()
    {
        ICommand<Guid> command = new CreateDocumentCommand();

        IUseCaseRequest<Result<Guid>> request = command;

        Assert.Same(command, request);
    }

    [Fact(DisplayName = "Declares generic Result as the response of a query")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void QueryDeclaresGenericResultResponse()
    {
        IQuery<DocumentSummary> query = new FindDocumentQuery();

        IUseCaseRequest<Result<DocumentSummary>> request = query;

        Assert.Same(query, request);
    }

    private sealed record PublishDocumentCommand : ICommand;

    private sealed record CreateDocumentCommand : ICommand<Guid>;

    private sealed record FindDocumentQuery : IQuery<DocumentSummary>;

    private sealed record DocumentSummary(Guid Id, string Title);
}
