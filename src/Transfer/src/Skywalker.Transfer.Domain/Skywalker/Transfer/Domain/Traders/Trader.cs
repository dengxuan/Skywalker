using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.TransferDetails;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.TradeUsers
{
    public class Trader : AggregateRoot<Guid>
    {
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 交易账户状态
        /// </summary>
        public TraderTypes TraderType { get; set; }

        /// <summary>
        /// 交易订单
        /// </summary>
        public ICollection<TradeOrder> TradeOrders { get; set; }

        /// <summary>
        /// 转账明细
        /// </summary>
        public ICollection<TransferDetail> TransferDetails { get; set; }

        ///<inheritdoc/>
        internal Trader()
        {
            TradeOrders = new List<TradeOrder>();
            TransferDetails = new List<TransferDetail>();
        }

        ///<inheritdoc/>
        internal Trader(ICollection<TradeOrder>? tradeOrders = null, ICollection<TransferDetail>? transferDetails = null)
        {
            TradeOrders = tradeOrders ?? new List<TradeOrder>();
            TransferDetails = transferDetails ?? new List<TransferDetail>();
        }

        ///<inheritdoc/>
        public Task<decimal> TransferAsync(TransferTypes transferType, decimal amount, string? message)
        {
            if (TraderType == TraderTypes.Locked)
            {
                throw new TraderLockedException();
            }
            return Task.Run(() =>
            {
                Balance = transferType switch
                {
                    TransferTypes.Recharge or TransferTypes.Present or TransferTypes.In => Balance + amount.Positive(nameof(amount)),
                    TransferTypes.Withdraw or TransferTypes.Payment or TransferTypes.Out => Balance - amount.Positive(nameof(amount)),
                    _ => amount.Positive(nameof(amount)),
                };
                TransferDetails.Add(new TransferDetail(this, transferType, amount, Balance, message));
                return Balance;
            });
        }
    }
}
