// Licensed to the Gordon

using Microsoft.AspNetCore.Builder;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <summary>
/// Extension methods for the HTTP request decryption middleware.
/// </summary>
public static class RequestDecryptionBuilderExtensions
{
    /// <summary>
    /// Adds middleware for dynamically decrypting HTTP request bodies.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
    public static IApplicationBuilder UseRequestDecryption(this IApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseMiddleware<RequestDecryptionMiddleware>();
    }
}
