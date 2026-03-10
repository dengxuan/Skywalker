// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.ObjectAccessor;
using Skywalker.Extensions.ObjectAccessor.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ObjectAccessorIServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="services"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static IServiceCollection AddObjectAccessor<TInstance>(this IServiceCollection services, TInstance instance) where TInstance : class
    {
        services.TryAddSingleton<IObjectAccessor<TInstance>>(new DefaultObjectAccessor<TInstance>(instance));
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="services"></param>
    /// <param name="instanceFactory"></param>
    /// <returns></returns>
    public static IServiceCollection AddObjectAccessor<TInstance>(this IServiceCollection services, Func<IServiceProvider, TInstance> instanceFactory) where TInstance : class
    {
        services.TryAddSingleton<IObjectAccessor<TInstance>>(sp => new DefaultObjectAccessor<TInstance>(instanceFactory(sp)));
        return services;
    }
}
