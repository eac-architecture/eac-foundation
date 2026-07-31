using EAC.Foundation.Application.Pagination;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class PageRequestTests
{
    [Fact(DisplayName = "Creates a one-based page request with a positive size")]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorPreservesValidValues()
    {
        var request = new PageRequest(2, 50);

        Assert.Equal(2, request.Number);
        Assert.Equal(50, request.Size);
    }

    [Fact(DisplayName = "Uses structural equality for page requests")]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void EqualPageRequestsUseStructuralEquality()
    {
        var first = new PageRequest(2, 50);
        var second = new PageRequest(2, 50);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact(DisplayName = "Leaves the maximum page size to the consuming application")]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorDoesNotImposeAnApplicationSpecificMaximum()
    {
        var request = new PageRequest(int.MaxValue, int.MaxValue);

        Assert.Equal(int.MaxValue, request.Number);
        Assert.Equal(int.MaxValue, request.Size);
    }

    [Theory(DisplayName = "Rejects a page number below one")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorRejectsPageNumberBelowOne(int number)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(number, 25));

        Assert.Equal("number", exception.ParamName);
    }

    [Theory(DisplayName = "Rejects a page size below one")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void ConstructorRejectsPageSizeBelowOne(int size)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(1, size));

        Assert.Equal("size", exception.ParamName);
    }
}
