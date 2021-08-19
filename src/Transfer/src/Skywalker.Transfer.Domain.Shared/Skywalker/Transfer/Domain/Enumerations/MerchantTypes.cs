using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Enumerations
{
    /// <summary>
    /// 商户类型
    /// </summary>
    [Flags]
    public enum MerchantTypes : byte
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Closed = 0x00,

        /// <summary>
        /// 充值
        /// </summary>
        Recharge = 0x01,

        /// <summary>
        /// 提现
        /// </summary>
        Withdraw = 0x02,

        /// <summary>
        /// 全部
        /// </summary>
        Entire = Recharge | Withdraw
    }
}
