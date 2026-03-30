namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 标记服务为单例生命周期（整个应用一个实例）。
/// </summary>
/// <remarks>
/// 实现此接口的类将由 Source Generator 自动注册到 DI 容器。
/// <para>
/// 使用示例：
/// <code>
/// public class CacheService : ICacheService, ISingletonDependency
/// {
///     // 整个应用程序生命周期内共享同一实例
/// }
/// </code>
/// </para>
/// </remarks>
public interface ISingletonDependency
{
}
