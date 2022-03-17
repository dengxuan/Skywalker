namespace Skywalker.IdentifierGenerator.Abstractions;

public interface IdistributedSupport
{
    /// <summary>
    /// 获取下一个可用的机器Id
    /// </summary>
    /// <returns></returns>
    Task<int> GetNextMechineId();

    /// <summary>
    /// 刷新机器Id的存活状态
    /// </summary>
    /// <returns></returns>
    Task RefreshAlive();
}
