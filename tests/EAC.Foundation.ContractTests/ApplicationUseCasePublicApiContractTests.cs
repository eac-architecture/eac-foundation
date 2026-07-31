using System.Reflection;
using EAC.Foundation.Application.Commands;
using EAC.Foundation.Application.Queries;
using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class ApplicationUseCasePublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved base UseCase contract")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void BaseUseCaseMatchesApprovedContract()
    {
        var useCaseType = typeof(IUseCase<,>);
        var genericArguments = useCaseType.GetGenericArguments();
        var useCaseRequestType = genericArguments[0];
        var useCaseResponseType = genericArguments[1];
        var requestConstraint = Assert.Single(useCaseRequestType.GetGenericParameterConstraints());
        var executeMethod = useCaseType.GetMethod(
            nameof(IUseCase<IUseCaseRequest<object>, object>.ExecuteAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        Assert.True(useCaseType.IsInterface);
        Assert.Equal("TUseCaseRequest", useCaseRequestType.Name);
        Assert.Equal("TUseCaseResponse", useCaseResponseType.Name);
        Assert.Equal(GenericParameterAttributes.None, useCaseRequestType.GenericParameterAttributes);
        Assert.Equal(GenericParameterAttributes.None, useCaseResponseType.GenericParameterAttributes);
        Assert.Equal(typeof(IUseCaseRequest<>), requestConstraint.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(requestConstraint.GetGenericArguments()));
        Assert.NotNull(executeMethod);
        Assert.Equal(typeof(Task<>), executeMethod.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(executeMethod.ReturnType.GetGenericArguments()));

        var parameters = executeMethod.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(useCaseRequestType, parameters[0].ParameterType);
        Assert.Equal("useCaseRequest", parameters[0].Name);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
        Assert.True(parameters[1].IsOptional);
    }

    [Fact(DisplayName = "Keeps the approved Command UseCase contracts")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void CommandUseCasesMatchApprovedContract()
    {
        AssertSpecializedUseCase(
            typeof(ICommandUseCase<>),
            typeof(ICommand),
            typeof(Result));
        AssertGenericSpecializedUseCase(
            typeof(ICommandUseCase<,>),
            typeof(ICommand<>));
    }

    [Fact(DisplayName = "Keeps the approved Query UseCase contract")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void QueryUseCaseMatchesApprovedContract()
    {
        AssertGenericSpecializedUseCase(
            typeof(IQueryUseCase<,>),
            typeof(IQuery<>));
    }

    private static void AssertSpecializedUseCase(
        Type useCaseType,
        Type requestConstraintType,
        Type responseType)
    {
        var requestType = Assert.Single(useCaseType.GetGenericArguments());
        var requestConstraint = Assert.Single(requestType.GetGenericParameterConstraints());
        var inheritedUseCase = Assert.Single(useCaseType.GetInterfaces());
        var inheritedArguments = inheritedUseCase.GetGenericArguments();

        Assert.Empty(useCaseType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        Assert.Equal(requestConstraintType, requestConstraint);
        Assert.Equal(typeof(IUseCase<,>), inheritedUseCase.GetGenericTypeDefinition());
        Assert.Equal(requestType, inheritedArguments[0]);
        Assert.Equal(responseType, inheritedArguments[1]);
    }

    private static void AssertGenericSpecializedUseCase(
        Type useCaseType,
        Type requestConstraintDefinition)
    {
        var genericArguments = useCaseType.GetGenericArguments();
        var requestType = genericArguments[0];
        var valueType = genericArguments[1];
        var requestConstraint = Assert.Single(requestType.GetGenericParameterConstraints());
        var inheritedUseCase = Assert.Single(useCaseType.GetInterfaces());
        var inheritedArguments = inheritedUseCase.GetGenericArguments();
        var responseType = inheritedArguments[1];

        Assert.Empty(useCaseType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        Assert.Equal(requestConstraintDefinition, requestConstraint.GetGenericTypeDefinition());
        Assert.Equal(valueType, Assert.Single(requestConstraint.GetGenericArguments()));
        Assert.Equal(typeof(IUseCase<,>), inheritedUseCase.GetGenericTypeDefinition());
        Assert.Equal(requestType, inheritedArguments[0]);
        Assert.Equal(typeof(Result<>), responseType.GetGenericTypeDefinition());
        Assert.Equal(valueType, Assert.Single(responseType.GetGenericArguments()));
    }
}
