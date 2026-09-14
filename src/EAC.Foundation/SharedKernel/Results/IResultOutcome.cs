namespace EAC.Foundation.SharedKernel.Results;

/// <summary>Exposes the common success state of value and non-value results.</summary>
public interface IResultOutcome
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    bool IsFailure { get; }

}
