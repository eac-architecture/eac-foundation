using System.Reflection;
using EAC.Foundation.Application.Persistence;
using EAC.Foundation.Application.Queries;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.ArchitectureTests;

public sealed class ApplicationArchitectureTests
{
    private static readonly Assembly FoundationAssembly = typeof(Result).Assembly;

    [Fact(DisplayName = "Keeps the Query Use Case contract independent from Command Side persistence ports")]
    [Trait("Rule", "EAC-CONF-APP-007")]
    public void QueryUseCaseContractDoesNotExposeCommandSidePersistencePorts()
    {
        var exposedTypes = GetExposedTypes(typeof(IQueryUseCase<,>))
            .Select(GetGenericTypeDefinitionOrSelf)
            .Distinct()
            .ToArray();

        Assert.DoesNotContain(typeof(IRepository<,>), exposedTypes);
        Assert.DoesNotContain(typeof(IUnitOfWork), exposedTypes);
        Assert.DoesNotContain(typeof(CommitResult), exposedTypes);
    }

    [Fact(DisplayName = "Keeps the complete Application public surface provider and transport neutral")]
    [Trait("Rule", "EAC-CONF-APP-008")]
    public void ApplicationPublicSurfaceUsesOnlyApprovedNamespaces()
    {
        var unexpectedTypes = FoundationAssembly
            .GetExportedTypes()
            .Where(type => type.Namespace?.StartsWith("EAC.Foundation.Application", StringComparison.Ordinal) == true)
            .SelectMany(GetExposedTypes)
            .Where(type => !type.IsGenericParameter)
            .Where(type => !IsApprovedType(type))
            .Select(type => type.FullName ?? type.Name)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(unexpectedTypes);
    }

    private static bool IsApprovedType(Type type)
    {
        var candidate = GetGenericTypeDefinitionOrSelf(type);
        var typeNamespace = candidate.Namespace;

        return typeNamespace is not null &&
               (typeNamespace.Equals("System", StringComparison.Ordinal) ||
                typeNamespace.StartsWith("System.", StringComparison.Ordinal) ||
                typeNamespace.StartsWith("EAC.Foundation.Application", StringComparison.Ordinal) ||
                typeNamespace.Equals("EAC.Foundation.Domain", StringComparison.Ordinal) ||
                typeNamespace.StartsWith("EAC.Foundation.SharedKernel", StringComparison.Ordinal));
    }

    private static IEnumerable<Type> GetExposedTypes(Type type)
    {
        yield return type;

        if (type.BaseType is not null)
        {
            foreach (var exposedType in Flatten(type.BaseType))
            {
                yield return exposedType;
            }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            foreach (var exposedType in Flatten(interfaceType))
            {
                yield return exposedType;
            }
        }

        foreach (var constraint in type
                     .GetGenericArguments()
                     .SelectMany(argument => argument.GetGenericParameterConstraints()))
        {
            foreach (var exposedType in Flatten(constraint))
            {
                yield return exposedType;
            }
        }

        foreach (var constructor in type.GetConstructors(
                     BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            foreach (var parameter in constructor.GetParameters())
            {
                foreach (var exposedType in Flatten(parameter.ParameterType))
                {
                    yield return exposedType;
                }
            }
        }

        foreach (var method in type.GetMethods(
                     BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            foreach (var exposedType in Flatten(method.ReturnType))
            {
                yield return exposedType;
            }

            foreach (var parameter in method.GetParameters())
            {
                foreach (var exposedType in Flatten(parameter.ParameterType))
                {
                    yield return exposedType;
                }
            }
        }

        foreach (var property in type.GetProperties(
                     BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            foreach (var exposedType in Flatten(property.PropertyType))
            {
                yield return exposedType;
            }
        }
    }

    private static IEnumerable<Type> Flatten(Type type)
    {
        yield return type;

        if (type.HasElementType && type.GetElementType() is { } elementType)
        {
            foreach (var exposedType in Flatten(elementType))
            {
                yield return exposedType;
            }
        }

        if (!type.IsGenericType)
        {
            yield break;
        }

        foreach (var argument in type.GetGenericArguments())
        {
            foreach (var exposedType in Flatten(argument))
            {
                yield return exposedType;
            }
        }
    }

    private static Type GetGenericTypeDefinitionOrSelf(Type type) =>
        type.IsGenericType ? type.GetGenericTypeDefinition() : type;
}
