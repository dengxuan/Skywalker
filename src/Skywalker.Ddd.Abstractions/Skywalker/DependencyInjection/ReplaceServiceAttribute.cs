
namespace Skywalker.DependencyInjection;

/// <summary>
/// 标记服务使用替换模式注册。
/// </summary>
/// <remarks>
/// 默认情况下，服务使用 <c>TryAdd</c> 方法注册（不会覆盖已有注册）。
/// 标记此特性后，将使用 <c>Replace</c> 方法注册（会覆盖已有注册）。
/// <para>
/// 使用示例：
/// <code>
/// [ReplaceService]
/// public class CustomEmailService : IEmailService, ITransientDependency
/// {
///     // 会替换已有的 IEmailService 注册
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ReplaceServiceAttribute : Attribute
{
}
