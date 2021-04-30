using Skywalker.Domain.Entities;
using Skywalker.Domain.Services;
using Skywalker.Transfer.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Merchants
{
    public interface IMerchantManager : IDomainService<Merchant>
    {
        /// <summary>
        /// 创建商户,
        /// </summary>
        /// <param name="scheme">商户标识</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户账户</param>
        /// <param name="cipherKey">商户密钥</param>
        /// <param name="address">商户地址</param>
        /// <param name="notifyAddress">商户通知地址</param>
        /// <param name="merchantType">商户类型</param>
        /// <exception cref="EntityAlreadyExistedException">商户已经存在时抛出，商户scheme + key同时存在，则系统判断商户已存在</exception>
        /// <exception cref="ArgumentNullException">若scheme, name, address, notifyAddress中任意一项为空则抛出该异常</exception>
        /// <returns>若创建成功，则返回商户<see cref="Merchant"/></returns>
        Task<Merchant> CreateAsync(string scheme, string name, string description, string key, string cipherKey, string address, string notifyAddress, MerchantTypes merchantType = MerchantTypes.Entire);

        /// <summary>
        /// 通过商户标识查询商户列表
        /// </summary>
        /// <param name="scheme">商户标识</param>
        /// <exception cref="ArgumentNullException">scheme为空时抛出</exception>
        /// <returns>与此标识相关的所有商户<see cref="Merchant"/></returns>
        Task<List<Merchant>> FindAsync(string scheme);

        /// <summary>
        /// 通过商户类型查询商户列表
        /// </summary>
        /// <param name="scheme">商户标识</param>
        /// <exception cref="ArgumentNullException">scheme为空时抛出</exception>
        /// <returns>与此标识相关的所有商户<see cref="Merchant"/></returns>
        Task<List<Merchant>> FindAsync(MerchantTypes merchantType);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name, string description);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户账户</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name, string description, string key);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户账户</param>
        /// <param name="chipherKey">密钥</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户账户</param>
        /// <param name="chipherKey">密钥</param>
        /// <param name="address">商户下单地址</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey, string address);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户账户</param>
        /// <param name="chipherKey">密钥</param>
        /// <param name="address">商户下单地址</param>
        /// <param name="notifyAddress">商户通知地址</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey, string address, string notifyAddress);

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="name">商户名称</param>
        /// <param name="description">商户描述</param>
        /// <param name="key">商户账户</param>
        /// <param name="chipherKey">密钥</param>
        /// <param name="address">商户下单地址</param>
        /// <param name="notifyAddress">商户通知地址</param>
        /// <param name="merchantType">商户类型</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey, string address, string notifyAddress, MerchantTypes merchantType);

        /// <summary>
        /// 更新商户
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="key">商户账户</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityKeyAsync(Guid id, string key);

        /// <summary>
        /// 更新商户
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="key">商户账户</param>
        /// <param name="chipherKey">商户密钥</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <exception cref="EntityAlreadyExistedException">更新后的商户Key <see cref="Merchant.Scheme"/>, <see cref="Merchant.Key"/>已存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityKeyAsync(Guid id, string key, string chipherKey);

        /// <summary>
        /// 更新商户密钥
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="chipherKey">商户密钥</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityChipherKeyAsync(Guid id, string chipherKey);

        /// <summary>
        /// 更新商户地址
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="address">商户下单地址</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityAddressAsync(Guid id, string address);

        /// <summary>
        /// 更新商户地址
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="address">商户下单地址</param>
        /// <param name="notifyAddress">商户通知地址</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityAddressAsync(Guid id, string address, string notifyAddress);

        /// <summary>
        /// 更新商户通知地址
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="notifyAddress">商户通知地址</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityNotifyAddressAsync(Guid id, string notifyAddress);

        /// <summary>
        /// 更新商户类型
        /// </summary>
        /// <param name="id">商户Id</param>
        /// <param name="merchantType">商户类型</param>
        /// <exception cref="EntityNotFoundException">商户不存在时抛出</exception>
        /// <returns>更新后的商户<see cref="Merchant"/></returns>
        Task<Merchant> ModityMerchantTypeAsync(Guid id, MerchantTypes merchantType);
    }
}
