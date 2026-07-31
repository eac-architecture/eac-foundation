namespace EAC.Foundation.Application.Pipeline;

/// <summary>
/// Represents the next operation in a local use case execution pipeline.
/// </summary>
/// <typeparam name="TUseCaseResponse">The complete response returned by the use case pipeline.</typeparam>
/// <param name="cancellationToken">A token that propagates cancellation to the next operation.</param>
/// <returns>A task containing the statically declared use case response.</returns>
public delegate Task<TUseCaseResponse> UseCaseContinuation<TUseCaseResponse>(
    CancellationToken cancellationToken);
