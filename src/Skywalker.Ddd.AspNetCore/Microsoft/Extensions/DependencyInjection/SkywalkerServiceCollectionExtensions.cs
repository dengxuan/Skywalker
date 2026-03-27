// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker ASP.NET Core 服务注册扩展方法。
/// </summary>
public static class SkywalkerAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// 添加 ASP.NET Core 特有服务：异常处理、响应包装。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <returns>Skywalker 构建器。</returns>
    public static ISkywalkerBuilder AddAspNetCore(this ISkywalkerBuilder builder)
    {
        builder.Services.AddSkywalkerExceptionHandling();
        builder.Services.AddResponseWrapper();
        return builder;
    }
}
