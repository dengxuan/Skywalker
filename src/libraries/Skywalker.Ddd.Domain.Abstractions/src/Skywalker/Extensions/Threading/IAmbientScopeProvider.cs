using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.Threading;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAmbientScopeProvider<T> : ISingletonDependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextKey"></param>
    /// <returns></returns>
    T? GetValue(string contextKey);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextKey"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IDisposable BeginScope(string contextKey, T value);
}
