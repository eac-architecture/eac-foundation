using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.UnitTests;

public sealed class ErrorTests
{
    [Theory(DisplayName = "Preserves every valid error classification")]
    [InlineData(ErrorType.Failure)]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Unauthorized)]
    [InlineData(ErrorType.Forbidden)]
    [InlineData(ErrorType.Unavailable)]
    [Trait("Rule", "EAC-CONF-FOUND-001")]
    public void ConstructorPreservesValidValues(ErrorType type)
    {
        var error = new Error("document.invalid-state_2", "Safe description.", type);

        Assert.Equal("document.invalid-state_2", error.Code);
        Assert.Equal("Safe description.", error.Description);
        Assert.Equal(type, error.Type);
    }

    [Theory(DisplayName = "Creates the expected error classification through factories")]
    [InlineData(ErrorType.Failure)]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Unauthorized)]
    [InlineData(ErrorType.Forbidden)]
    [InlineData(ErrorType.Unavailable)]
    [Trait("Rule", "EAC-CONF-FOUND-001")]
    public void FactoryCreatesTheExpectedClassification(ErrorType type)
    {
        var error = CreateWithFactory(type);

        Assert.Equal("document.invalid-state", error.Code);
        Assert.Equal("Safe description.", error.Description);
        Assert.Equal(type, error.Type);
    }

    [Fact(DisplayName = "Uses structural equality for errors")]
    [Trait("Rule", "EAC-CONF-FOUND-001")]
    public void EqualErrorsUseStructuralEquality()
    {
        var first = Error.Conflict("document.invalid-state", "Safe description.");
        var second = Error.Conflict("document.invalid-state", "Safe description.");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact(DisplayName = "Rejects a null error code")]
    [Trait("Rule", "EAC-CONF-FOUND-002")]
    public void ConstructorRejectsNullCode()
    {
        Assert.Throws<ArgumentNullException>(() => new Error(null!, "Safe description.", ErrorType.Failure));
    }

    [Theory(DisplayName = "Rejects malformed error codes")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Document.invalid")]
    [InlineData("1document.invalid")]
    [InlineData(".document")]
    [InlineData("document.")]
    [InlineData("document..invalid")]
    [InlineData("document.-invalid")]
    [InlineData("document invalid")]
    [InlineData("document/invalid")]
    [InlineData("document.áccent")]
    [Trait("Rule", "EAC-CONF-FOUND-002")]
    public void ConstructorRejectsMalformedCode(string code)
    {
        Assert.Throws<ArgumentException>(() => new Error(code, "Safe description.", ErrorType.Failure));
    }

    [Fact(DisplayName = "Rejects error codes longer than 128 characters")]
    [Trait("Rule", "EAC-CONF-FOUND-002")]
    public void ConstructorRejectsCodeLongerThan128Characters()
    {
        var code = $"a{new string('b', 128)}";

        Assert.Throws<ArgumentException>(() => new Error(code, "Safe description.", ErrorType.Failure));
    }

    [Fact(DisplayName = "Rejects a null error description")]
    [Trait("Rule", "EAC-CONF-FOUND-002")]
    public void ConstructorRejectsNullDescription()
    {
        Assert.Throws<ArgumentNullException>(() => new Error("document.invalid", null!, ErrorType.Failure));
    }

    [Theory(DisplayName = "Rejects empty error descriptions")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [Trait("Rule", "EAC-CONF-FOUND-002")]
    public void ConstructorRejectsEmptyDescription(string description)
    {
        Assert.Throws<ArgumentException>(() => new Error("document.invalid", description, ErrorType.Failure));
    }

    [Fact(DisplayName = "Rejects an unknown error classification")]
    [Trait("Rule", "EAC-CONF-FOUND-002")]
    public void ConstructorRejectsUnknownClassification()
    {
        Assert.Throws<ArgumentException>(
            () => new Error("document.invalid", "Safe description.", (ErrorType)int.MaxValue));
    }

    private static Error CreateWithFactory(ErrorType type) => type switch
    {
        ErrorType.Failure => Error.Failure("document.invalid-state", "Safe description."),
        ErrorType.Validation => Error.Validation("document.invalid-state", "Safe description."),
        ErrorType.NotFound => Error.NotFound("document.invalid-state", "Safe description."),
        ErrorType.Conflict => Error.Conflict("document.invalid-state", "Safe description."),
        ErrorType.Unauthorized => Error.Unauthorized("document.invalid-state", "Safe description."),
        ErrorType.Forbidden => Error.Forbidden("document.invalid-state", "Safe description."),
        ErrorType.Unavailable => Error.Unavailable("document.invalid-state", "Safe description."),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
