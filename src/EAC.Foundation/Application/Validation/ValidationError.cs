using EAC.Foundation.SharedKernel.Results;

namespace EAC.Foundation.Application.Validation;

/// <summary>
/// Represents an expected validation error with its ordered failures.
/// </summary>
public sealed record ValidationError : IError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> record.
    /// </summary>
    /// <param name="code">Stable, non-localized error code.</param>
    /// <param name="description">Consumer-safe error description.</param>
    /// <param name="failures">Ordered validation failures.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="code"/>, <paramref name="description"/> or <paramref name="failures"/> is
    /// <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The error contract is malformed, no failure is supplied, or the collection contains a
    /// <see langword="null"/> item.
    /// </exception>
    public ValidationError(
        string code,
        string description,
        IReadOnlyCollection<ValidationFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);

        var error = Error.Validation(code, description);
        var failureSnapshot = failures.ToArray();

        if (failureSnapshot.Length == 0)
        {
            throw new ArgumentException(
                "A validation error must contain at least one failure.",
                nameof(failures));
        }

        if (Array.Exists(failureSnapshot, static failure => failure is null))
        {
            throw new ArgumentException(
                "A validation error cannot contain a null failure.",
                nameof(failures));
        }

        Code = error.Code;
        Description = error.Description;
        Failures = Array.AsReadOnly(failureSnapshot);
    }

    /// <inheritdoc />
    public string Code { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public ErrorType Type => ErrorType.Validation;

    /// <summary>Gets an immutable snapshot of the ordered validation failures.</summary>
    public IReadOnlyCollection<ValidationFailure> Failures { get; }
}
