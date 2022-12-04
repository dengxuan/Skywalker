namespace Skywalker.Extensions.Snowflake;

/// <summary>
/// 分布式支持
/// </summary>
public interface IWorker
{
    /// <summary>
    /// 获取下一个可用的机器Id
    /// </summary>
    /// <returns></returns>
    ushort NextWorkerId();

    /// <summary>
    /// 刷新机器Id的存活状态
    /// </summary>
    /// <returns></returns>
    Task RefreshAlive();
}
