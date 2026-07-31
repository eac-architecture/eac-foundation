using System.Reflection;
using EAC.Foundation.Domain;
using EAC.Foundation.SharedKernel.Domain;
using EAC.Foundation.SharedKernel.Results;
using Xunit;

namespace EAC.Foundation.ContractTests;

public sealed class PublicApiContractTests
{
    private static readonly Assembly FoundationAssembly = typeof(Result).Assembly;

    private static readonly string[] ApprovedPublicTypes =
    [
        "EAC.Foundation.Application.Commands.ICommand",
        "EAC.Foundation.Application.Commands.ICommandUseCase`1",
        "EAC.Foundation.Application.Commands.ICommandUseCase`2",
        "EAC.Foundation.Application.Commands.ICommand`1",
        "EAC.Foundation.Application.Dispatching.IUseCaseDispatcher",
        "EAC.Foundation.Application.Pagination.PageRequest",
        "EAC.Foundation.Application.Pagination.Page`1",
        "EAC.Foundation.Application.Persistence.CommitResult",
        "EAC.Foundation.Application.Persistence.IQueryService`2",
        "EAC.Foundation.Application.Persistence.IRepository`2",
        "EAC.Foundation.Application.Persistence.IUnitOfWork",
        "EAC.Foundation.Application.Pipeline.IPipelineBehavior`2",
        "EAC.Foundation.Application.Pipeline.UseCaseContinuation`1",
        "EAC.Foundation.Application.Queries.IQueryUseCase`2",
        "EAC.Foundation.Application.Queries.IQuery`1",
        "EAC.Foundation.Application.Requests.IUseCaseRequest`1",
        "EAC.Foundation.Application.Requests.IUseCase`2",
        "EAC.Foundation.Application.Validation.IUseCaseRequestValidator`1",
        "EAC.Foundation.Application.Validation.ValidationError",
        "EAC.Foundation.Application.Validation.ValidationFailure",
        "EAC.Foundation.Application.Validation.ValidationOutcome",
        "EAC.Foundation.Domain.AggregateRoot`1",
        "EAC.Foundation.Domain.Entity`1",
        "EAC.Foundation.Domain.IAggregateRoot",
        "EAC.Foundation.Domain.IEntity`1",
        "EAC.Foundation.Domain.IHasDomainEvents",
        "EAC.Foundation.Domain.ValueObject",
        "EAC.Foundation.SharedKernel.Domain.IDomainEvent",
        "EAC.Foundation.SharedKernel.Results.Error",
        "EAC.Foundation.SharedKernel.Results.ErrorType",
        "EAC.Foundation.SharedKernel.Results.IError",
        "EAC.Foundation.SharedKernel.Results.Result",
        "EAC.Foundation.SharedKernel.Results.Result`1",
    ];

    private static readonly string[] ApprovedErrorTypeValues =
    [
        "Failure=0",
        "Validation=1",
        "NotFound=2",
        "Conflict=3",
        "Unauthorized=4",
        "Forbidden=5",
        "Unavailable=6",
    ];

    private static readonly string[] ApprovedErrorProperties =
    [
        "Code:System.String",
        "Description:System.String",
        $"Type:{typeof(ErrorType).FullName}",
    ];

    private static readonly string[] ApprovedErrorConstructors =
    [
        $".ctor(System.String,System.String,{typeof(ErrorType).FullName})",
    ];

    private static readonly string[] ApprovedErrorFactories =
    [
        $"static {typeof(Error).FullName} Conflict(System.String,System.String)",
        $"static {typeof(Error).FullName} Failure(System.String,System.String)",
        $"static {typeof(Error).FullName} Forbidden(System.String,System.String)",
        $"static {typeof(Error).FullName} NotFound(System.String,System.String)",
        $"static {typeof(Error).FullName} Unauthorized(System.String,System.String)",
        $"static {typeof(Error).FullName} Unavailable(System.String,System.String)",
        $"static {typeof(Error).FullName} Validation(System.String,System.String)",
    ];

    private static readonly string[] ApprovedResultProperties =
    [
        $"Error:{typeof(IError).FullName}",
        "IsFailure:System.Boolean",
        "IsSuccess:System.Boolean",
    ];

    private static readonly string[] ApprovedResultMethods =
    [
        $"TResult Match``1(System.Func<TResult>,System.Func<{typeof(IError).FullName},TResult>)",
        $"static {typeof(Result).FullName} Failure({typeof(IError).FullName})",
        $"static {typeof(Result).FullName} Success()",
    ];

    private static readonly string[] ApprovedGenericResultProperties =
    [
        $"Error:{typeof(IError).FullName}",
        "IsFailure:System.Boolean",
        "IsSuccess:System.Boolean",
        "Value:TValue",
    ];

    private static readonly string[] ApprovedGenericResultMethods =
    [
        $"TResult Match``1(System.Func<TValue,TResult>,System.Func<{typeof(IError).FullName},TResult>)",
        $"static EAC.Foundation.SharedKernel.Results.Result<TValue> Failure({typeof(IError).FullName})",
        "static EAC.Foundation.SharedKernel.Results.Result<TValue> Success(TValue)",
    ];

    private static readonly string[] ApprovedDomainEventProperties =
    [
        "EventId:System.Guid",
        "OccurredAtUtc:System.DateTimeOffset",
    ];

    [Fact(DisplayName = "Exports exactly the approved public types")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    [Trait("Rule", "EAC-CONF-APP-009")]
    public void PublicTypeSnapshotMatchesApprovedContract()
    {
        var actualTypes = FoundationAssembly
            .GetExportedTypes()
            .Select(type => type.FullName)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ApprovedPublicTypes, actualTypes);
    }

