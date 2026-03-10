using System.Reflection;
using FluentValidation;
using Skywalker.Validation.FluentValidation;
using FV = FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding FluentValidation services.
/// </summary>
public static class FluentValidationServiceCollectionExtensions
{
    /// <summary>
    /// Adds FluentValidation validators from the specified assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly to scan for validators.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, Assembly assembly)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // Register adapters for each validator
        var validatorTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(FV.IValidator<>)));

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(FV.IValidator<>));

            var modelType = validatorInterface.GetGenericArguments()[0];
            var adapterType = typeof(FluentValidationValidatorAdapter<>).MakeGenericType(modelType);
            var skywalkerValidatorInterface = typeof(Skywalker.Validation.IValidator<>).MakeGenericType(modelType);

            services.AddTransient(skywalkerValidatorInterface, sp =>
            {
                var fluentValidator = sp.GetRequiredService(validatorInterface);
                return Activator.CreateInstance(adapterType, fluentValidator)!;
            });
        }

        return services;
    }

    /// <summary>
    /// Adds FluentValidation validators from the assembly containing the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the assembly to scan.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddFluentValidation<T>(this IServiceCollection services)
    {
        return services.AddFluentValidation(typeof(T).Assembly);
    }
}

