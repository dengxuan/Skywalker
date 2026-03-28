
namespace Skywalker.DependencyInjection;

/// <summary>
/// 指定类应该注册为 Keyed Service。
/// </summary>
/// <remarks>
/// 使用此特性可以将服务注册为 Keyed Service，通过 key 来区分不同的实现。
/// <para>
/// 使用示例：
/// <code>
/// [KeyedService(typeof(ILotteryRewarder), "Tgks")]
/// public class TgksLotteryRewarder : ILotteryRewarder, ITransientDependency
/// {
///     // 注册为 services.AddKeyedTransient&lt;ILotteryRewarder, TgksLotteryRewarder&gt;("Tgks")
/// }
/// </code>
/// </para>
/// <para>
/// 也可以使用常量：
/// <code>
/// [KeyedService(typeof(ILotteryRewarder), GamingConsts.LotteryTypes.Tgks)]
/// public class TgksLotteryRewarder : ILotteryRewarder, ITransientDependency
/// {
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class KeyedServiceAttribute : Attribute
{
    /// <summary>
    /// 获取要注册的服务类型。
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// 获取服务的 Key。
    /// </summary>
    public object Key { get; }

    /// <summary>
    /// 初始化 <see cref="KeyedServiceAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="serviceType">要注册的服务类型。</param>
    /// <param name="key">服务的 Key。</param>
    public KeyedServiceAttribute(Type serviceType, object key)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }
}
