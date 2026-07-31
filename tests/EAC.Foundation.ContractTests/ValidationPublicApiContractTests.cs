using System.Reflection;
using EAC.Foundation.Application.Validation;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class ValidationPublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved UseCase request validator contract")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void UseCaseRequestValidatorMatchesApprovedContract()
    {
        var type = typeof(IUseCaseRequestValidator<>);
        var useCaseRequestType = Assert.Single(type.GetGenericArguments());
        var validateMethod = type.GetMethod(
            nameof(IUseCaseRequestValidator<object>.ValidateAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        Assert.True(type.IsInterface);
        Assert.Equal("TUseCaseRequest", useCaseRequestType.Name);
        Assert.Equal(
            GenericParameterAttributes.Contravariant,
            useCaseRequestType.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        Assert.NotNull(validateMethod);
        Assert.Equal(typeof(ValueTask<ValidationOutcome>), validateMethod.ReturnType);

        var parameters = validateMethod.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(useCaseRequestType, parameters[0].ParameterType);
        Assert.Equal("useCaseRequest", parameters[0].Name);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
        Assert.Equal("cancellationToken", parameters[1].Name);
        Assert.True(parameters[1].IsOptional);
    }

    [Fact(DisplayName = "Keeps the approved Validation Failure contract")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationFailureMatchesApprovedContract()
    {
        var type = typeof(ValidationFailure);
        var constructor = Assert.Single(type.GetConstructors());

        Assert.True(type.IsSealed);
        Assert.Equal(["field", "code", "message"], constructor.GetParameters().Select(parameter => parameter.Name));
        Assert.Equal(
            ["Code:System.String", "Field:System.String", "Message:System.String"],
            GetDeclaredPropertySnapshot(type));
    }

    [Fact(DisplayName = "Keeps the approved Validation Error contract")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationErrorMatchesApprovedContract()
    {
        var type = typeof(ValidationError);
        var constructor = Assert.Single(type.GetConstructors());
        var parameters = constructor.GetParameters();

        Assert.True(type.IsSealed);
        Assert.Contains(typeof(IError), type.GetInterfaces());
        Assert.Equal(["code", "description", "failures"], parameters.Select(parameter => parameter.Name));
        Assert.Equal(typeof(IReadOnlyCollection<ValidationFailure>), parameters[2].ParameterType);
        Assert.Equal(
            [
                "Code:System.String",
                "Description:System.String",
                $"Failures:{typeof(IReadOnlyCollection<ValidationFailure>).FullName}",
                $"Type:{typeof(ErrorType).FullName}",
            ],
            GetDeclaredPropertySnapshot(type));
    }

    [Fact(DisplayName = "Keeps the approved Validation Outcome contract")]
    [Trait("Rule", "EAC-CONF-APP-004")]
    public void ValidationOutcomeMatchesApprovedContract()
    {
        var type = typeof(ValidationOutcome);
        var methods = type
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .OrderBy(method => method.Name, StringComparer.Ordinal)
            .ToArray();

        Assert.True(type.IsSealed);
        Assert.Empty(type.GetConstructors());
        Assert.Equal(
            [
                $"Failures:{typeof(IReadOnlyCollection<ValidationFailure>).FullName}",
                "IsValid:System.Boolean",
            ],
            GetDeclaredPropertySnapshot(type));
        Assert.Equal(["Invalid", "Valid"], methods.Select(method => method.Name));
        Assert.Equal(typeof(IEnumerable<ValidationFailure>), methods[0].GetParameters().Single().ParameterType);
        Assert.Empty(methods[1].GetParameters());
        Assert.All(methods, method => Assert.Equal(type, method.ReturnType));
    }

    private static string[] GetDeclaredPropertySnapshot(Type type) => type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Select(property => $"{property.Name}:{property.PropertyType.FullName}")
        .Order(StringComparer.Ordinal)
        .ToArray();
}
