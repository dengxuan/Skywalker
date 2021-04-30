using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.UserFundings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.TradeUsers
{
    public class Trader : AggregateRoot<Guid>, ITrader
    {
        ///<inheritdoc/>
        public decimal Balance { get; set; }

        ///<inheritdoc/>
        public TraderTypes TraderType { get; set; }

        ///<inheritdoc/>
        public ICollection<TradeOrder<ITrader>> TradeOrders { get; set; }

        ///<inheritdoc/>
        public ICollection<TransferDetails> TransferDetails { get; set; }

        ///<inheritdoc/>
        internal Trader()
        {
            TradeOrders = new List<TradeOrder<ITrader>>();
            TransferDetails = new List<TransferDetails>();
        }

        ///<inheritdoc/>
        internal Trader(ICollection<TradeOrder<ITrader>>? tradeOrders = null, ICollection<TransferDetails>? transferDetails = null)
        {
            TradeOrders = tradeOrders ?? new List<TradeOrder<ITrader>>();
            TransferDetails = transferDetails ?? new List<TransferDetails>();
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
                TransferDetails.Add(new TransferDetails(this, transferType, amount, Balance, message));
                return Balance;
            });
        }
    }
}
