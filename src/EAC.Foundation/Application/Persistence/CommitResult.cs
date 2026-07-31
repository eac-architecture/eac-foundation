namespace EAC.Foundation.Application.Persistence;

/// <summary>
/// Represents the non-negative number of entries accepted by a local unit of work.
/// </summary>
public readonly record struct CommitResult
{
    /// <summary>Initializes a new instance of the <see cref="CommitResult"/> record.</summary>
    /// <param name="affectedEntries">Non-negative number of accepted entries.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="affectedEntries"/> is negative.
    /// </exception>
    public CommitResult(int affectedEntries)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(affectedEntries);
        AffectedEntries = affectedEntries;
    }

    /// <summary>Gets the number of entries accepted by the local unit of work.</summary>
    public int AffectedEntries { get; }
}
