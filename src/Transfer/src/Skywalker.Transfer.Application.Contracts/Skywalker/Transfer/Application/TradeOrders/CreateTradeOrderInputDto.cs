using Skywalker.Application.Dtos;
using System;

namespace Skywalker.Transfer.Application.TradeOrders.Dtos
{
    public class CreateTradeOrderInputDto : EntityDto
    {
        public Guid MerchantId { get; set; }

        public Guid TraderId { get; set; }

        public decimal Amount { get; set; }

    }
}
