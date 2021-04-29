using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.TradeOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Merchants
{
    /// <summary>
    /// 支付商户
    /// </summary>
    public class Merchant : AggregateRoot<Guid>
    {
        /// <summary>
        /// 商户标识
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// 商户Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        public string CipherKey { get; set; }

        /// <summary>
        /// 下单地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        public string NotifyAddress { get; set; }

        /// <summary>
        /// 渠道类型 <see cref="MerchantTypes"/>
        /// </summary>
        public MerchantTypes MerchantType { get; set; }

        /// <summary>
        /// 商户交易订单
        /// </summary>
        public ICollection<TradeOrder<Entity>> TradeOrders { get; set; }

        /// <summary>
        /// 构造一个商户
        /// </summary>
        /// <param name="id">Id <see cref="Entity{TKey}.Id"/></param>
        /// <param name="scheme">商户标识</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户Key，可为空</param>
        /// <param name="cipherKey">商户密钥，可为空</param>
        /// <param name="address">商户下单地址</param>
        /// <param name="notifyAddress">商户通知(回调)地址</param>
        internal Merchant(Guid id, string scheme, string name, string description, string key, string cipherKey, string address, string notifyAddress, MerchantTypes merchantType = MerchantTypes.Entire)
        {
            Id = id;
            Scheme = scheme.NotNullOrEmpty(nameof(scheme));
            Name = name.NotNullOrEmpty(nameof(name));
            Description = description;
            Key = key;
            CipherKey = cipherKey;
            Address = address.NotNullOrEmpty(nameof(address));
            NotifyAddress = notifyAddress.NotNullOrEmpty(notifyAddress);
            MerchantType = merchantType;
            TradeOrders = new List<TradeOrder<Entity>>();
        }
    }
}
