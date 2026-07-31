using System.Reflection;
using EAC.Foundation.Application.Persistence;
using Xunit;

namespace EAC.Foundation.ArchitectureTests;

public sealed class PersistencePortArchitectureTests
{
    private static readonly Type[] PortTypes =
    [
        typeof(IRepository<,>),
        typeof(IUnitOfWork),
        typeof(IQueryService<,>),
        typeof(CommitResult),
    ];

    [Fact(DisplayName = "Exposes no provider, queryable, session or transport types through persistence ports")]
    [Trait("Rule", "EAC-CONF-APP-008")]
    public void PersistencePortsExposeOnlyFoundationAndBaseClassLibraryTypes()
    {
        var exposedTypes = PortTypes
            .SelectMany(GetExposedTypes)
            .Distinct()
            .ToArray();

        Assert.DoesNotContain(exposedTypes, IsQueryableType);
        Assert.DoesNotContain(
            exposedTypes,
            type => type.Namespace is not null &&
                    (type.Namespace.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) ||
                     type.Namespace.StartsWith("MongoDB", StringComparison.Ordinal) ||
                     type.Namespace.StartsWith("Marten", StringComparison.Ordinal) ||
                     type.Namespace.StartsWith("System.Net", StringComparison.Ordinal)));
    }

    private static bool IsQueryableType(Type type) =>
        type == typeof(IQueryable) ||
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>);

    private static IEnumerable<Type> GetExposedTypes(Type type)
    {
        foreach (var constraint in type
                     .GetGenericArguments()
                     .SelectMany(argument => argument.GetGenericParameterConstraints()))
        {
            foreach (var exposedType in Flatten(constraint))
            {
                yield return exposedType;
            }
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
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
    }

    private static IEnumerable<Type> Flatten(Type type)
    {
        yield return type;

        if (!type.IsGenericType)
        {
            yield break;
        }

        foreach (var argument in type.GetGenericArguments())
        {
            foreach (var nestedType in Flatten(argument))
            {
                yield return nestedType;
            }
        }
    }
}
