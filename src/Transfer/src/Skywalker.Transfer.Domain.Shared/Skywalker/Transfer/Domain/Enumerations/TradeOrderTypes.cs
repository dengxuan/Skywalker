using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Enumerations
{
    public enum TradeOrderTypes : byte
    {
        /// <summary>
        /// 订单初始化
        /// </summary>
        Initial,

        /// <summary>
        /// 交易进行中
        /// </summary>
        Trading,

        /// <summary>
        /// 交易成功
        /// </summary>
        Succeed,

        /// <summary>
        /// 交易失败
        /// </summary>
        Failure,

        /// <summary>
        ///交易撤销
        /// </summary>
        Revoked,

        /// <summary>
        /// 交易超时
        /// </summary>
        Timeout,
    }
}
