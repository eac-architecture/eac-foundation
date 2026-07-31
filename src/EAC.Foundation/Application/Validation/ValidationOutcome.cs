namespace EAC.Foundation.Application.Validation;

/// <summary>
/// Represents the immutable outcome of validating a use case request.
/// </summary>
public sealed class ValidationOutcome
{
    private static readonly ValidationOutcome ValidOutcome =
        new(Array.AsReadOnly(Array.Empty<ValidationFailure>()));

    private ValidationOutcome(IReadOnlyCollection<ValidationFailure> failures)
    {
        Failures = failures;
    }

    /// <summary>Gets a value indicating whether validation succeeded.</summary>
    public bool IsValid => Failures.Count == 0;

    /// <summary>Gets an immutable snapshot of the ordered validation failures.</summary>
    public IReadOnlyCollection<ValidationFailure> Failures { get; }

    /// <summary>Creates a successful validation outcome without failures.</summary>
    /// <returns>The immutable successful validation outcome.</returns>
    public static ValidationOutcome Valid() => ValidOutcome;

    /// <summary>Creates a failed validation outcome from ordered failures.</summary>
    /// <param name="failures">Validation failures in their original order.</param>
    /// <returns>An immutable failed validation outcome.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="failures"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// No failure is supplied or the sequence contains a <see langword="null"/> item.
    /// </exception>
    public static ValidationOutcome Invalid(IEnumerable<ValidationFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);

        var failureSnapshot = failures.ToArray();

        if (failureSnapshot.Length == 0)
        {
            throw new ArgumentException(
                "An invalid validation outcome must contain at least one failure.",
                nameof(failures));
        }

        if (Array.Exists(failureSnapshot, static failure => failure is null))
        {
            throw new ArgumentException(
                "An invalid validation outcome cannot contain a null failure.",
                nameof(failures));
        }

        return new ValidationOutcome(Array.AsReadOnly(failureSnapshot));
    }
}
