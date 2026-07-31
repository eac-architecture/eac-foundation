namespace EAC.Foundation.SharedKernel.Results;

/// <summary>
/// Classifies an expected error without coupling it to a transport protocol.
/// </summary>
public enum ErrorType
{
    /// <summary>Represents a general expected failure.</summary>
    Failure = 0,

    /// <summary>Represents invalid input.</summary>
    Validation = 1,

    /// <summary>Represents a missing resource.</summary>
    NotFound = 2,

    /// <summary>Represents a conflict with the current state.</summary>
    Conflict = 3,

    /// <summary>Represents a missing or invalid identity.</summary>
    Unauthorized = 4,

    /// <summary>Represents an identity without the required capability.</summary>
    Forbidden = 5,

    /// <summary>Represents a temporarily unavailable dependency or capability.</summary>
    Unavailable = 6,
}
