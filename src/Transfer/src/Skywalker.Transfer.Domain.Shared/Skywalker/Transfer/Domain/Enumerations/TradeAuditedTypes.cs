using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Enumerations
{
    /// <summary>
    /// 订单审计状态
    /// </summary>
    public enum TradeAuditedTypes
    {
        /// <summary>
        /// 自动放行
        /// </summary>
        Autopass,

        /// <summary>
        /// 审核中
        /// </summary>
        Auditing,

        /// <summary>
        /// 同意
        /// </summary>
        Approved,

        /// <summary>
        /// 拒绝
        /// </summary>
        Rejected
    }
}
