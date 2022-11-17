using Microsoft.AspNetCore.Builder;
using Skywalker.AspNetCore.Security;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <inheritdoc/>
/// </summary>
public static class SecurityIApplicationBuilderExtensions
{

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }
        return app.UseMiddleware<ResponseEncryptionMiddleware>();
    }
}
