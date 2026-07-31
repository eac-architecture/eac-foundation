using EAC.Foundation.SharedKernel.Results;

namespace EAC.Foundation.Application.Validation;

/// <summary>
/// Represents one consumer-safe validation failure.
/// </summary>
public sealed record ValidationFailure
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationFailure"/> record.
    /// </summary>
    /// <param name="field">
    /// Field or member path associated with the failure, or an empty string for the complete request.
    /// </param>
    /// <param name="code">Stable, non-localized validation code.</param>
    /// <param name="message">Consumer-safe validation message.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="field"/>, <paramref name="code"/> or <paramref name="message"/> is
    /// <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The field contains only whitespace, the code is malformed, or the message is empty.
    /// </exception>
    public ValidationFailure(string field, string code, string message)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(message);

        if (field.Length > 0 && string.IsNullOrWhiteSpace(field))
        {
            throw new ArgumentException(
                "The validation field must be empty or contain a non-whitespace member path.",
                nameof(field));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "The validation message cannot be empty or whitespace.",
                nameof(message));
        }

        var validationError = Error.Validation(code, message);

        Field = field;
        Code = validationError.Code;
        Message = validationError.Description;
    }

    /// <summary>
    /// Gets the field or member path, or an empty string when the complete request is invalid.
    /// </summary>
    public string Field { get; }

    /// <summary>Gets the stable, non-localized validation code.</summary>
    public string Code { get; }

    /// <summary>Gets the consumer-safe validation message.</summary>
    public string Message { get; }
}
