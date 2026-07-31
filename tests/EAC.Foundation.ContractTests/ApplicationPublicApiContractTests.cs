using System.Reflection;
using EAC.Foundation.Application.Commands;
using EAC.Foundation.Application.Queries;
using EAC.Foundation.Application.Requests;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class ApplicationPublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved UseCase request hierarchy")]
    [Trait("Rule", "EAC-CONF-APP-001")]
    public void UseCaseRequestHierarchyMatchesApprovedContract()
    {
        var useCaseRequestType = typeof(IUseCaseRequest<>);

        AssertMarkerInterface(useCaseRequestType);
        AssertMarkerInterface(typeof(ICommand));
        AssertMarkerInterface(typeof(ICommand<>));
        AssertMarkerInterface(typeof(IQuery<>));
        Assert.Equal("TUseCaseResponse", Assert.Single(useCaseRequestType.GetGenericArguments()).Name);

        Assert.Contains(typeof(IUseCaseRequest<Result>), typeof(ICommand).GetInterfaces());
        AssertGenericResultRequest(typeof(ICommand<>));
        AssertGenericResultRequest(typeof(IQuery<>));
    }

    private static void AssertMarkerInterface(Type type)
    {
        Assert.True(type.IsInterface);
        Assert.Empty(type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
    }

    private static void AssertGenericResultRequest(Type requestType)
    {
        var valueType = Assert.Single(requestType.GetGenericArguments());
        var inheritedUseCaseRequest = Assert.Single(requestType.GetInterfaces());
        var useCaseResponseType = Assert.Single(inheritedUseCaseRequest.GetGenericArguments());

        Assert.Equal(typeof(IUseCaseRequest<>), inheritedUseCaseRequest.GetGenericTypeDefinition());
        Assert.Equal(typeof(Result<>), useCaseResponseType.GetGenericTypeDefinition());
        Assert.Equal(valueType, Assert.Single(useCaseResponseType.GetGenericArguments()));
    }
}
