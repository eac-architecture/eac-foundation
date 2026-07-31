namespace EAC.Foundation.Application.Validation;

/// <summary>
/// Validates one use case request without coupling the application contract to a validation provider.
/// </summary>
/// <typeparam name="TUseCaseRequest">Type of use case request to validate.</typeparam>
public interface IUseCaseRequestValidator<in TUseCaseRequest>
{
    /// <summary>Validates the supplied use case request asynchronously.</summary>
    /// <param name="useCaseRequest">Use case request to validate.</param>
    /// <param name="cancellationToken">Token used to cancel the validation operation.</param>
    /// <returns>The explicit validation outcome.</returns>
    ValueTask<ValidationOutcome> ValidateAsync(
        TUseCaseRequest useCaseRequest,
        CancellationToken cancellationToken = default);
}
