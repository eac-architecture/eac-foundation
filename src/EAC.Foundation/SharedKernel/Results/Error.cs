using System.Diagnostics.CodeAnalysis;

namespace EAC.Foundation.SharedKernel.Results;

/// <summary>
/// Represents an immutable expected error.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Error is the approved cross-language public contract name.")]
public sealed record Error : IError
{
    private const int MaximumCodeLength = 128;

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> record.
    /// </summary>
    /// <param name="code">Stable, non-localized error code.</param>
    /// <param name="description">Consumer-safe description.</param>
    /// <param name="type">Protocol-neutral error classification.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="code"/> or <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// A value is empty, malformed, or outside the supported classification set.
    /// </exception>
    public Error(string code, string description, ErrorType type)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(description);

        if (!IsWellFormedCode(code))
        {
            throw new ArgumentException(
                "The error code must be a lower-case ASCII code of at most 128 characters, " +
                "with segments separated by '.', '-' or '_'.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("The error description cannot be empty or whitespace.", nameof(description));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentException("The error type is not supported.", nameof(type));
        }

        Code = code;
        Description = description;
        Type = type;
    }

    /// <inheritdoc />
    public string Code { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public ErrorType Type { get; }

    /// <summary>Creates a general expected failure.</summary>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>Creates a missing-resource error.</summary>
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>Creates a state-conflict error.</summary>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>Creates an authentication-required error.</summary>
    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    /// <summary>Creates an insufficient-capability error.</summary>
    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    /// <summary>Creates a temporary-unavailability error.</summary>
    public static Error Unavailable(string code, string description) =>
        new(code, description, ErrorType.Unavailable);

    private static bool IsWellFormedCode(string code)
    {
        if (code.Length is 0 or > MaximumCodeLength || !IsLowerAsciiLetter(code[0]))
        {
            return false;
        }

        var previousWasSeparator = false;

        for (var index = 1; index < code.Length; index++)
        {
            var character = code[index];

            if (IsLowerAsciiLetter(character) || IsAsciiDigit(character))
            {
                previousWasSeparator = false;
                continue;
            }

            if (!IsSeparator(character) || previousWasSeparator || index == code.Length - 1)
            {
                return false;
            }

            previousWasSeparator = true;
        }

        return true;
    }

    private static bool IsLowerAsciiLetter(char character) => character is >= 'a' and <= 'z';

    private static bool IsAsciiDigit(char character) => character is >= '0' and <= '9';

    private static bool IsSeparator(char character) => character is '.' or '-' or '_';
}
