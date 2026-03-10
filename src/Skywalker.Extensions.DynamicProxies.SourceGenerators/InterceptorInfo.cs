using System;
using System.Collections.Generic;

namespace Skywalker.Extensions.DynamicProxies.SourceGenerators;

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
/// 表示需要被代理的服务信息。
/// </summary>
internal sealed class ProxyServiceInfo : IEquatable<ProxyServiceInfo>
{
    /// <summary>
    /// 获取实现类的完全限定名称。
    /// </summary>
    public string ImplementationType { get; }

    /// <summary>
    /// 获取命名空间。
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// 获取类名（不含命名空间）。
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// 获取要代理的接口类型列表。
    /// </summary>
    public IReadOnlyList<string> ServiceInterfaces { get; }

    /// <summary>
    /// 获取需要代理的方法信息。
    /// </summary>
    public IReadOnlyList<ProxyMethodInfo> Methods { get; }

    /// <summary>
    /// 获取服务生命周期。
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 获取原类是否为 public。
    /// </summary>
    public bool IsPublic { get; }

    public ProxyServiceInfo(
        string implementationType,
        string @namespace,
        string className,
        IReadOnlyList<string> serviceInterfaces,
        IReadOnlyList<ProxyMethodInfo> methods,
        ServiceLifetime lifetime,
        bool isPublic = true)
    {
        ImplementationType = implementationType;
        Namespace = @namespace;
        ClassName = className;
        ServiceInterfaces = serviceInterfaces;
        Methods = methods;
        Lifetime = lifetime;
        IsPublic = isPublic;
    }

    public bool Equals(ProxyServiceInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ImplementationType == other.ImplementationType;
    }

    public override bool Equals(object? obj) => Equals(obj as ProxyServiceInfo);

    public override int GetHashCode() => ImplementationType.GetHashCode();
}

/// <summary>
/// 表示需要代理的方法信息。
/// </summary>
internal sealed class ProxyMethodInfo
{
    /// <summary>
    /// 获取方法名称。
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// 获取返回类型。
    /// </summary>
    public string ReturnType { get; }

    /// <summary>
    /// 获取参数列表。
    /// </summary>
    public IReadOnlyList<(string Type, string Name)> Parameters { get; }

    /// <summary>
    /// 获取是否是异步方法。
    /// </summary>
    public bool IsAsync { get; }

    /// <summary>
    /// 获取是否有返回值。
    /// </summary>
    public bool HasReturnValue { get; }

    /// <summary>
    /// 获取泛型参数。
    /// </summary>
    public IReadOnlyList<string> TypeParameters { get; }

    /// <summary>
    /// 获取泛型约束。
    /// </summary>
    public IReadOnlyList<string> TypeConstraints { get; }

    public ProxyMethodInfo(
        string methodName,
        string returnType,
        IReadOnlyList<(string Type, string Name)> parameters,
        bool isAsync,
        bool hasReturnValue,
        IReadOnlyList<string> typeParameters,
        IReadOnlyList<string> typeConstraints)
    {
        MethodName = methodName;
        ReturnType = returnType;
        Parameters = parameters;
        IsAsync = isAsync;
        HasReturnValue = hasReturnValue;
        TypeParameters = typeParameters;
        TypeConstraints = typeConstraints;
    }
}
