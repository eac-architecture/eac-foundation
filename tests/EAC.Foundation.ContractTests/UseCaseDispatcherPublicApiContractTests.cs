using System.Reflection;
using EAC.Foundation.Application.Dispatching;
using EAC.Foundation.Application.Requests;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class UseCaseDispatcherPublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved local UseCase dispatcher contract")]
    [Trait("Rule", "EAC-CONF-APP-010")]
    public void UseCaseDispatcherMatchesApprovedContract()
    {
        var dispatcherType = typeof(IUseCaseDispatcher);
        var dispatchMethod = dispatcherType.GetMethod(
            nameof(IUseCaseDispatcher.DispatchAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        Assert.True(dispatcherType.IsInterface);
        Assert.NotNull(dispatchMethod);
        Assert.True(dispatchMethod.IsGenericMethodDefinition);

        var useCaseResponseType = Assert.Single(dispatchMethod.GetGenericArguments());
        Assert.Equal("TUseCaseResponse", useCaseResponseType.Name);
        Assert.Equal(typeof(Task<>), dispatchMethod.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(dispatchMethod.ReturnType.GetGenericArguments()));

        var parameters = dispatchMethod.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(IUseCaseRequest<>), parameters[0].ParameterType.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(parameters[0].ParameterType.GetGenericArguments()));
        Assert.Equal("useCaseRequest", parameters[0].Name);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
        Assert.True(parameters[1].IsOptional);
    }
}
