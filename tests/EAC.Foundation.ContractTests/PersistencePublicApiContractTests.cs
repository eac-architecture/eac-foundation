using System.Reflection;
using System.Runtime.CompilerServices;
using EAC.Foundation.Application.Persistence;
using EAC.Foundation.Domain;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class PersistencePublicApiContractTests
{
    [Fact(DisplayName = "Keeps Repository restricted to identified Aggregate Roots")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void RepositoryMatchesApprovedContract()
    {
        var type = typeof(IRepository<,>);
        var genericArguments = type.GetGenericArguments();
        var aggregateType = genericArguments[0];
        var idType = genericArguments[1];
        var aggregateConstraints = aggregateType.GetGenericParameterConstraints();
        var methods = type.GetMethods().ToDictionary(method => method.Name, StringComparer.Ordinal);

        Assert.True(type.IsInterface);
        Assert.Equal("TAggregate", aggregateType.Name);
        Assert.Equal("TId", idType.Name);
        Assert.True(
            (aggregateType.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0);
        Assert.Equal(
            GenericParameterAttributes.Contravariant,
            idType.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        Assert.Contains(typeof(IAggregateRoot), aggregateConstraints);
        var entityConstraint = Assert.Single(
            aggregateConstraints,
            constraint => constraint.IsGenericType &&
                          constraint.GetGenericTypeDefinition() == typeof(IEntity<>));
        Assert.Equal(idType, Assert.Single(entityConstraint.GetGenericArguments()));
        Assert.Equal(["AddAsync", "FindAsync", "Remove"], methods.Keys.Order(StringComparer.Ordinal));

        AssertRepositoryFindMethod(methods["FindAsync"], aggregateType, idType);
        AssertRepositoryAddMethod(methods["AddAsync"], aggregateType);
        AssertRepositoryRemoveMethod(methods["Remove"], aggregateType);
    }

    [Fact(DisplayName = "Keeps the approved Unit of Work contract")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void UnitOfWorkMatchesApprovedContract()
    {
        var type = typeof(IUnitOfWork);
        var method = Assert.Single(type.GetMethods());
        var cancellationToken = Assert.Single(method.GetParameters());

        Assert.True(type.IsInterface);
        Assert.Equal(nameof(IUnitOfWork.CommitAsync), method.Name);
        Assert.Equal(typeof(Task<CommitResult>), method.ReturnType);
        Assert.Equal(typeof(CancellationToken), cancellationToken.ParameterType);
        Assert.Equal("cancellationToken", cancellationToken.Name);
        Assert.True(cancellationToken.IsOptional);
    }

    [Fact(DisplayName = "Keeps the approved Commit Result contract")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void CommitResultMatchesApprovedContract()
    {
        var type = typeof(CommitResult);
        var constructor = Assert.Single(type.GetConstructors());
        var property = Assert.Single(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        Assert.True(type.IsValueType);
        Assert.True(type.IsDefined(typeof(IsReadOnlyAttribute), inherit: false));
        Assert.Equal("affectedEntries", Assert.Single(constructor.GetParameters()).Name);
        Assert.Equal(typeof(int), Assert.Single(constructor.GetParameters()).ParameterType);
        Assert.Equal(nameof(CommitResult.AffectedEntries), property.Name);
        Assert.Equal(typeof(int), property.PropertyType);
        Assert.False(property.CanWrite);
    }

    [Fact(DisplayName = "Keeps Query Service separate from Aggregate Repository")]
    [Trait("Rule", "EAC-CONF-APP-012")]
    public void QueryServiceMatchesApprovedContract()
    {
        var type = typeof(IQueryService<,>);
        var genericArguments = type.GetGenericArguments();
        var readModelType = genericArguments[0];
        var idType = genericArguments[1];
        var methods = type.GetMethods().ToDictionary(method => method.Name, StringComparer.Ordinal);

        Assert.True(type.IsInterface);
        Assert.Equal("TReadModel", readModelType.Name);
        Assert.Equal("TId", idType.Name);
        Assert.Equal(
            GenericParameterAttributes.Contravariant,
            idType.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        Assert.Empty(readModelType.GetGenericParameterConstraints());
        Assert.Equal(["ExistsAsync", "FindAsync"], methods.Keys.Order(StringComparer.Ordinal));

        AssertQueryFindMethod(methods["FindAsync"], readModelType, idType);
        AssertQueryExistsMethod(methods["ExistsAsync"], idType);
    }

    private static void AssertRepositoryFindMethod(MethodInfo method, Type aggregateType, Type idType)
    {
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(aggregateType, Assert.Single(method.ReturnType.GetGenericArguments()));
        AssertCommonIdentityAndCancellationParameters(method, idType);
    }

    private static void AssertRepositoryAddMethod(MethodInfo method, Type aggregateType)
    {
        Assert.Equal(typeof(ValueTask), method.ReturnType);
        var parameters = method.GetParameters();
        Assert.Equal(aggregateType, parameters[0].ParameterType);
        Assert.Equal("aggregate", parameters[0].Name);
        AssertCancellationParameter(parameters[1]);
    }

    private static void AssertRepositoryRemoveMethod(MethodInfo method, Type aggregateType)
    {
        Assert.Equal(typeof(void), method.ReturnType);
        var aggregate = Assert.Single(method.GetParameters());
        Assert.Equal(aggregateType, aggregate.ParameterType);
        Assert.Equal("aggregate", aggregate.Name);
    }

    private static void AssertQueryFindMethod(MethodInfo method, Type readModelType, Type idType)
    {
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(readModelType, Assert.Single(method.ReturnType.GetGenericArguments()));
        AssertCommonIdentityAndCancellationParameters(method, idType);
    }

    private static void AssertQueryExistsMethod(MethodInfo method, Type idType)
    {
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
        AssertCommonIdentityAndCancellationParameters(method, idType);
    }

    private static void AssertCommonIdentityAndCancellationParameters(MethodInfo method, Type idType)
    {
        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(idType, parameters[0].ParameterType);
        Assert.Equal("id", parameters[0].Name);
        AssertCancellationParameter(parameters[1]);
    }

    private static void AssertCancellationParameter(ParameterInfo parameter)
    {
        Assert.Equal(typeof(CancellationToken), parameter.ParameterType);
        Assert.Equal("cancellationToken", parameter.Name);
        Assert.True(parameter.IsOptional);
    }
}
