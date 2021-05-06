using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Skywalker.Domain.Entities;
using Skywalker.Transfer.Domain.Enumerations;
using Skywalker.Transfer.Domain.TradeOrders;

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
        [MaxLength(TransferConsts.Validations.MaxMerchantSchemeLength)]
        public string Scheme { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [MaxLength(TransferConsts.Validations.MaxMerchantNumberLength)]
        public string Number { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(TransferConsts.Validations.MaxMerchantNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(TransferConsts.Validations.MaxMerchantDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        [MaxLength(TransferConsts.Validations.MaxMerchantCipherKeyLength)]
        public string CipherKey { get; set; }

        /// <summary>
        /// 下单地址
        /// </summary>
        [MaxLength(TransferConsts.Validations.MaxMerchantAddressLength)]
        public string Address { get; set; }

        /// <summary>
        /// 渠道类型 <see cref="MerchantTypes"/>
        /// </summary>
        public MerchantTypes MerchantType { get; set; }

        /// <summary>
        /// 商户交易订单
        /// </summary>
        public ICollection<TradeOrder> TradeOrders { get; set; }

        /// <summary>
        /// Just for ORM and ObjectMapper
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Merchant() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
        internal Merchant(string scheme, string name, string description, string key, string cipherKey, string address, MerchantTypes merchantType = MerchantTypes.Entire)
        {
            Scheme = scheme.NotNullOrEmpty(nameof(scheme));
            Name = name.NotNullOrEmpty(nameof(name));
            Description = description;
            Number = key;
            CipherKey = cipherKey;
            Address = address.NotNullOrEmpty(nameof(address));
            MerchantType = merchantType;
            TradeOrders = new List<TradeOrder>();
        }

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
        internal Merchant(Guid id, string scheme, string name, string description, string key, string cipherKey, string address, MerchantTypes merchantType = MerchantTypes.Entire) :
            this(scheme, name, description, key, cipherKey, address, merchantType)
        {
            Id = id;
        }
    }
}
