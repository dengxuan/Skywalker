namespace Skywalker.Validation;

/// <summary>
/// Interface for objects that can validate themselves.
/// </summary>
public interface IValidatableObject
{
    /// <summary>
    /// Validates this object.
    /// </summary>
    /// <returns>A collection of validation errors.</returns>
    IEnumerable<ValidationError> Validate();
}

/// <summary>
/// Interface for objects that can validate themselves asynchronously.
/// </summary>
public interface IAsyncValidatableObject
{
    /// <summary>
    /// Validates this object asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of validation errors.</returns>
    Task<IEnumerable<ValidationError>> ValidateAsync(CancellationToken cancellationToken = default);
}

