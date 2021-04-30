using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using System;

namespace Skywalker.Transfer.Domain.UserFundings
{
    /// <summary>
    /// 用户资金
    /// </summary>
    public class TransferDetails : Entity<Guid>
    {
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
        private TransferDetails() { }

        /// <summary>
        /// 创建转账明细
        /// </summary>
        /// <param name="transferType">转账类型</param>
        /// <param name="amount">转账金额</param>
        /// <param name="balance">转账后的余额</param>
        /// <param name="message">转账消息</param>
        /// <exception cref="ArgumentOutOfRangeException">金额<paramref name="amount"/>小于等于0时抛出 <seealso cref="Check.Positive(decimal, string)"/> </exception>
        /// <exception cref="ArgumentOutOfRangeException">余额<paramref name="balance"/>小于等于0时抛出 <seealso cref="Check.Positive(decimal, string)"/></exception>
        internal TransferDetails(TransferTypes transferType, decimal amount, decimal balance, string? message)
        {
            TransferType = transferType;
            Amount = amount.Positive(nameof(amount));
            Balance = balance.Positive(nameof(balance));
            Message = message;
        }
    }
}
