using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Validation;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding validation services.
/// </summary>
public static class ValidationServiceCollectionExtensions
{
    /// <summary>
    /// Adds validation services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddAutoServices();
        services.TryAddTransient(typeof(IValidator<>), typeof(DataAnnotationsValidator<>));

        return services;
    }

    /// <summary>
    /// Adds a validator to the service collection.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddValidator<TValidator>(this IServiceCollection services)
        where TValidator : class, IValidator
    {
        services.AddTransient<IValidator, TValidator>();
        return services;
    }

    /// <summary>
    /// Adds a validator for a specific type.
    /// </summary>
    /// <typeparam name="T">The type to validate.</typeparam>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddValidator<T, TValidator>(this IServiceCollection services)
        where T : class
        where TValidator : class, IValidator<T>
    {
        services.AddTransient<IValidator<T>, TValidator>();
        return services;
    }
}

