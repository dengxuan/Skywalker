namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 标记服务为作用域生命周期（每个作用域一个实例）。
/// </summary>
/// <remarks>
/// 实现此接口的类将由 Source Generator 自动注册到 DI 容器。
/// <para>
/// 使用示例：
/// <code>
/// public class UserRepository : IUserRepository, IScopedDependency
/// {
///     // 在同一个作用域（如 HTTP 请求）内共享同一实例
/// }
/// </code>
/// </para>
/// </remarks>
public interface IScopedDependency
{
}
