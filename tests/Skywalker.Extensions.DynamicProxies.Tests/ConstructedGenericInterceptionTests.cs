using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Extensions.DynamicProxies.Tests;

/// <summary>
/// #308：闭合泛型实现（如 EF SG 生成注册的 EntityFrameworkCoreDomainService&lt;TEntity, TKey&gt;）
/// 不可能拥有 source-generated 代理；AddInterceptedServices 必须跳过而非抛出启动异常。
/// </summary>
public sealed class ConstructedGenericInterceptionTests
{
    public interface IGenericDomainService<TEntity> : IInterceptable
    {
        Task<TEntity?> FindAsync();
    }

    public class GenericDomainService<TEntity> : IGenericDomainService<TEntity>
    {
        public Task<TEntity?> FindAsync() => Task.FromResult(default(TEntity));
    }

    [Fact]
    public void AddInterceptedServices_SkipsConstructedGenericImplementations_WithoutThrowing()
    {
        var services = new ServiceCollection();
        // 模拟 EF SG 生成的闭合泛型注册：IDomainService<Entity> -> EntityFrameworkCoreDomainService<Entity, Key>
        services.AddTransient(typeof(IGenericDomainService<string>), typeof(GenericDomainService<string>));

        // 修复前此调用抛 InvalidOperationException（no source-generated DynamicProxy metadata）
        var exception = Record.Exception(() => services.AddInterceptedServices());

        Assert.Null(exception);

        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IGenericDomainService<string>>();

        // 原注册保持不代理
        Assert.IsType<GenericDomainService<string>>(service);
    }
}
