// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Security 服务扩展方法。
/// </summary>
public static class SecurityIServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Security 模块服务到服务集合。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        return services.AddAutoServices();
    }
}
