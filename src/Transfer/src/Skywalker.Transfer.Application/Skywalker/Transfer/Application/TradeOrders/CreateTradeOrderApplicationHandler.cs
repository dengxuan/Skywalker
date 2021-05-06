using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Transfer.Application.TradeOrders.Dtos;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.Traders;
using Skywalker.Transfer.Domain.TradeUsers;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Application.TradeOrders
{
    public class CreateTradeOrderApplicationHandler : IApplicationHandler<CreateTradeOrderInputDto, TradeOrderOutputDto>
    {
        private readonly IObjectMapper _objectMapper;
        private readonly ITraderManager _traderManager;
        private readonly IMerchantManager _merchantManager;
        private readonly ITradeOrderManager<Trader> _tradeOrderManager;

        public CreateTradeOrderApplicationHandler(ITradeOrderManager<Trader> tradeOrderManager, IMerchantManager merchantManager, ITraderManager traderManager, IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
            _traderManager = traderManager;
            _merchantManager = merchantManager;
            _tradeOrderManager = tradeOrderManager;
        }

        public async Task<TradeOrderOutputDto?> HandleAsync(CreateTradeOrderInputDto inputDto, CancellationToken cancellationToken = default)
        {
            Merchant merchant = await _merchantManager.GetAsync(inputDto.MerchantId);
            Trader trader = await _traderManager.GetAsync(inputDto.MerchantId);
            TradeOrder tradeOrder = await _tradeOrderManager.CreateAsync(merchant, trader, inputDto.Amount, 0, 0);
            return _objectMapper.Map<TradeOrder,TradeOrderOutputDto>(tradeOrder);
        }
    }
}
