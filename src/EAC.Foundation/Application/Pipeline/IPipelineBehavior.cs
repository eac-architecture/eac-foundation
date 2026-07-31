using EAC.Foundation.Application.Requests;

namespace EAC.Foundation.Application.Pipeline;

/// <summary>
/// Defines a behavior that participates in a local use case execution pipeline.
/// </summary>
/// <typeparam name="TUseCaseRequest">The request processed by the pipeline.</typeparam>
/// <typeparam name="TUseCaseResponse">The complete response returned by the pipeline.</typeparam>
public interface IPipelineBehavior<TUseCaseRequest, TUseCaseResponse>
    where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
{
    /// <summary>
    /// Executes the behavior around the next operation in the pipeline.
    /// </summary>
    /// <param name="useCaseRequest">The use case request being processed.</param>
    /// <param name="continuation">The next operation in the pipeline.</param>
    /// <param name="cancellationToken">A token that propagates cancellation through the pipeline.</param>
    /// <returns>A task containing the statically declared use case response.</returns>
    Task<TUseCaseResponse> ExecuteAsync(
        TUseCaseRequest useCaseRequest,
        UseCaseContinuation<TUseCaseResponse> continuation,
        CancellationToken cancellationToken = default);
}
