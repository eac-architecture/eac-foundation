using System.Reflection;
using EAC.Foundation.Domain;
using EAC.Foundation.SharedKernel.Domain;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class DomainPublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved ValueObject public contract")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void ValueObjectMatchesApprovedContract()
    {
        var valueObjectType = typeof(ValueObject);
        var equalityComponents = valueObjectType.GetMethod(
            "GetEqualityComponents",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        var typedEquals = valueObjectType.GetMethod(
            nameof(ValueObject.Equals),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            [valueObjectType]);
        var objectEquals = valueObjectType.GetMethod(
            nameof(ValueObject.Equals),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            [typeof(object)]);
        var hashCode = valueObjectType.GetMethod(
            nameof(GetHashCode),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            Type.EmptyTypes);
        var equality = valueObjectType.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
        var inequality = valueObjectType.GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);

        Assert.True(valueObjectType.IsAbstract);
        Assert.Contains(typeof(IEquatable<ValueObject>), valueObjectType.GetInterfaces());
        Assert.NotNull(equalityComponents);
        Assert.True(equalityComponents.IsFamily);
        Assert.True(equalityComponents.IsAbstract);
        Assert.Equal(typeof(IEnumerable<object>), equalityComponents.ReturnType);
        Assert.NotNull(typedEquals);
        Assert.NotNull(objectEquals);
        Assert.NotNull(hashCode);
        Assert.NotNull(equality);
        Assert.NotNull(inequality);
        Assert.Equal(typeof(bool), typedEquals.ReturnType);
        Assert.Equal(typeof(bool), objectEquals.ReturnType);
        Assert.Equal(typeof(int), hashCode.ReturnType);
        Assert.Equal(typeof(bool), equality.ReturnType);
        Assert.Equal(typeof(bool), inequality.ReturnType);
    }

    [Fact(DisplayName = "Keeps the approved aggregate interfaces")]
    [Trait("Rule", "EAC-CONF-DOM-003")]
    public void AggregateInterfacesMatchApprovedContract()
    {
        var markerType = typeof(IAggregateRoot);
        var eventsType = typeof(IHasDomainEvents);
        var eventsProperty = Assert.Single(eventsType.GetProperties());
        var dequeueMethod = eventsType.GetMethod(
            nameof(IHasDomainEvents.DequeueDomainEvents),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var expectedCollectionType = typeof(IReadOnlyCollection<IDomainEvent>);

        Assert.True(markerType.IsInterface);
        Assert.Empty(markerType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        Assert.True(eventsType.IsInterface);
        Assert.Equal(nameof(IHasDomainEvents.DomainEvents), eventsProperty.Name);
        Assert.Equal(expectedCollectionType, eventsProperty.PropertyType);
        Assert.NotNull(eventsProperty.GetMethod);
        Assert.Null(eventsProperty.SetMethod);
        Assert.NotNull(dequeueMethod);
        Assert.Equal(nameof(IHasDomainEvents.DequeueDomainEvents), dequeueMethod.Name);
        Assert.Equal(expectedCollectionType, dequeueMethod.ReturnType);
        Assert.Empty(dequeueMethod.GetParameters());
    }

    [Fact(DisplayName = "Keeps the approved AggregateRoot public contract")]
    [Trait("Rule", "EAC-CONF-DOM-003")]
    public void AggregateRootMatchesApprovedContract()
    {
        var aggregateType = typeof(AggregateRoot<>);
        var baseType = aggregateType.BaseType;
        var domainEventsProperty = aggregateType.GetProperty(nameof(IHasDomainEvents.DomainEvents));
        var dequeueMethod = aggregateType.GetMethod(
            nameof(IHasDomainEvents.DequeueDomainEvents),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var raiseMethod = aggregateType.GetMethod(
            "RaiseDomainEvent",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        var constructors = aggregateType.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        Assert.True(aggregateType.IsAbstract);
        Assert.NotNull(baseType);
        Assert.True(baseType.IsGenericType);
        Assert.Equal(typeof(Entity<>), baseType.GetGenericTypeDefinition());
        Assert.Contains(typeof(IAggregateRoot), aggregateType.GetInterfaces());
        Assert.Contains(typeof(IHasDomainEvents), aggregateType.GetInterfaces());
        Assert.NotNull(domainEventsProperty);
        Assert.Equal(typeof(IReadOnlyCollection<IDomainEvent>), domainEventsProperty.PropertyType);
        Assert.NotNull(dequeueMethod);
        Assert.Equal(typeof(IReadOnlyCollection<IDomainEvent>), dequeueMethod.ReturnType);
        Assert.NotNull(raiseMethod);
        Assert.True(raiseMethod.IsFamily);
        Assert.Equal(typeof(void), raiseMethod.ReturnType);
        Assert.Equal(typeof(IDomainEvent), Assert.Single(raiseMethod.GetParameters()).ParameterType);
        Assert.Equal(2, constructors.Length);
        Assert.All(constructors, constructor => Assert.True(constructor.IsFamily));
        Assert.Empty(aggregateType.GetConstructors(BindingFlags.Instance | BindingFlags.Public));
    }

    [Fact(DisplayName = "Keeps the approved entity identity interface")]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void EntityIdentityInterfaceMatchesApprovedContract()
    {
        var interfaceType = typeof(IEntity<>);
        var identifier = Assert.Single(interfaceType.GetGenericArguments());
        var property = Assert.Single(interfaceType.GetProperties());

        Assert.True(interfaceType.IsInterface);
        Assert.Equal(GenericParameterAttributes.Covariant, identifier.GenericParameterAttributes);
        Assert.Equal("Id", property.Name);
        Assert.Equal(identifier, property.PropertyType);
        Assert.NotNull(property.GetMethod);
        Assert.Null(property.SetMethod);
    }

    [Fact(DisplayName = "Keeps the approved Entity properties and constructors")]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void EntityPropertiesAndConstructorsMatchApprovedContract()
    {
        var entityType = typeof(Entity<>);
        var identifier = Assert.Single(entityType.GetGenericArguments());
        var idProperty = entityType.GetProperty(nameof(Entity<object>.Id));
        var transientProperty = entityType.GetProperty(nameof(Entity<object>.IsTransient));
        var constructors = entityType.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        Assert.True(entityType.IsAbstract);
        Assert.NotNull(idProperty);
        Assert.NotNull(transientProperty);
        Assert.Equal(identifier, idProperty.PropertyType);
        Assert.NotNull(idProperty.GetMethod);
        Assert.True(idProperty.GetMethod.IsPublic);
        Assert.NotNull(idProperty.SetMethod);
        Assert.True(idProperty.SetMethod.IsFamily);
        Assert.Equal(typeof(bool), transientProperty.PropertyType);
        Assert.NotNull(transientProperty.GetMethod);
        Assert.True(transientProperty.GetMethod.IsPublic);
        Assert.Null(transientProperty.SetMethod);
        Assert.Equal(2, constructors.Length);
        Assert.All(constructors, constructor => Assert.True(constructor.IsFamily));
        Assert.Contains(constructors, constructor => constructor.GetParameters().Length == 0);
        Assert.Contains(
            constructors,
            constructor => constructor.GetParameters() is [{ ParameterType: var parameterType }] &&
                           parameterType == identifier);
        Assert.Empty(entityType.GetConstructors(BindingFlags.Instance | BindingFlags.Public));
    }

    [Fact(DisplayName = "Keeps the approved Entity equality members")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void EntityEqualityMembersMatchApprovedContract()
    {
        var entityType = typeof(Entity<>);
        var typedEquals = entityType.GetMethod(
            nameof(Entity<object>.Equals),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            [entityType]);
        var objectEquals = entityType.GetMethod(
            nameof(Entity<object>.Equals),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            [typeof(object)]);
        var hashCode = entityType.GetMethod(
            nameof(GetHashCode),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            Type.EmptyTypes);
        var equality = entityType.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
        var inequality = entityType.GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);

        Assert.NotNull(typedEquals);
        Assert.NotNull(objectEquals);
        Assert.NotNull(hashCode);
        Assert.NotNull(equality);
        Assert.NotNull(inequality);
        Assert.Equal(typeof(bool), typedEquals.ReturnType);
        Assert.Equal(typeof(bool), objectEquals.ReturnType);
        Assert.Equal(typeof(int), hashCode.ReturnType);
        Assert.Equal(typeof(bool), equality.ReturnType);
        Assert.Equal(typeof(bool), inequality.ReturnType);
    }
}
