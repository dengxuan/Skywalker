// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DDD Exceptions 服务扩展方法。
/// </summary>
public static class DddExceptionsServiceCollectionExtensions
{
    /// <summary>
    /// 添加 DDD Exceptions 服务。
    /// </summary>
    public static IServiceCollection AddDddExceptions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAutoServices();

        return services;
    }
}
