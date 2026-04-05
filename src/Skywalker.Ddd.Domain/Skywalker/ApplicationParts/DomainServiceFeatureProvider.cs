// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 从 <see cref="ApplicationPart"/> 中发现实现了 <see cref="IDomainService"/> 或 <see cref="IRepository"/> 的类型，
/// 按约定注册为 Scoped 生命周期，并注册 Domain 层框架服务。
/// 支持 <see cref="ExposeServicesAttribute"/>、<see cref="ReplaceServiceAttribute"/>、
/// <see cref="SharedInstanceAttribute"/>、<see cref="KeyedServiceAttribute"/> 等约定。
/// </summary>
public class DomainServiceFeatureProvider : IApplicationFeatureProvider<ServiceRegistrationFeature>
{
    private static readonly HashSet<Type> s_excludedInterfaces =
    [
        typeof(IDomainService),
        typeof(IInterceptable),
        typeof(IDisposable),
        typeof(IAsyncDisposable),
    ];

    /// <inheritdoc />
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ServiceRegistrationFeature feature)
    {
        // 调用外部组件的注册方法
        feature.ServiceCollection.AddDddExceptions();
        feature.ServiceCollection.AddEventBusLocal();
        feature.ServiceCollection.AddThreading();

        // Domain 框架服务
        feature.Services.Add(ServiceDescriptor.Singleton<IAsyncQueryableExecuter, AsyncQueryableExecuter>());
        feature.Services.Add(ServiceDescriptor.Singleton<IDataSeeder, DataSeeder>());
        feature.Services.Add(ServiceDescriptor.Singleton<IDataFilter, DataFilter>());
        feature.Services.Add(ServiceDescriptor.Singleton(typeof(IDataFilter<>), typeof(DataFilter<>)));
        feature.Services.Add(ServiceDescriptor.Singleton<IConnectionStringResolver, DefaultConnectionStringResolver>());

        // 扫描 IDomainService 和 IRepository 自定义实现
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

                ServiceRegistrationHelper.RegisterKeyedServices(type, feature);

                if (typeof(IDomainService).IsAssignableFrom(type))
                {
                    var interfaces = GetDomainServiceInterfaces(type);
                    ServiceRegistrationHelper.RegisterType(type, interfaces, ServiceLifetime.Scoped, feature);
                }
                else if (typeof(IRepository).IsAssignableFrom(type))
                {
                    var interfaces = GetCustomRepositoryInterfaces(type);
                    ServiceRegistrationHelper.RegisterType(type, interfaces, ServiceLifetime.Scoped, feature);
                }
            }
        }
    }

    private static IEnumerable<Type> GetDomainServiceInterfaces(Type type)
    {
        return type.GetInterfaces().Where(iface => !s_excludedInterfaces.Contains(iface));
    }

    private static IEnumerable<Type> GetCustomRepositoryInterfaces(Type type)
    {
        // 只排除非泛型 IRepository 标记接口；标准泛型接口（IRepository<T> 等）保留注册，
        // 这样用户自定义的仓储可以覆盖 AddSkywalkerDbContext 的默认注册（TryAdd 先到先得）。
        return type.GetInterfaces().Where(iface =>
            typeof(IRepository).IsAssignableFrom(iface) && iface != typeof(IRepository));
    }
}
