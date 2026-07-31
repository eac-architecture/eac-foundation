using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;

namespace EAC.Foundation.Application.Queries;

/// <summary>
/// Defines a use case for a query that returns a result containing a value.
/// </summary>
/// <typeparam name="TQuery">The query handled by the use case.</typeparam>
/// <typeparam name="TValue">The value returned when the query succeeds.</typeparam>
public interface IQueryUseCase<TQuery, TValue> : IUseCase<TQuery, Result<TValue>>
    where TQuery : IQuery<TValue>
{
}
