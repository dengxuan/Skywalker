using System;

namespace Skywalker.Scheduler
{
    /// <summary>
    /// 表示处理器执行失败的重试信息。
    /// </summary>
    public struct HandlerFailure
    {
        /// <summary>
        /// 获取执行重试的次数。
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 获取最近一次重试的时间。
        /// </summary>
        public DateTime? Timestamp{ get; }

        /// <summary>
        /// 获取重试的最后期限，如果为空表示无限制。
        /// </summary>
        public DateTime? Expiration{ get; }

        public HandlerFailure(int count, DateTime? timestamp, DateTime? expiration)
        {
            this.Count = count;
            this.Timestamp = timestamp;
            this.Expiration = expiration;
        }
    }
}
