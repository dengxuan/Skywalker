using System;

namespace Skywalker.Scheduler.Abstractions
{
    /// <summary>
    /// 表示时间触发器的接口。
    /// </summary>
    public interface ITrigger : IEquatable<ITrigger>
    {
        /// <summary>
        /// 获取触发器的表达式文本。
        /// </summary>
        string Expression { get; }

        /// <summary>
        /// 获取或设置触发器的生效时间。
        /// </summary>
        DateTime? EffectiveTime { get; set; }

        /// <summary>
        /// 获取或设置触发器的截止时间。
        /// </summary>
        DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// 计算触发器的下次触发时间，如果结果为空(null)表示不再触发。
        /// </summary>
        /// <param name="inclusive">指定一个值，本次计算是否包含当前时间点。</param>
        /// <returns>返回下次触发的时间，如果为空(null)则表示不再触发。</returns>
        DateTime? GetNextOccurrence(bool inclusive = false);

        /// <summary>
        /// 计算触发器的下次触发时间，如果结果为空(null)表示不再触发。
        /// </summary>
        /// <param name="origin">指定开始计算的起始时间。</param>
        /// <param name="inclusive">指定一个值，本次计算是否包含<paramref name="origin"/>参数指定的起始时间。</param>
        /// <returns>返回下次触发的时间，如果为空(null)则表示不再触发。</returns>
        DateTime? GetNextOccurrence(DateTime origin, bool inclusive = false);
    }
}
