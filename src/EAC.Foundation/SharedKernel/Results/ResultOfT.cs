using System.Diagnostics.CodeAnalysis;

namespace EAC.Foundation.SharedKernel.Results;

/// <summary>
/// Represents the outcome of an operation that returns a value on success.
/// </summary>
/// <typeparam name="TValue">The successful value type.</typeparam>
public sealed class Result<TValue>
{
    private readonly TValue? value;

    private Result(bool isSuccess, TValue? value, IError? error)
    {
        IsSuccess = isSuccess;
        this.value = value;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the successful value, or the default value when the operation failed.</summary>
    public TValue? Value => value;

    /// <summary>Gets the expected error when the operation failed.</summary>
    public IError? Error { get; }

    /// <summary>Creates a successful result containing the supplied value.</summary>
    [SuppressMessage(
        "Design",
        "CA1000:Do not declare static members on generic types",
        Justification = "Result<TValue>.Success is part of the approved fluent creation contract.")]
    public static Result<TValue> Success(TValue value) => new(true, value, null);

    /// <summary>Creates a failed result containing exactly one error and no successful value.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is <see langword="null"/>.</exception>
    [SuppressMessage(
        "Design",
        "CA1000:Do not declare static members on generic types",
        Justification = "Result<TValue>.Failure is part of the approved fluent creation contract.")]
    public static Result<TValue> Failure(IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, default, error);
    }

    /// <summary>Maps the result by executing exactly one callback.</summary>
    /// <typeparam name="TResult">The mapped result type.</typeparam>
    /// <param name="onSuccess">Callback used for a successful result.</param>
    /// <param name="onFailure">Callback used for a failed result.</param>
    /// <returns>The value returned by the selected callback.</returns>
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<IError, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess(value!) : onFailure(Error!);
    }
}
