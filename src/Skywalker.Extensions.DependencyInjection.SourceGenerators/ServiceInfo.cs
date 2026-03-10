using System;
using System.Collections.Generic;

namespace Skywalker.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// 服务生命周期枚举。
/// </summary>
internal enum ServiceLifetime
{
    Transient,
    Scoped,
    Singleton
}

/// <summary>
/// 表示一个需要自动注册的服务信息。
/// </summary>
internal sealed class ServiceInfo : IEquatable<ServiceInfo>
{
    /// <summary>
    /// 获取实现类的完全限定名称。
    /// </summary>
    public string ImplementationType { get; }

    /// <summary>
    /// 获取实现类的命名空间。
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// 获取要注册的服务类型列表。
    /// </summary>
    public IReadOnlyList<string> ServiceTypes { get; }

    /// <summary>
    /// 获取服务生命周期。
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 获取是否使用替换模式注册。
    /// </summary>
    public bool ReplaceExisting { get; }

    /// <summary>
    /// 获取是否是开放泛型类型。
    /// </summary>
    public bool IsOpenGeneric { get; }

    /// <summary>
    /// 获取是否使用共享实例模式注册。
    /// </summary>
    /// <remarks>
    /// 当为 <c>true</c> 时，所有服务接口将通过工厂委托共享同一个实例。
    /// </remarks>
    public bool SharedInstance { get; }

    /// <summary>
    /// 初始化 <see cref="ServiceInfo"/> 类的新实例。
    /// </summary>
    public ServiceInfo(
        string implementationType,
        string @namespace,
        IReadOnlyList<string> serviceTypes,
        ServiceLifetime lifetime,
        bool replaceExisting,
        bool isOpenGeneric,
        bool sharedInstance = false)
    {
        ImplementationType = implementationType;
        Namespace = @namespace;
        ServiceTypes = serviceTypes;
        Lifetime = lifetime;
        ReplaceExisting = replaceExisting;
        IsOpenGeneric = isOpenGeneric;
        SharedInstance = sharedInstance;
    }

    public bool Equals(ServiceInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ImplementationType == other.ImplementationType;
    }

    public override bool Equals(object? obj) => Equals(obj as ServiceInfo);

    public override int GetHashCode() => ImplementationType.GetHashCode();

    public override string ToString() => $"{ImplementationType} ({Lifetime})";
}
