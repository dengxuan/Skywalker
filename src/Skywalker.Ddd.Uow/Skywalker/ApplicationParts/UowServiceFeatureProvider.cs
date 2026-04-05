// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.ApplicationParts;

/// <summary>
/// UoW 层服务提供者，向 <see cref="ServiceRegistrationFeature"/> 注册工作单元相关基础服务。
/// </summary>
public class UowServiceFeatureProvider : IApplicationFeatureProvider<ServiceRegistrationFeature>
{
    /// <inheritdoc />
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ServiceRegistrationFeature feature)
    {
        // Singleton
        feature.Services.Add(ServiceDescriptor.Singleton<IUnitOfWorkManager, UnitOfWorkManager>());
        feature.Services.Add(ServiceDescriptor.Singleton<IUnitOfWorkTransactionBehaviourProvider, NullUnitOfWorkTransactionBehaviourProvider>());
        feature.Services.Add(ServiceDescriptor.Singleton<AmbientUnitOfWork, AmbientUnitOfWork>());
        feature.Services.Add(ServiceDescriptor.Singleton<IAmbientUnitOfWork>(sp => sp.GetRequiredService<AmbientUnitOfWork>()));
        feature.Services.Add(ServiceDescriptor.Singleton<IUnitOfWorkAccessor>(sp => sp.GetRequiredService<AmbientUnitOfWork>()));

        // Transient
        feature.Services.Add(ServiceDescriptor.Transient<IUnitOfWork, UnitOfWork>());
        feature.Services.Add(ServiceDescriptor.Transient<IInterceptor, UnitOfWorkInterceptor>());
    }
}
