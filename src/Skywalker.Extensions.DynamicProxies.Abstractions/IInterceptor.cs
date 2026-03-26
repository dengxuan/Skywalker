
namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 定义拦截器接口。
/// </summary>
/// <remarks>
/// 实现此接口以创建自定义拦截器，用于在方法调用前后执行逻辑。
/// <para>
/// 使用示例：
/// <code>
/// public class LoggingInterceptor : IInterceptor, ITransientDependency
/// {
///     public async Task InterceptAsync(IMethodInvocation invocation)
///     {
///         Console.WriteLine($"Before: {invocation.MethodName}");
///         await invocation.ProceedAsync();
///         Console.WriteLine($"After: {invocation.MethodName}");
///     }
/// }
/// </code>
/// </para>
/// </remarks>
public interface IInterceptor
{
    /// <summary>
    /// 拦截方法调用。
    /// </summary>
    /// <param name="invocation">方法调用上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    /// <remarks>
    /// 实现者必须调用 <see cref="IMethodInvocation.ProceedAsync"/> 来继续执行调用链，
    /// 否则实际方法不会被调用。
    /// </remarks>
    Task InterceptAsync(IMethodInvocation invocation);
}
