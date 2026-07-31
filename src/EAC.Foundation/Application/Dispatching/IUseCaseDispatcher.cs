using EAC.Foundation.Application.Requests;

namespace EAC.Foundation.Application.Dispatching;

/// <summary>
/// Defines local, in-process dispatching for use case requests.
/// </summary>
public interface IUseCaseDispatcher
{
    /// <summary>
    /// Dispatches a use case request and returns its statically declared response.
    /// </summary>
    /// <typeparam name="TUseCaseResponse">The complete response returned by the use case.</typeparam>
    /// <param name="useCaseRequest">The use case request to dispatch.</param>
    /// <param name="cancellationToken">A token that propagates cancellation through the execution pipeline.</param>
    /// <returns>A task containing the statically declared use case response.</returns>
    Task<TUseCaseResponse> DispatchAsync<TUseCaseResponse>(
        IUseCaseRequest<TUseCaseResponse> useCaseRequest,
        CancellationToken cancellationToken = default);
}
