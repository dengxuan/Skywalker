using Microsoft.AspNetCore.Builder;
using System;

namespace Skywalker.Extensions.AspNetCore.Security.DependencyInjection
{
    public static class SecurityIApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ResponseEncryptionMiddleware>();
        }
    }
}
