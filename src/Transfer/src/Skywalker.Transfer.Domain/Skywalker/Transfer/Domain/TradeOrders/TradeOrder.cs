using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeUsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Transfer.Domain.TradeOrders
{
    /// <summary>
    /// 交易订单
    /// </summary>
    public class TradeOrder : Entity<Guid>
    {
        /// <summary>
        /// 订单所属商户
        /// </summary>
        [NotNull]
        public Merchant Merchant { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [NotNull]
        public Trader Trader { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        [NotNull]
        public decimal Amount { get; set; }

        /// <summary>
        /// 交易手续费
        /// </summary>
        [NotNull]
        public decimal HandlingFee { get; set; }

        /// <summary>
        /// 代扣用户手续费
        /// </summary>
        [NotNull]
        public decimal Withholding { get; set; }

        /// <summary>
        /// 订单类型<see cref="TradeOrderTypes"/>
        /// </summary>
        [NotNull]
        public TradeOrderTypes TradeOrderType { get; set; }

        /// <summary>
        /// 订单审计状态<see cref="TradeAuditedTypes"/>
        /// </summary>
        [NotNull]
        public TradeAuditedTypes TradeAuditedType { get; set; }

        /// <summary>
        /// 撤单时间
        /// </summary>
        [NotNull]
        public DateTime? RevokeTime { get; set; }

        /// <summary>
        /// 撤销原因
        /// </summary>
        [MaybeNull]
        [MaxLength(TransferConsts.Validations.MaxTradeOrderRevokeReasonLength)]
        public string? RevokeReason { get; set; }

        /// <summary>
        /// Just for ORM and ObjectMapping
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private TradeOrder() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// 初始化订单
        /// </summary>
        /// <param name="id">订单编号</param>
        /// <param name="merchant">商户</param>
        /// <param name="trader">用户</param>
        /// <param name="amount">订单金额</param>
        /// <param name="handlingFee">交易手续费</param>
        /// <param name="withholding">代扣手续费</param>
        /// <param name="tradeOrderType">订单类型</param>
        /// <param name="tradeAuditedType">审核类型</param>
        /// <param name="revokeTime">撤单时间</param>
        /// <param name="revokeReason">撤单原因</param>
        internal TradeOrder(Guid id, Merchant merchant, Trader trader, decimal amount, decimal handlingFee, decimal withholding, TradeOrderTypes tradeOrderType = TradeOrderTypes.Initial, TradeAuditedTypes tradeAuditedType = TradeAuditedTypes.Auditing, DateTime? revokeTime = null, string? revokeReason = null)
        {
            Id = id;
            Merchant = merchant.NotNull(nameof(merchant));
            Trader = trader.NotNull(nameof(trader));
            Amount = amount;
            HandlingFee = handlingFee;
            Withholding = withholding;
            TradeOrderType = tradeOrderType;
            TradeAuditedType = tradeAuditedType;
            RevokeTime = revokeTime;
            RevokeReason = revokeReason;
        }
    }
}
