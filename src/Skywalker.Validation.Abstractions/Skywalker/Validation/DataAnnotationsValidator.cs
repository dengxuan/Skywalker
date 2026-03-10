using System.ComponentModel.DataAnnotations;
using Skywalker.DependencyInjection;

namespace Skywalker.Validation;

/// <summary>
/// Validator that uses DataAnnotations attributes.
/// </summary>
public class DataAnnotationsValidator : IValidator, ITransientDependency
{
    public IValidationResult Validate(object instance)
    {
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new ValidationContext(instance);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            instance, context, results, validateAllProperties: true);

        if (isValid)
        {
            return ValidationResult.Success();
        }

        var errors = results.SelectMany(r =>
            r.MemberNames.DefaultIfEmpty(string.Empty)
                .Select(memberName => new ValidationError(memberName, r.ErrorMessage ?? "Validation failed")));

        return ValidationResult.Failure(errors);
    }

    public Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Validate(instance));
    }
}

/// <summary>
/// Generic DataAnnotations validator.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public class DataAnnotationsValidator<T> : DataAnnotationsValidator, IValidator<T> where T : class
{
    public IValidationResult Validate(T instance)
    {
        return base.Validate(instance);
    }

    public Task<IValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        return base.ValidateAsync(instance, cancellationToken);
    }
}

