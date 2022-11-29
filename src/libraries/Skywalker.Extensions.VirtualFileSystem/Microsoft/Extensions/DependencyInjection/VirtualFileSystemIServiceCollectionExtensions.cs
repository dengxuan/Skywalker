// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.VirtualFileSystem;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class VirtualFileSystemIServiceCollectionExtensions
{
    public static IServiceCollection AddVirtualFileSystem(this IServiceCollection services)
    {
        services.TryAddSingleton<IDynamicFileProvider, DynamicFileProvider>();
        services.TryAddSingleton<IVirtualFileProvider, VirtualFileProvider>();
        return services;
    }
}
