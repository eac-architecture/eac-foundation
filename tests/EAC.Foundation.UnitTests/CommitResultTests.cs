using EAC.Foundation.Application.Persistence;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class CommitResultTests
{
    [Fact(DisplayName = "Preserves the non-negative affected entry count")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void ConstructorPreservesAffectedEntries()
    {
        var result = new CommitResult(3);

        Assert.Equal(3, result.AffectedEntries);
    }

    [Theory(DisplayName = "Rejects a negative affected entry count")]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void ConstructorRejectsNegativeAffectedEntries(int affectedEntries)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new CommitResult(affectedEntries));

        Assert.Equal("affectedEntries", exception.ParamName);
    }

    [Fact(DisplayName = "Uses structural equality for commit results")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void EqualCommitResultsUseStructuralEquality()
    {
        Assert.Equal(new CommitResult(3), new CommitResult(3));
    }
}
