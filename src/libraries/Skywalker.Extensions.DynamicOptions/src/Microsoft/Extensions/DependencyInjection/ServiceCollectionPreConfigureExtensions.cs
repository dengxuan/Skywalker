// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DynamicOptions;
using Skywalker.Extensions.ObjectAccessor.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
internal static class ServiceCollectionPreConfigureExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static IServiceCollection PreConfigure<TOptions>(this IServiceCollection services, Action<TOptions> optionsAction)
    {
        services.GetPreConfigureActions<TOptions>().Add(optionsAction);
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static TOptions ExecutePreConfiguredActions<TOptions>(this IServiceCollection services)
        where TOptions : new()
    {
        return services.ExecutePreConfiguredActions(new TOptions());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static TOptions ExecutePreConfiguredActions<TOptions>(this IServiceCollection services, TOptions options)
    {
        services.GetPreConfigureActions<TOptions>().Configure(options);
        return options;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static PreConfigureActionList<TOptions> GetPreConfigureActions<TOptions>(this IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IObjectAccessor<PreConfigureActionList<TOptions>>>()?.Value;
        if (actionList == null)
        {
            actionList = new PreConfigureActionList<TOptions>();
            services.AddObjectAccessor(actionList);
        }

        return actionList;
    }
}
