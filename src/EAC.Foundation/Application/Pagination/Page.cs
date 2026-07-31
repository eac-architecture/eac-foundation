namespace EAC.Foundation.Application.Pagination;

/// <summary>
/// Represents an immutable page of items with exact offset-page metadata.
/// </summary>
/// <typeparam name="TItem">Type of item contained in the page.</typeparam>
public sealed class Page<TItem>
{
    /// <summary>Initializes a new instance of the <see cref="Page{TItem}"/> class.</summary>
    /// <param name="items">Items returned for the requested page.</param>
    /// <param name="number">One-based page number.</param>
    /// <param name="size">Positive number of items requested per page.</param>
    /// <param name="totalItems">Non-negative total number of matching items.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="items"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A numeric value is outside its supported range or the exact page count exceeds
    /// <see cref="int.MaxValue"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The page contains more items than its requested size or reported total.
    /// </exception>
    public Page(
        IEnumerable<TItem> items,
        int number,
        int size,
        long totalItems)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItems);

        var itemSnapshot = items.ToArray();

        if (itemSnapshot.Length > size)
        {
            throw new ArgumentException(
                "A page cannot contain more items than its requested size.",
                nameof(items));
        }

        if (itemSnapshot.LongLength > totalItems)
        {
            throw new ArgumentException(
                "A page cannot contain more items than its reported total.",
                nameof(items));
        }

        var totalPages = CalculateTotalPages(totalItems, size);

        Items = Array.AsReadOnly(itemSnapshot);
        Number = number;
        Size = size;
        TotalItems = totalItems;
        TotalPages = totalPages;
        HasPrevious = number > 1;
        HasNext = number < totalPages;
    }

    /// <summary>Gets an immutable snapshot of the page items.</summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>Gets the one-based page number.</summary>
    public int Number { get; }

    /// <summary>Gets the positive number of items requested per page.</summary>
    public int Size { get; }

    /// <summary>Gets the total number of matching items.</summary>
    public long TotalItems { get; }

    /// <summary>Gets the exact number of pages, or zero when no items match.</summary>
    public int TotalPages { get; }

    /// <summary>Gets a value indicating whether a preceding page number exists.</summary>
    public bool HasPrevious { get; }

    /// <summary>Gets a value indicating whether another page with matching items exists.</summary>
    public bool HasNext { get; }

    private static int CalculateTotalPages(long totalItems, int size)
    {
        if (totalItems == 0)
        {
            return 0;
        }

        var totalPages = ((totalItems - 1) / size) + 1;

        if (totalPages > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalItems),
                totalItems,
                "The exact number of pages exceeds the supported Int32 page-number range.");
        }

        return (int)totalPages;
    }
}
