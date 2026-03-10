// Licensed to the Gordon

using Microsoft.AspNetCore.Builder;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Extension methods for the ResponseEncryption middleware.
/// </summary>
public static class ResponseEncryptionBuilderExtensions
{
    /// <summary>
    /// Adds middleware for dynamically encrypting HTTP Responses.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
    public static IApplicationBuilder UseResponseEncryption(this IApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseMiddleware<ResponseEncryptionMiddleware>();
    }
}
