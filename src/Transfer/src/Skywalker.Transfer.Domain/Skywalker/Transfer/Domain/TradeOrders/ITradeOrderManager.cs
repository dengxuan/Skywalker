using Skywalker.Domain.Entities;
using Skywalker.Domain.Services;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.Merchants;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.TradeOrders
{
    public interface ITradeOrderManager<TUser> : IDomainService where TUser: class, IEntity
    {
        Task<TradeOrder<TUser>> CreateAsync(Merchant merchant, TUser user, decimal amount, decimal handlingFee, decimal withholding, TradeOrderTypes tradeOrderType = TradeOrderTypes.Initial, TradeAuditedTypes tradeAuditedType = TradeAuditedTypes.Auditing);


    }
}
