using System.Reflection;
using System.Runtime.Versioning;
using Xunit;

namespace EAC.Foundation.ArchitectureTests;

public sealed class AssemblyBoundaryTests
{
    [Fact(DisplayName = "Uses the approved assembly identity")]
    [Trait("Rule", "EAC-CONF-FOUND-007")]
    public void AssemblyHasTheExpectedIdentity()
    {
        var assembly = LoadFoundationAssembly();

        Assert.Equal("EAC.Foundation", assembly.GetName().Name);
    }

    [Fact(DisplayName = "Targets .NET 10")]
    [Trait("Rule", "EAC-CONF-FOUND-007")]
    public void AssemblyTargetsNet10()
    {
        var assembly = LoadFoundationAssembly();
        var targetFramework = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

        Assert.NotNull(targetFramework);
        Assert.Equal(".NETCoreApp,Version=v10.0", targetFramework.FrameworkName);
    }

    [Fact(DisplayName = "References only the .NET base class library")]
    [Trait("Rule", "EAC-CONF-FOUND-007")]
    public void AssemblyReferencesOnlyTheBaseClassLibrary()
    {
        var assembly = LoadFoundationAssembly();
        var externalReferences = assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null &&
                           !name.Equals("System", StringComparison.Ordinal) &&
                           !name.StartsWith("System.", StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(externalReferences);
    }

    [Fact(DisplayName = "Exports only approved Foundation namespaces")]
    [Trait("Rule", "EAC-CONF-FOUND-007")]
    public void AssemblyExportsOnlyApprovedFoundationNamespaces()
    {
        var assembly = LoadFoundationAssembly();
        var unexpectedTypes = assembly
            .GetExportedTypes()
            .Where(type => type.Namespace is null ||
                           (!type.Namespace.StartsWith("EAC.Foundation.Application", StringComparison.Ordinal) &&
                            !type.Namespace.StartsWith("EAC.Foundation.SharedKernel", StringComparison.Ordinal) &&
                            !type.Namespace.Equals("EAC.Foundation.Domain", StringComparison.Ordinal)))
            .Select(type => type.FullName)
            .ToArray();

        Assert.Empty(unexpectedTypes);
    }

    private static Assembly LoadFoundationAssembly() =>
        Assembly.Load(new AssemblyName("EAC.Foundation"));
}
