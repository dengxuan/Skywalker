namespace Skywalker.Validation;

/// <summary>
/// Interface for validators.
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Validates the specified object.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <returns>The validation result.</returns>
    IValidationResult Validate(object instance);

    /// <summary>
    /// Validates the specified object asynchronously.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic interface for validators.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public interface IValidator<in T> : IValidator where T : class
{
    /// <summary>
    /// Validates the specified object.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <returns>The validation result.</returns>
    IValidationResult Validate(T instance);

    /// <summary>
    /// Validates the specified object asynchronously.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    Task<IValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}

