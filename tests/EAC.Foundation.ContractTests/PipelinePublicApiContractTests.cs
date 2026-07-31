using System.Reflection;
using EAC.Foundation.Application.Pipeline;
using EAC.Foundation.Application.Requests;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class PipelinePublicApiContractTests
{
    [Fact(DisplayName = "Keeps the approved UseCase continuation contract")]
    [Trait("Rule", "EAC-CONF-APP-011")]
    public void UseCaseContinuationMatchesApprovedContract()
    {
        var delegateType = typeof(UseCaseContinuation<>);
        var useCaseResponseType = Assert.Single(delegateType.GetGenericArguments());
        var invokeMethod = delegateType.GetMethod(nameof(Action.Invoke));

        Assert.True(typeof(MulticastDelegate).IsAssignableFrom(delegateType));
        Assert.Equal("TUseCaseResponse", useCaseResponseType.Name);
        Assert.NotNull(invokeMethod);
        Assert.Equal(typeof(Task<>), invokeMethod.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(invokeMethod.ReturnType.GetGenericArguments()));

        var cancellationToken = Assert.Single(invokeMethod.GetParameters());
        Assert.Equal(typeof(CancellationToken), cancellationToken.ParameterType);
        Assert.Equal("cancellationToken", cancellationToken.Name);
        Assert.False(cancellationToken.IsOptional);
    }

    [Fact(DisplayName = "Keeps the approved Pipeline Behavior contract")]
    [Trait("Rule", "EAC-CONF-APP-011")]
    public void PipelineBehaviorMatchesApprovedContract()
    {
        var behaviorType = typeof(IPipelineBehavior<,>);
        var genericArguments = behaviorType.GetGenericArguments();
        var useCaseRequestType = genericArguments[0];
        var useCaseResponseType = genericArguments[1];
        var requestConstraint = Assert.Single(useCaseRequestType.GetGenericParameterConstraints());
        var executeMethod = behaviorType.GetMethod(
            nameof(IPipelineBehavior<IUseCaseRequest<object>, object>.ExecuteAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        Assert.True(behaviorType.IsInterface);
        Assert.Equal("TUseCaseRequest", useCaseRequestType.Name);
        Assert.Equal("TUseCaseResponse", useCaseResponseType.Name);
        Assert.Equal(typeof(IUseCaseRequest<>), requestConstraint.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(requestConstraint.GetGenericArguments()));
        Assert.NotNull(executeMethod);
        Assert.Equal(typeof(Task<>), executeMethod.ReturnType.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(executeMethod.ReturnType.GetGenericArguments()));

        var parameters = executeMethod.GetParameters();
        Assert.Equal(3, parameters.Length);
        Assert.Equal(useCaseRequestType, parameters[0].ParameterType);
        Assert.Equal("useCaseRequest", parameters[0].Name);
        Assert.Equal(typeof(UseCaseContinuation<>), parameters[1].ParameterType.GetGenericTypeDefinition());
        Assert.Equal(useCaseResponseType, Assert.Single(parameters[1].ParameterType.GetGenericArguments()));
        Assert.Equal("continuation", parameters[1].Name);
        Assert.Equal(typeof(CancellationToken), parameters[2].ParameterType);
        Assert.True(parameters[2].IsOptional);
    }
}
