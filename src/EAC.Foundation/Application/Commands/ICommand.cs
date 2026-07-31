using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;

namespace EAC.Foundation.Application.Commands;

/// <summary>
/// Represents an in-process command that returns a result without a value.
/// </summary>
public interface ICommand : IUseCaseRequest<Result>
{
}

/// <summary>
/// Represents an in-process command that returns a result containing a value.
/// </summary>
/// <typeparam name="TValue">The value produced when the command succeeds.</typeparam>
public interface ICommand<TValue> : IUseCaseRequest<Result<TValue>>
{
}
