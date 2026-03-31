// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 从 <see cref="ApplicationPart"/> 中发现实现了 <see cref="IApplicationService"/> 的类型，
/// 按约定注册为 Scoped 生命周期。
/// </summary>
public class ApplicationServiceFeatureProvider : IApplicationFeatureProvider<ServiceRegistrationFeature>
{
    private static readonly HashSet<Type> s_excludedInterfaces =
    [
        typeof(IApplicationService),
        typeof(IInterceptable),
        typeof(IDisposable),
        typeof(IAsyncDisposable),
    ];

    /// <inheritdoc />
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ServiceRegistrationFeature feature)
    {
        foreach (var part in parts)
        {
            if (part is not IApplicationPartTypeProvider typeProvider)
            {
                continue;
            }

            foreach (var typeInfo in typeProvider.Types)
            {
                var type = typeInfo.AsType();

                if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
                {
                    continue;
                }

                if (type.IsDefined(typeof(DisableAutoRegistrationAttribute), false))
                {
                    continue;
                }

                if (!typeof(IApplicationService).IsAssignableFrom(type))
                {
                    continue;
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (s_excludedInterfaces.Contains(iface))
                    {
                        continue;
                    }

                    feature.Services.Add(new ServiceDescriptor(iface, type, ServiceLifetime.Scoped));
                }
            }
        }
    }
}
