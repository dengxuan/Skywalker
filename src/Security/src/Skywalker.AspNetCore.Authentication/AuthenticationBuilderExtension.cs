using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.AspNetCore.Authentication.Abstractions;
using System;

namespace Skywalker.AspNetCore.Authentication
{
    public static class AuthenticationBuilderExtension
    {
        public static AuthenticationBuilder AddAuthentication(this IServiceCollection services, string defaultScheme)
        {
            return services.AddAuthentication(defaultScheme, SkywalkerAuthenticationDefaults.AuthenticationScheme);
        }

        public static AuthenticationBuilder AddAuthentication(this IServiceCollection services, string defaultScheme, string scheme)
        {
            return services.AddAuthentication(scheme, defaultScheme, _ => { });
        }

        public static AuthenticationBuilder AddAuthentication(this IServiceCollection services, string defaultScheme, Action<SkywalkerAuthenticationOptions> configureOptions)
        {
            return services.AddAuthentication(SkywalkerAuthenticationDefaults.AuthenticationScheme, defaultScheme, configureOptions);
        }

        public static AuthenticationBuilder AddAuthentication(this IServiceCollection services, string defaultScheme, string schema, Action<SkywalkerAuthenticationOptions> configureOptions)
        {
            services.AddSingleton<ISkywalkerTokenValidator, SkywalkerTokenValidator>();
            AuthenticationBuilder authenticationBuilder = services.AddAuthentication(defaultScheme);
            return authenticationBuilder.AddScheme<SkywalkerAuthenticationOptions, SkywalkerAuthenticationHandler>(schema, configureOptions);
        }
    }
}
