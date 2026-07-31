using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;

namespace EAC.Foundation.Application.Queries;

/// <summary>
/// Represents an in-process query that returns a result containing a value.
/// </summary>
/// <typeparam name="TValue">The value returned when the query succeeds.</typeparam>
public interface IQuery<TValue> : IUseCaseRequest<Result<TValue>>
{
}
