using System;

namespace Skywalker.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// 表示一个需要自动注册的 Keyed Service 信息。
/// </summary>
internal sealed class KeyedServiceInfo : IEquatable<KeyedServiceInfo>
{
    /// <summary>
    /// 获取实现类的完全限定名称。
    /// </summary>
    public string ImplementationType { get; }

    /// <summary>
    /// 获取服务类型的完全限定名称。
    /// </summary>
    public string ServiceType { get; }

    /// <summary>
    /// 获取服务的 Key 值（字符串表示）。
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 获取 Key 值的类型（用于生成正确的代码）。
    /// </summary>
    public KeyType KeyType { get; }

    /// <summary>
    /// 获取服务生命周期。
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 初始化 <see cref="KeyedServiceInfo"/> 类的新实例。
    /// </summary>
    public KeyedServiceInfo(
        string implementationType,
        string serviceType,
        string key,
        KeyType keyType,
        ServiceLifetime lifetime)
    {
        ImplementationType = implementationType;
        ServiceType = serviceType;
        Key = key;
        KeyType = keyType;
        Lifetime = lifetime;
    }

    public bool Equals(KeyedServiceInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ImplementationType == other.ImplementationType &&
               ServiceType == other.ServiceType &&
               Key == other.Key;
    }

    public override bool Equals(object? obj) => Equals(obj as KeyedServiceInfo);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + ImplementationType.GetHashCode();
            hash = hash * 31 + ServiceType.GetHashCode();
            hash = hash * 31 + Key.GetHashCode();
            return hash;
        }
    }

    public override string ToString() => $"{ServiceType}[{Key}] -> {ImplementationType} ({Lifetime})";
}

/// <summary>
/// Key 值的类型。
/// </summary>
internal enum KeyType
{
    /// <summary>
    /// 字符串字面量。
    /// </summary>
    String,

    /// <summary>
    /// 整数字面量。
    /// </summary>
    Int,

    /// <summary>
    /// 常量引用（如 GamingConsts.LotteryTypes.Tgks）。
    /// </summary>
    ConstantReference
}
