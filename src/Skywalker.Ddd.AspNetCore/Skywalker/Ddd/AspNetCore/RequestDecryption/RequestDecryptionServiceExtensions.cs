// Licensed to the Gordon

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <summary>
/// Extension methods for the HTTP request decryption middleware.
/// </summary>
public static class RequestDecryptionServiceExtensions
{
    /// <summary>
    /// Add request decryption services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRequestDecryption(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IRequestDecryptionProvider, DefaultRequestDecryptionProvider>();
        services.AddKeyedTransient<IDecryptionProvider, TripledesDecryptionProvider>(TripledesDecryptionProvider.Name);
        return services;
    }
}
