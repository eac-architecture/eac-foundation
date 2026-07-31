using EAC.Foundation.Application.Persistence;
using EAC.Foundation.Domain;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class PersistencePortTests
{
    [Fact(DisplayName = "Preserves aggregate operations and cancellation through a Repository")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public async Task RepositoryContractPreservesAggregateOperationsAndCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var aggregate = new TestAggregate(Guid.Parse("ad1006ba-2c7e-45db-af55-6ee8667a5065"));
        var repository = new TestRepository();

        await repository.AddAsync(aggregate, cancellation.Token);
        var found = await repository.FindAsync(aggregate.Id, cancellation.Token);
        repository.Remove(aggregate);

        Assert.Same(aggregate, found);
        Assert.Same(aggregate, repository.RemovedAggregate);
        Assert.Equal(cancellation.Token, repository.ObservedCancellationToken);
    }

    [Fact(DisplayName = "Preserves commit result and cancellation through a Unit of Work")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public async Task UnitOfWorkContractPreservesResultAndCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var expectedResult = new CommitResult(2);
        var unitOfWork = new TestUnitOfWork(expectedResult);

        var result = await unitOfWork.CommitAsync(cancellation.Token);

        Assert.Equal(expectedResult, result);
        Assert.Equal(cancellation.Token, unitOfWork.ObservedCancellationToken);
    }

    [Fact(DisplayName = "Preserves read models and cancellation through a Query Service")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public async Task QueryServiceContractPreservesReadModelsAndCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var id = Guid.Parse("4f90ea58-a6b0-4de8-89ca-581cd3f5b240");
        var expectedModel = new TestReadModel(id, "POL-0001");
        var queryService = new TestQueryService(expectedModel);

        var found = await queryService.FindAsync(id, cancellation.Token);
        var exists = await queryService.ExistsAsync(id, cancellation.Token);

        Assert.Same(expectedModel, found);
        Assert.True(exists);
        Assert.Equal(cancellation.Token, queryService.ObservedCancellationToken);
    }

    private sealed class TestAggregate(Guid id) : AggregateRoot<Guid>(id);

    private sealed record TestReadModel(Guid Id, string PolicyNumber);

    private sealed class TestRepository : IRepository<TestAggregate, Guid>
    {
        private TestAggregate? _aggregate;

        public TestAggregate? RemovedAggregate { get; private set; }

        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<TestAggregate?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(_aggregate?.Id == id ? _aggregate : null);
        }

        public ValueTask AddAsync(
            TestAggregate aggregate,
            CancellationToken cancellationToken = default)
        {
            _aggregate = aggregate;
            ObservedCancellationToken = cancellationToken;
            return ValueTask.CompletedTask;
        }

        public void Remove(TestAggregate aggregate)
        {
            RemovedAggregate = aggregate;
            _aggregate = null;
        }
    }

    private sealed class TestUnitOfWork(CommitResult result) : IUnitOfWork
    {
        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<CommitResult> CommitAsync(CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(result);
        }
    }

    private sealed class TestQueryService(TestReadModel readModel) :
        IQueryService<TestReadModel, Guid>
    {
        public CancellationToken ObservedCancellationToken { get; private set; }

        public Task<TestReadModel?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(readModel.Id == id ? readModel : null);
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ObservedCancellationToken = cancellationToken;
            return Task.FromResult(readModel.Id == id);
        }
    }
}
