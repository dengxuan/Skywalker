// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.ExceptionHandler;
using Skywalker.Extensions.DynamicOptions;

namespace Microsoft.Extensions.Options;

/// <summary>
/// 
/// </summary>
public static class OptionsAbpDynamicOptionsManagerExtensions
{
    public static Task SetAsync<T>(this IOptions<T> options)
        where T : class
    {
        return options.ToDynamicOptions().SetAsync();
    }

    public static Task SetAsync<T>(this IOptions<T> options, string name)
        where T : class
    {
        return options.ToDynamicOptions().SetAsync(name);
    }

    private static SkywalkerDynamicOptionsManager<T> ToDynamicOptions<T>(this IOptions<T> options)
        where T : class
    {
        if (options is SkywalkerDynamicOptionsManager<T> dynamicOptionsManager)
        {
            return dynamicOptionsManager;
        }

        throw new SkywalkerException($"Options must be derived from the {typeof(SkywalkerDynamicOptionsManager<>).FullName}!");
    }
}
