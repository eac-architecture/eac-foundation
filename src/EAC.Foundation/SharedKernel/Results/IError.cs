namespace EAC.Foundation.SharedKernel.Results;

/// <summary>
/// Describes an expected failure that can cross application boundaries safely.
/// </summary>
public interface IError
{
    /// <summary>Gets the stable, non-localized error code.</summary>
    string Code { get; }

    /// <summary>Gets a consumer-safe description.</summary>
    string Description { get; }

    /// <summary>Gets the protocol-neutral error classification.</summary>
    ErrorType Type { get; }
}
