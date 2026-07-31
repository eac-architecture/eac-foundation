using EAC.Foundation.Domain;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ValueObjectTests
{
    [Fact(DisplayName = "Treats equal ordered components of the same type as equal")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void SameConcreteTypeAndOrderedComponentsAreEqual()
    {
        var left = new Money("EUR", 125.50m);
        var right = new Money("EUR", 125.50m);

        Assert.Equal(left, right);
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact(DisplayName = "Distinguishes a different component value")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void DifferentComponentValueIsNotEqual()
    {
        var left = new Money("EUR", 125.50m);
        var right = new Money("EUR", 200m);

        Assert.NotEqual(left, right);
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact(DisplayName = "Includes component order in equality")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void ComponentOrderParticipatesInEquality()
    {
        var left = new OrderedPair("first", "second");
        var right = new OrderedPair("second", "first");

        Assert.NotEqual(left, right);
    }

    [Fact(DisplayName = "Distinguishes value object types even when components match")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void DifferentConcreteTypesAreNotEqualWithSameComponents()
    {
        ValueObject money = new Money("EUR", 125.50m);
        ValueObject price = new Price("EUR", 125.50m);

        Assert.NotEqual(money, price);
        Assert.False(money == price);
    }

    [Fact(DisplayName = "Includes null components in equality")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void NullComponentsParticipateInEquality()
    {
        var left = new OptionalCode(null);
        var right = new OptionalCode(null);
        var different = new OptionalCode("code-100");

        Assert.Equal(left, right);
        Assert.NotEqual(left, different);
    }

    [Fact(DisplayName = "Keeps ValueObject equality operators null-safe")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void NullOperatorsAreConsistent()
    {
        Money? left = null;
        Money? right = null;
        var value = new Money("EUR", 125.50m);

        Assert.True(left == right);
        Assert.False(left != right);
        Assert.False(value == null);
        Assert.True(value != null);
    }

    [Fact(DisplayName = "Treats the same reference as equal without enumerating components")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void SameReferenceIsEqualWithoutEnumeratingComponents()
    {
        var value = new ThrowingComponents();
        var sameValue = value;

        Assert.True(value.Equals(sameValue));
        Assert.True(value == sameValue);
    }

    [Fact(DisplayName = "Produces a stable hash code for immutable components")]
    [Trait("Rule", "EAC-CONF-DOM-005")]
    public void HashCodeIsStableForImmutableComponents()
    {
        var value = new Money("EUR", 125.50m);

        var first = value.GetHashCode();
        var second = value.GetHashCode();

        Assert.Equal(first, second);
    }

    private sealed class Money(string currency, decimal amount) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return currency;
            yield return amount;
        }
    }

    private sealed class Price(string currency, decimal amount) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return currency;
            yield return amount;
        }
    }

    private sealed class OrderedPair(string first, string second) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return first;
            yield return second;
        }
    }

    private sealed class OptionalCode(string? value) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return value;
        }
    }

    private sealed class ThrowingComponents : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents() =>
            throw new InvalidOperationException("Components should not be evaluated for the same reference.");
    }
}
