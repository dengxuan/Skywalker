// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 注册 Entity Framework Core 基础框架服务。
/// </summary>
public class EntityFrameworkCoreFeatureProvider : IApplicationFeatureProvider<ServiceRegistrationFeature>
{
    /// <inheritdoc />
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ServiceRegistrationFeature feature)
    {
        // 调用外部组件的注册方法
        feature.ServiceCollection.AddGuidGenerator();
        feature.ServiceCollection.AddTimezone();

        // EFCore 框架服务
        feature.Services.Add(ServiceDescriptor.Singleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>)));
        feature.Services.Add(ServiceDescriptor.Transient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>());
    }
}
