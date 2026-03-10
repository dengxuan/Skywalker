namespace Skywalker.DependencyInjection;

/// <summary>
/// 标记服务为瞬态生命周期（每次请求创建新实例）。
/// </summary>
/// <remarks>
/// 实现此接口的类将由 Source Generator 自动注册到 DI 容器。
/// <para>
/// 使用示例：
/// <code>
/// public class EmailService : IEmailService, ITransientDependency
/// {
///     // 每次注入都会创建新实例
/// }
/// </code>
/// </para>
/// </remarks>
public interface ITransientDependency
{
}