    [Fact(DisplayName = "Preserves ErrorType names and numeric values")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void ErrorTypeSnapshotPreservesNamesAndNumericValues()
    {
        var actualValues = Enum
            .GetValues<ErrorType>()
            .Select(value => $"{value}={(int)value}")
            .ToArray();

        Assert.Equal(ApprovedErrorTypeValues, actualValues);
    }

    [Fact(DisplayName = "Keeps the approved Error public contract")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void ErrorSnapshotMatchesApprovedPropertiesConstructorAndFactories()
    {
        Assert.Equal(ApprovedErrorProperties, GetPublicPropertySnapshot(typeof(Error)));
        Assert.Equal(ApprovedErrorConstructors, GetPublicConstructorSnapshot(typeof(Error)));
        Assert.Equal(ApprovedErrorFactories, GetBusinessMethodSnapshot(typeof(Error)));
    }

    [Fact(DisplayName = "Keeps the approved Result public contract")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void ResultSnapshotMatchesApprovedContract()
    {
        Assert.Equal(ApprovedResultProperties, GetPublicPropertySnapshot(typeof(Result)));
        Assert.Empty(GetPublicConstructorSnapshot(typeof(Result)));
        Assert.Equal(ApprovedResultMethods, GetBusinessMethodSnapshot(typeof(Result)));
    }

    [Fact(DisplayName = "Keeps the approved generic Result public contract")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void GenericResultSnapshotMatchesApprovedContract()
    {
        var genericResult = typeof(Result<>);

        Assert.Equal(ApprovedGenericResultProperties, GetPublicPropertySnapshot(genericResult));
        Assert.Empty(GetPublicConstructorSnapshot(genericResult));
        Assert.Equal(ApprovedGenericResultMethods, GetBusinessMethodSnapshot(genericResult));
    }

    [Fact(DisplayName = "Keeps the approved interface properties")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void InterfaceSnapshotMatchesApprovedProperties()
    {
        Assert.Equal(ApprovedErrorProperties, GetPublicPropertySnapshot(typeof(IError)));
        Assert.Equal(ApprovedDomainEventProperties, GetPublicPropertySnapshot(typeof(IDomainEvent)));
    }

    [Fact(DisplayName = "Keeps the approved nullable annotations")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void NullableAnnotationsMatchApprovedContract()
    {
        var nullability = new NullabilityInfoContext();
        var errorProperty = typeof(Result).GetProperty(nameof(Result.Error));
        var valueProperty = typeof(Result<>).GetProperty(nameof(Result<object>.Value));
        var errorCodeProperty = typeof(IError).GetProperty(nameof(IError.Code));

        Assert.NotNull(errorProperty);
        Assert.NotNull(valueProperty);
        Assert.NotNull(errorCodeProperty);
        Assert.Equal(NullabilityState.Nullable, nullability.Create(errorProperty).ReadState);
        Assert.Equal(NullabilityState.Nullable, nullability.Create(valueProperty).ReadState);
        Assert.Equal(NullabilityState.NotNull, nullability.Create(errorCodeProperty).ReadState);
    }

    [Fact(DisplayName = "Exposes no implicit Result conversions")]
    [Trait("Rule", "EAC-CONF-FOUND-006")]
    public void ResultsDeclareNoImplicitConversions()
    {
        Assert.DoesNotContain(typeof(Result).GetMethods(), method => method.Name == "op_Implicit");
        Assert.DoesNotContain(typeof(Result<>).GetMethods(), method => method.Name == "op_Implicit");
    }

    private static string[] GetPublicPropertySnapshot(Type type) => type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Select(property => $"{property.Name}:{FormatType(property.PropertyType)}")
        .Order(StringComparer.Ordinal)
        .ToArray();

    private static string[] GetPublicConstructorSnapshot(Type type) => type
        .GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Select(constructor => $".ctor({FormatParameters(constructor.GetParameters())})")
        .Order(StringComparer.Ordinal)
        .ToArray();

    private static string[] GetBusinessMethodSnapshot(Type type) => type
        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Where(method => !method.IsSpecialName)
        .Where(method => method.Name is not nameof(ToString) and not nameof(GetHashCode) and not nameof(Equals))
        .Where(method => !method.Name.Contains("<Clone>$", StringComparison.Ordinal))
        .Select(method =>
        {
            var genericArity = method.IsGenericMethodDefinition
                ? $"``{method.GetGenericArguments().Length}"
                : string.Empty;
            var staticModifier = method.IsStatic ? "static " : string.Empty;
            return $"{staticModifier}{FormatType(method.ReturnType)} " +
                   $"{method.Name}{genericArity}({FormatParameters(method.GetParameters())})";
        })
        .Order(StringComparer.Ordinal)
        .ToArray();

    private static string FormatParameters(IEnumerable<ParameterInfo> parameters) =>
        string.Join(',', parameters.Select(parameter => FormatType(parameter.ParameterType)));

    private static string FormatType(Type type)
    {
        if (type.IsGenericParameter)
        {
            return type.Name;
        }

        if (!type.IsGenericType)
        {
            return type.FullName ?? type.Name;
        }

        var genericDefinition = type.GetGenericTypeDefinition();
        var genericName = genericDefinition.FullName ?? genericDefinition.Name;
        var aritySeparator = genericName.IndexOf('`', StringComparison.Ordinal);

        if (aritySeparator >= 0)
        {
            genericName = genericName[..aritySeparator];
        }

        var arguments = string.Join(',', type.GetGenericArguments().Select(FormatType));
        return $"{genericName}<{arguments}>";
    }
}
