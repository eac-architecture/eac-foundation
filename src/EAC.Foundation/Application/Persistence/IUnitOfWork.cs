namespace EAC.Foundation.Application.Persistence;

/// <summary>
/// Defines the commit boundary for one local unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Commits the pending local changes.</summary>
    /// <param name="cancellationToken">Token used to cancel the commit operation.</param>
    /// <returns>The explicit local commit result.</returns>
    Task<CommitResult> CommitAsync(
        CancellationToken cancellationToken = default);
}
