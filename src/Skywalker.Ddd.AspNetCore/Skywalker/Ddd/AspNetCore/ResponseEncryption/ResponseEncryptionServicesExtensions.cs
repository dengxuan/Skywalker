// Licensed to the Gordon

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Extension methods for the ResponseEncryption middleware.
/// </summary>
public static class ResponseEncryptionServicesExtensions
{
    /// <summary>
    /// Add response encryption services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddResponseEncryption(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IResponseEncryptionProvider, ResponseEncryptionProvider>();
        services.AddKeyedTransient<IEncryptionProvider, TripledesEncryptionProvider>(TripledesEncryptionProvider.Name);
        return services;
    }
}
