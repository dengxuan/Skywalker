using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.AspNetCore.Authentication.Abstractions;

namespace Skywalker.AspNetCore.Authentication;

public static class AuthenticationBuilderExtension
{
    public static AuthenticationBuilder AddSkywalker(this AuthenticationBuilder builder)
    {
        return builder.AddSkywalker(SkywalkerAuthenticationDefaults.AuthenticationScheme);
    }

    public static AuthenticationBuilder AddSkywalker(this AuthenticationBuilder builder, string scheme)
    {
        return builder.AddSkywalker(scheme, _ => { });
    }

    public static AuthenticationBuilder AddSkywalker(this AuthenticationBuilder builder, Action<SkywalkerAuthenticationOptions> configureOptions)
    {
        return builder.AddSkywalker(SkywalkerAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }

    public static AuthenticationBuilder AddSkywalker(this AuthenticationBuilder builder, string schema, Action<SkywalkerAuthenticationOptions> configureOptions)
    {
        builder.Services.AddSingleton<ISkywalkerTokenValidator, SkywalkerTokenValidator>();
        return builder.AddScheme<SkywalkerAuthenticationOptions, SkywalkerAuthenticationHandler>(schema, configureOptions);
    }
}
