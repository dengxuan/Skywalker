// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Sms;
using Skywalker.Sms.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SmsIServiceCollectionExtensions
{
    /// <summary>
    /// Adds SMS services with NullSmsSender as default fallback.
    /// </summary>
    public static IServiceCollection AddSms(this IServiceCollection services)
    {
        services.TryAddSingleton<ISmsSender, NullSmsSender>();
        return services;
    }
}
