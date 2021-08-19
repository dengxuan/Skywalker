using Skywalker.Domain.Services;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeUsers;
using System;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.TradeOrders
{
    public interface ITradeOrderManager<TUser> : IDomainService
    {
        Task<TradeOrder> CreateAsync(Merchant merchant, TUser user, decimal amount, decimal handlingFee, decimal withholding, TradeOrderTypes tradeOrderType = TradeOrderTypes.Initial, TradeAuditedTypes tradeAuditedType = TradeAuditedTypes.Auditing);

        /// <summary>
        /// 交易进行中
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns>订单</returns>
        Task<TradeOrder> Trading(Guid id);

        /// <summary>
        /// 交易成功
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns>订单</returns>
        Task<TradeOrder> Succeed(Guid id);

        /// <summary>
        /// 交易失败
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns>订单</returns>
        Task<TradeOrder> Failure(Guid id);

        /// <summary>
        /// 交易取消
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns>订单</returns>
        Task<TradeOrder> Revoked(Guid id);

        /// <summary>
        /// 交易超时
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns>订单</returns>
        Task<TradeOrder> Timeout(Guid id);

        /// <summary>
        /// 更新交易状态
        /// </summary>
        /// <param name="id">订单号</param>
        /// <param name="tradeOrderType">订单类型</param>
        /// <returns>订单</returns>
        Task<TradeOrder> ChangeAsync(Guid id, TradeOrderTypes tradeOrderType);

    }
}
