
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 标记此类需要共享实例注册。
/// </summary>
/// <remarks>
/// <para>
/// 当一个类实现多个服务接口时，默认情况下每个接口会分别注册到 DI 容器，
/// 可能导致多个实例被创建。使用此特性可以确保所有服务接口共享同一个实例。
/// </para>
/// <para>
/// 生成的注册代码会使用工厂模式：
/// <code>
/// // 首先注册实现类自身
/// services.TryAddSingleton&lt;AmbientUnitOfWork&gt;();
/// 
/// // 然后通过工厂委托注册各个接口
/// services.TryAddSingleton&lt;IAmbientUnitOfWork&gt;(sp => sp.GetRequiredService&lt;AmbientUnitOfWork&gt;());
/// services.TryAddSingleton&lt;IUnitOfWorkAccessor&gt;(sp => sp.GetRequiredService&lt;AmbientUnitOfWork&gt;());
/// </code>
/// </para>
/// <para>
/// 使用示例：
/// <code>
/// [SharedInstance]
/// [ExposeServices(typeof(IAmbientUnitOfWork), typeof(IUnitOfWorkAccessor))]
/// public class AmbientUnitOfWork : IAmbientUnitOfWork
/// {
///     private readonly AsyncLocal&lt;IUnitOfWork?&gt; _currentUow = new();
///     // ...
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SharedInstanceAttribute : Attribute
{
}
