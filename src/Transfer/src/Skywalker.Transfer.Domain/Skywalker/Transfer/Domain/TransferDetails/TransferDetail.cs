using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.TradeUsers;
using System;

namespace Skywalker.Transfer.Domain.TransferDetails
{
    /// <summary>
    /// 用户资金
    /// </summary>
    public class TransferDetail : Entity<Guid>
    {
        public Trader Trader { get; set; }

        /// <summary>
        /// 转账类型
        /// </summary>
        public TransferTypes TransferType { get; set; }

        /// <summary>
        /// 转账金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 资金类型
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Just for ORM and ObjectMapper
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private TransferDetail() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// 创建转账明细
        /// </summary>
        /// <param name="transferType">转账类型</param>
        /// <param name="amount">转账金额</param>
        /// <param name="balance">转账后的余额</param>
        /// <param name="message">转账消息</param>
        /// <exception cref="ArgumentOutOfRangeException">金额<paramref name="amount"/>小于等于0时抛出 <seealso cref="Check.Positive(decimal, string)"/> </exception>
        /// <exception cref="ArgumentOutOfRangeException">余额<paramref name="balance"/>小于等于0时抛出 <seealso cref="Check.Positive(decimal, string)"/></exception>
        internal TransferDetail(Trader trader, TransferTypes transferType, decimal amount, decimal balance, string? message)
        {
            Trader = trader.NotNull(nameof(trader));
            TransferType = transferType;
            Amount = amount.Positive(nameof(amount));
            Balance = balance.Positive(nameof(balance));
            Message = message;
        }
    }
}
