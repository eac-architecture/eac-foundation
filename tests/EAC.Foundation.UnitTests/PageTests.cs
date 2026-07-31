using EAC.Foundation.Application.Pagination;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class PageTests
{
    [Fact(DisplayName = "Creates a page with exact metadata and an immutable item snapshot")]
    [Trait("Rule", "EAC-CONF-APP-006")]
    public void ConstructorPreservesMetadataAndImmutableItems()
    {
        var source = new List<string> { "POL-0001", "POL-0002" };

        var page = new Page<string>(source, 2, 2, 5);

        source.Reverse();
        source.Add("POL-0003");

        Assert.Equal(["POL-0001", "POL-0002"], page.Items);
        Assert.False(page.Items is string[]);
        Assert.Equal(2, page.Number);
        Assert.Equal(2, page.Size);
        Assert.Equal(5, page.TotalItems);
        Assert.Equal(3, page.TotalPages);
        Assert.True(page.HasPrevious);
        Assert.True(page.HasNext);
    }

    [Theory(DisplayName = "Calculates navigation from the requested page and total")]
    [InlineData(1, false, true)]
    [InlineData(2, true, true)]
    [InlineData(3, true, false)]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorCalculatesNavigation(int number, bool hasPrevious, bool hasNext)
    {
        var page = new Page<string>([], number, 10, 25);

        Assert.Equal(3, page.TotalPages);
        Assert.Equal(hasPrevious, page.HasPrevious);
        Assert.Equal(hasNext, page.HasNext);
    }

    [Fact(DisplayName = "Creates an empty page with zero totals and no navigation")]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void EmptyCreatesAnEmptyFirstPage()
    {
        var page = new Page<string>([], 1, 25, 0);

        Assert.Empty(page.Items);
        Assert.Equal(1, page.Number);
        Assert.Equal(25, page.Size);
        Assert.Equal(0, page.TotalItems);
        Assert.Equal(0, page.TotalPages);
        Assert.False(page.HasPrevious);
        Assert.False(page.HasNext);
    }

    [Fact(DisplayName = "Supports a total item count greater than Int32 MaxValue")]
    [Trait("Rule", "EAC-CONF-APP-006")]
    public void ConstructorSupportsLongTotalItemCount()
    {
        var totalItems = (long)int.MaxValue + 1;

        var page = new Page<string>([], 1, 2, totalItems);

        Assert.Equal(totalItems, page.TotalItems);
        Assert.Equal(1_073_741_824, page.TotalPages);
        Assert.True(page.HasNext);
    }

    [Fact(DisplayName = "Rejects a total that requires more than Int32 MaxValue pages")]
    [Trait("Rule", "EAC-CONF-APP-006")]
    public void ConstructorRejectsUnrepresentableTotalPageCount()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<string>([], 1, 1, (long)int.MaxValue + 1));

        Assert.Equal("totalItems", exception.ParamName);
    }

    [Theory(DisplayName = "Rejects a page number below one")]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorRejectsPageNumberBelowOne(int number)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<string>([], number, 25, 0));

        Assert.Equal("number", exception.ParamName);
    }

    [Theory(DisplayName = "Rejects a page size below one")]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorRejectsPageSizeBelowOne(int size)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<string>([], 1, size, 0));

        Assert.Equal("size", exception.ParamName);
    }

    [Theory(DisplayName = "Rejects a negative total item count")]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorRejectsNegativeTotalItems(long totalItems)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Page<string>([], 1, 25, totalItems));

        Assert.Equal("totalItems", exception.ParamName);
    }

    [Fact(DisplayName = "Rejects a null item sequence")]
    [Trait("Rule", "EAC-CONF-APP-006")]
    public void ConstructorRejectsNullItems()
    {
        Assert.Throws<ArgumentNullException>(() => new Page<string>(null!, 1, 25, 0));
    }

    [Fact(DisplayName = "Rejects more page items than the requested size")]
    [Trait("Rule", "EAC-CONF-APP-006")]
    public void ConstructorRejectsItemsBeyondRequestedSize()
    {
        Assert.Throws<ArgumentException>(
            () => new Page<string>(["POL-0001", "POL-0002"], 1, 1, 2));
    }

    [Fact(DisplayName = "Rejects more page items than the reported total")]
    [Trait("Rule", "EAC-CONF-APP-006")]
    public void ConstructorRejectsItemsBeyondReportedTotal()
    {
        Assert.Throws<ArgumentException>(
            () => new Page<string>(["POL-0001"], 1, 25, 0));
    }
}
