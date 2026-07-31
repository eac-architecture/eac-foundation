namespace EAC.Foundation.Application.Requests;

/// <summary>
/// Defines an in-process use case request with a statically known response type.
/// </summary>
/// <typeparam name="TUseCaseResponse">The complete response returned by the use case.</typeparam>
public interface IUseCaseRequest<TUseCaseResponse>
{
}
