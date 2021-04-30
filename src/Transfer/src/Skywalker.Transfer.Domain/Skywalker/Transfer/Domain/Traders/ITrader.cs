using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.UserFundings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.TradeUsers
{
    public interface ITrader : IAggregateRoot<Guid>
    {
        /// <summary>
        /// 余额
        /// </summary>
        decimal Balance { get; set; }

        /// <summary>
        /// 交易账户状态
        /// </summary>
        TraderTypes TraderType { get; set; }

        /// <summary>
        /// 交易订单
        /// </summary>
        ICollection<TradeOrder<ITrader>> TradeOrders { get; set; }

        /// <summary>
        /// 转账明细
        /// </summary>
        ICollection<TransferDetails> TransferDetails { get; set; }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="transferType">转账类型</param>
        /// <param name="amount">金额</param>
        /// <param name="message">转账说明</param>
        /// <exception cref="TraderLockedException">账户锁定时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">金额<paramref name="amount"/>小于等于0时抛出 <seealso cref="Check.Positive(decimal, string)"/> </exception>
        /// <returns></returns>
        Task<decimal> TransferAsync(TransferTypes transferType, decimal amount, string? message);
    }
}
