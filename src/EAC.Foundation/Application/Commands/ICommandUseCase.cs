using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;

namespace EAC.Foundation.Application.Commands;

/// <summary>
/// Defines a use case for a command that returns a result without a value.
/// </summary>
/// <typeparam name="TCommand">The command handled by the use case.</typeparam>
public interface ICommandUseCase<TCommand> : IUseCase<TCommand, Result>
    where TCommand : ICommand
{
}

/// <summary>
/// Defines a use case for a command that returns a result containing a value.
/// </summary>
/// <typeparam name="TCommand">The command handled by the use case.</typeparam>
/// <typeparam name="TValue">The value produced when the command succeeds.</typeparam>
public interface ICommandUseCase<TCommand, TValue> : IUseCase<TCommand, Result<TValue>>
    where TCommand : ICommand<TValue>
{
}
