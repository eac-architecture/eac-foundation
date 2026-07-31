namespace EAC.Foundation.Application.Requests;

/// <summary>
/// Defines a single in-process application use case for a request and its response.
/// </summary>
/// <typeparam name="TUseCaseRequest">The request handled by the use case.</typeparam>
/// <typeparam name="TUseCaseResponse">The complete response returned by the use case.</typeparam>
public interface IUseCase<TUseCaseRequest, TUseCaseResponse>
    where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
{
    /// <summary>
    /// Executes the use case for the supplied request.
    /// </summary>
    /// <param name="useCaseRequest">The use case request to execute.</param>
    /// <param name="cancellationToken">A token that propagates cancellation to application dependencies.</param>
    /// <returns>A task containing the statically declared response.</returns>
    Task<TUseCaseResponse> ExecuteAsync(
        TUseCaseRequest useCaseRequest,
        CancellationToken cancellationToken = default);
}
