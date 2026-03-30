using Castle.DynamicProxy;
using CastleInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 代理生成器接口。
/// </summary>
public interface IProxyGenerator
{
    /// <summary>
    /// 为指定类型创建接口代理。
    /// </summary>
    /// <typeparam name="TInterface">接口类型。</typeparam>
    /// <param name="target">目标实例。</param>
    /// <returns>代理实例。</returns>
    TInterface CreateInterfaceProxy<TInterface>(TInterface target) where TInterface : class;

    /// <summary>
    /// 为指定类型创建接口代理。
    /// </summary>
    /// <param name="interfaceType">接口类型。</param>
    /// <param name="target">目标实例。</param>
    /// <returns>代理实例。</returns>
    object CreateInterfaceProxy(Type interfaceType, object target);

    /// <summary>
    /// 为指定类型创建类代理。
    /// </summary>
    /// <typeparam name="TClass">类类型。</typeparam>
    /// <param name="arguments">构造函数参数。</param>
    /// <returns>代理实例。</returns>
    TClass CreateClassProxy<TClass>(params object[] arguments) where TClass : class;

    /// <summary>
    /// 为指定类型创建类代理。
    /// </summary>
    /// <param name="classType">类类型。</param>
    /// <param name="arguments">构造函数参数。</param>
    /// <returns>代理实例。</returns>
    object CreateClassProxy(Type classType, params object[] arguments);
}

/// <summary>
/// 基于 Castle.DynamicProxy 的代理生成器实现。
/// </summary>
public sealed class CastleProxyGenerator : IProxyGenerator
{
    private readonly Castle.DynamicProxy.ProxyGenerator _proxyGenerator;
    private readonly CastleInterceptor _interceptorAdapter;

    /// <summary>
    /// 初始化 <see cref="CastleProxyGenerator"/> 类的新实例。
    /// </summary>
    /// <param name="interceptors">拦截器列表。</param>
    public CastleProxyGenerator(IEnumerable<IInterceptor> interceptors)
    {
        _proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
        _interceptorAdapter = new CastleInterceptorAdapter(interceptors);
    }

    /// <inheritdoc/>
    public TInterface CreateInterfaceProxy<TInterface>(TInterface target) where TInterface : class
    {
        return _proxyGenerator.CreateInterfaceProxyWithTarget(target, _interceptorAdapter);
    }

    /// <inheritdoc/>
    public object CreateInterfaceProxy(Type interfaceType, object target)
    {
        return _proxyGenerator.CreateInterfaceProxyWithTarget(interfaceType, target, _interceptorAdapter);
    }

    /// <inheritdoc/>
    public TClass CreateClassProxy<TClass>(params object[] arguments) where TClass : class
    {
        if (arguments.Length == 0)
        {
            return _proxyGenerator.CreateClassProxy<TClass>(_interceptorAdapter);
        }
        // 使用正确的重载：CreateClassProxy(Type, Type[], ProxyGenerationOptions, object[], IInterceptor[])
        return (TClass)_proxyGenerator.CreateClassProxy(
            typeof(TClass),
            Type.EmptyTypes,
            ProxyGenerationOptions.Default,
            arguments,
            new CastleInterceptor[] { _interceptorAdapter });
    }

    /// <inheritdoc/>
    public object CreateClassProxy(Type classType, params object[] arguments)
    {
        if (arguments.Length == 0)
        {
            return _proxyGenerator.CreateClassProxy(classType, new CastleInterceptor[] { _interceptorAdapter });
        }
        return _proxyGenerator.CreateClassProxy(
            classType,
            Type.EmptyTypes,
            ProxyGenerationOptions.Default,
            arguments,
            new CastleInterceptor[] { _interceptorAdapter });
    }
}
