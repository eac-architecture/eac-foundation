namespace EAC.Foundation.SharedKernel.Results;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// </summary>
public sealed class Result
{
    private Result(IError? error)
    {
        Error = error;
        IsSuccess = error is null;
    }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the expected error when the operation failed.</summary>
    public IError? Error { get; }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(null);

    /// <summary>Creates a failed result containing exactly one error.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is <see langword="null"/>.</exception>
    public static Result Failure(IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>Maps the result by executing exactly one callback.</summary>
    /// <typeparam name="TResult">The mapped result type.</typeparam>
    /// <param name="onSuccess">Callback used for a successful result.</param>
    /// <param name="onFailure">Callback used for a failed result.</param>
    /// <returns>The value returned by the selected callback.</returns>
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<IError, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess() : onFailure(Error!);
    }
}
