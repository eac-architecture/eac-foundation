using EAC.Foundation.Domain;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class EntityTests
{
    [Fact(DisplayName = "Treats the same transient entity reference as equal")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void SameReferenceIsEqualEvenWhenEntityIsTransient()
    {
        var entity = new MaterializedEntity<Guid>();
        var sameEntity = entity;

        Assert.True(entity.Equals(sameEntity));
        Assert.True(entity == sameEntity);
        Assert.False(entity != sameEntity);
    }

    [Fact(DisplayName = "Treats persistent entities with the same type and identifier as equal")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void PersistentEntitiesOfSameConcreteTypeAndIdentifierAreEqual()
    {
        var id = Guid.Parse("d377bfcf-7194-43bb-b26b-e0da3c04244f");
        var left = new CustomerEntity(id);
        var right = new CustomerEntity(id);

        Assert.Equal(left, right);
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact(DisplayName = "Distinguishes persistent entities with different identifiers")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void PersistentEntitiesWithDifferentIdentifiersAreNotEqual()
    {
        var left = new CustomerEntity(Guid.Parse("714d869a-741a-4ac6-9dc7-94713a644ca8"));
        var right = new CustomerEntity(Guid.Parse("a58ad6df-fe16-492e-8ff9-168397380539"));

        Assert.NotEqual(left, right);
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact(DisplayName = "Distinguishes entity types even when identifiers match")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void EntitiesOfDifferentConcreteTypesAreNotEqualEvenWithSameIdentifier()
    {
        var id = Guid.Parse("ac0f4093-90fd-49e4-95b8-6e800f15a21d");
        Entity<Guid> customer = new CustomerEntity(id);
        Entity<Guid> policy = new PolicyEntity(id);

        Assert.NotEqual(customer, policy);
        Assert.False(customer == policy);
    }

    [Fact(DisplayName = "Distinguishes separate transient entities")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void DistinctTransientEntitiesAreNotEqual()
    {
        var left = new MaterializedEntity<Guid>();
        var right = new MaterializedEntity<Guid>();

        Assert.True(left.IsTransient);
        Assert.True(right.IsTransient);
        Assert.NotEqual(left, right);
        Assert.False(left == right);
    }

    [Fact(DisplayName = "Keeps Entity equality operators null-safe")]
    [Trait("Rule", "EAC-CONF-DOM-002")]
    public void NullOperatorsAreConsistent()
    {
        CustomerEntity? left = null;
        CustomerEntity? right = null;
        var entity = new CustomerEntity(Guid.Parse("7056d0d0-317d-45f9-8bfc-605487cf0c9b"));

        Assert.True(left == right);
        Assert.False(left != right);
        Assert.False(entity == null);
        Assert.True(entity != null);
    }

    [Fact(DisplayName = "Allows materializers to assign an identifier through the protected setter")]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void MaterializerCanAssignIdentifierThroughProtectedSetter()
    {
        var entity = new MaterializedEntity<Guid>();
        var id = Guid.Parse("b862b24b-815a-4c08-aaec-0bbb2c725387");

        entity.Materialize(id);

        Assert.False(entity.IsTransient);
        Assert.Equal(id, entity.Id);
    }

    [Fact(DisplayName = "Rejects a null reference identifier")]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void NullReferenceIdentifierIsRejectedByIdentifierConstructor()
    {
        Assert.Throws<ArgumentNullException>(() => new StringEntity(null!));
    }

    [Theory(DisplayName = "Uses the default integer identifier to determine transience")]
    [InlineData(0, true)]
    [InlineData(42, false)]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void IntegerIdentifierUsesItsDefaultValueToDetermineTransience(int id, bool expected)
    {
        var entity = new IntegerEntity(id);

        Assert.Equal(expected, entity.IsTransient);
    }

    [Fact(DisplayName = "Uses the default string identifier to determine transience")]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void StringIdentifierUsesItsDefaultValueToDetermineTransience()
    {
        var transient = new MaterializedEntity<string>();
        var persistent = new StringEntity("customer-100");

        Assert.True(transient.IsTransient);
        Assert.False(persistent.IsTransient);
    }

    [Fact(DisplayName = "Uses the default strongly typed identifier to determine transience")]
    [Trait("Rule", "EAC-CONF-DOM-001")]
    public void StrongIdentifierUsesItsDefaultValueToDetermineTransience()
    {
        var transient = new StrongIdentifierEntity(default);
        var persistent = new StrongIdentifierEntity(
            new CustomerId(Guid.Parse("fb684c09-93b5-4b96-9385-f33243c20ff7")));

        Assert.True(transient.IsTransient);
        Assert.False(persistent.IsTransient);
    }

    private sealed class CustomerEntity(Guid id) : Entity<Guid>(id);

    private sealed class PolicyEntity(Guid id) : Entity<Guid>(id);

    private sealed class IntegerEntity(int id) : Entity<int>(id);

    private sealed class StringEntity(string id) : Entity<string>(id);

    private sealed class StrongIdentifierEntity(CustomerId id) : Entity<CustomerId>(id);

    private sealed class MaterializedEntity<TId> : Entity<TId>
        where TId : notnull
    {
        public void Materialize(TId id) => Id = id;
    }

    private readonly record struct CustomerId(Guid Value);
}
