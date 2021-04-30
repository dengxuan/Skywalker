using Microsoft.EntityFrameworkCore;
using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.Transfer.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Merchants
{
    public class MerchantManager : DomainService<Merchant>, IMerchantManager
    {

        public MerchantManager(IRepository<Merchant, Guid> merchants, ICachingProvider cachingProvider) : base(merchants, cachingProvider)
        {
        }

        public Task<Merchant> CreateAsync(string scheme, string name, string description, string key, string cipherKey, string address, string notifyAddress, MerchantTypes merchantType = MerchantTypes.Entire)
        {
            Merchant merchant = new(Guid.NewGuid(), scheme, name, description, key, cipherKey, address, notifyAddress, merchantType);
            return Repository.InsertAsync(merchant);
        }

        public Task<List<Merchant>> FindAsync(string scheme)
        {
            return Repository.Where(predicate => predicate.Scheme == scheme).ToListAsync();
        }

        public Task<List<Merchant>> FindAsync(MerchantTypes merchantType)
        {
            return Repository.Where(predicate => predicate.MerchantType == merchantType).ToListAsync();
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name, string description)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            merchant.Description = description;
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name, string description, string key)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            merchant.Description = description;
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            merchant.Description = description;
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            merchant.CipherKey = chipherKey.NotNullOrEmpty(nameof(chipherKey));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey, string address)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            merchant.Description = description;
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            merchant.CipherKey = chipherKey.NotNullOrEmpty(nameof(chipherKey));
            merchant.Address = address.NotNullOrEmpty(nameof(address));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey, string address, string notifyAddress)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            merchant.Description = description;
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            merchant.CipherKey = chipherKey.NotNullOrEmpty(nameof(chipherKey));
            merchant.Address = address.NotNullOrEmpty(nameof(address));
            merchant.NotifyAddress = notifyAddress.NotNullOrEmpty(nameof(notifyAddress));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModifyAsync(Guid id, string name, string description, string key, string chipherKey, string address, string notifyAddress, MerchantTypes merchantType)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Name = name.NotNullOrEmpty(nameof(name));
            merchant.Description = description;
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            merchant.CipherKey = chipherKey.NotNullOrEmpty(nameof(chipherKey));
            merchant.Address = address.NotNullOrEmpty(nameof(address));
            merchant.NotifyAddress = notifyAddress.NotNullOrEmpty(nameof(notifyAddress));
            merchant.MerchantType = merchantType;
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityAddressAsync(Guid id, string address)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Address = address.NotNullOrEmpty(nameof(address));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityAddressAsync(Guid id, string address, string notifyAddress)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Address = address.NotNullOrEmpty(nameof(address));
            merchant.NotifyAddress = notifyAddress.NotNullOrEmpty(nameof(notifyAddress));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityChipherKeyAsync(Guid id, string chipherKey)
        {
            Merchant merchant = await GetAsync(id);
            merchant.CipherKey = chipherKey.NotNullOrEmpty(nameof(chipherKey));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityKeyAsync(Guid id, string key)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityKeyAsync(Guid id, string key, string chipherKey)
        {
            Merchant merchant = await GetAsync(id);
            merchant.Key = key.NotNullOrEmpty(nameof(key));
            merchant.CipherKey = chipherKey.NotNullOrEmpty(nameof(chipherKey));
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityMerchantTypeAsync(Guid id, MerchantTypes merchantType)
        {
            Merchant merchant = await GetAsync(id);
            merchant.MerchantType = merchantType;
            return await Repository.UpdateAsync(merchant);
        }

        public async Task<Merchant> ModityNotifyAddressAsync(Guid id, string notifyAddress)
        {
            Merchant merchant = await GetAsync(id);
            merchant.NotifyAddress = notifyAddress.NotNullOrEmpty(nameof(notifyAddress));
            return await Repository.UpdateAsync(merchant);
        }
    }
}
