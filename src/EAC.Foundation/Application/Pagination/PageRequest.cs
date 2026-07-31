namespace EAC.Foundation.Application.Pagination;

/// <summary>
/// Represents an immutable one-based page request without application-specific size limits.
/// </summary>
public sealed record PageRequest
{
    /// <summary>Initializes a new instance of the <see cref="PageRequest"/> record.</summary>
    /// <param name="number">One-based page number.</param>
    /// <param name="size">Positive number of items requested per page.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="number"/> or <paramref name="size"/> is less than one.
    /// </exception>
    public PageRequest(int number, int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        Number = number;
        Size = size;
    }

    /// <summary>Gets the one-based page number.</summary>
    public int Number { get; }

    /// <summary>Gets the positive number of items requested per page.</summary>
    public int Size { get; }
}
