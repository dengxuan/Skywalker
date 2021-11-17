using Microsoft.AspNetCore.Builder;
using Skywalker.Extensions.AspNetCore.Security;
using System;

namespace DependencyInjection
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
