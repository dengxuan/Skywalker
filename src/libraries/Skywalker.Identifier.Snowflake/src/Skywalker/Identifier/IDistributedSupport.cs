
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Identifier;

/// <summary>
/// 分布式支持
/// </summary>
public interface IDistributedSupport : ISingletonDependency
{
    /// <summary>
    /// 获取下一个可用的机器Id
    /// </summary>
    /// <returns></returns>
    Task<ushort> GetNextMechineId();

    /// <summary>
    /// 刷新机器Id的存活状态
    /// </summary>
    /// <returns></returns>
    Task RefreshAlive();
}
