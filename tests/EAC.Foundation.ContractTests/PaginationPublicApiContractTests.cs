using System.Reflection;
using EAC.Foundation.Application.Pagination;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class PaginationPublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved Page contract")]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void PageMatchesApprovedContract()
    {
        var type = typeof(Page<>);
        var itemType = Assert.Single(type.GetGenericArguments());
        var constructor = Assert.Single(type.GetConstructors());
        var parameters = constructor.GetParameters();
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ToDictionary(property => property.Name, StringComparer.Ordinal);

        Assert.True(type.IsSealed);
        Assert.Equal("TItem", itemType.Name);
        Assert.Equal(["items", "number", "size", "totalItems"], parameters.Select(parameter => parameter.Name));
        Assert.Equal(typeof(IEnumerable<>), parameters[0].ParameterType.GetGenericTypeDefinition());
        Assert.Equal(itemType, Assert.Single(parameters[0].ParameterType.GetGenericArguments()));
        Assert.Equal(typeof(int), parameters[1].ParameterType);
        Assert.Equal(typeof(int), parameters[2].ParameterType);
        Assert.Equal(typeof(long), parameters[3].ParameterType);
        Assert.Empty(type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly));
        Assert.Equal(7, properties.Count);
        Assert.Equal(typeof(IReadOnlyList<>), properties[nameof(Page<object>.Items)].PropertyType.GetGenericTypeDefinition());
        Assert.Equal(itemType, Assert.Single(properties[nameof(Page<object>.Items)].PropertyType.GetGenericArguments()));
        Assert.Equal(typeof(int), properties[nameof(Page<object>.Number)].PropertyType);
        Assert.Equal(typeof(int), properties[nameof(Page<object>.Size)].PropertyType);
        Assert.Equal(typeof(long), properties[nameof(Page<object>.TotalItems)].PropertyType);
        Assert.Equal(typeof(int), properties[nameof(Page<object>.TotalPages)].PropertyType);
        Assert.Equal(typeof(bool), properties[nameof(Page<object>.HasPrevious)].PropertyType);
        Assert.Equal(typeof(bool), properties[nameof(Page<object>.HasNext)].PropertyType);
        Assert.All(properties.Values, property => Assert.False(property.CanWrite));
    }

    [Fact(DisplayName = "Keeps the approved Page Request contract")]
    [Trait("Rule", "EAC-CONF-APP-005")]
    public void PageRequestMatchesApprovedContract()
    {
        var type = typeof(PageRequest);
        var constructor = Assert.Single(type.GetConstructors());
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(property => $"{property.Name}:{property.PropertyType.FullName}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(type.IsSealed);
        Assert.Equal(["number", "size"], constructor.GetParameters().Select(parameter => parameter.Name));
        Assert.All(constructor.GetParameters(), parameter => Assert.Equal(typeof(int), parameter.ParameterType));
        Assert.Equal(["Number:System.Int32", "Size:System.Int32"], properties);
        Assert.All(
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            property => Assert.False(property.CanWrite));
    }
}
