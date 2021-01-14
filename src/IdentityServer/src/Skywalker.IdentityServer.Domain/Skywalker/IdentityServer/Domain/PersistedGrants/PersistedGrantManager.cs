using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Extensions;
using Skywalker.IdentityServer.Models;
using Skywalker.IdentityServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.Domain.PersistedGrants
{
    public class PersistedGrantManager : DomainService
    {
        private readonly IRepository<PersistedGrant, string> _persistedGrants;

        public PersistedGrantManager(IRepository<PersistedGrant, string> persistedGrants, ICachingProvider cachingProvider) : base(cachingProvider)
        {
            _persistedGrants = persistedGrants;
        }


        /// <inheritdoc/>
        public async Task<PersistedGrant> StoreAsync(PersistedGrant grant)
        {
            return await _persistedGrants.InsertAsync(grant);
        }

        /// <inheritdoc/>
        public async Task<PersistedGrant?> GetAsync(string key)
        {
            return await _persistedGrants.FindAsync(key);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var items = Filter(filter);

            return Task.FromResult(items);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            await _persistedGrants.DeleteAsync(key);
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var items = Filter(filter);
            
            foreach (var item in items)
            {
                await _persistedGrants.DeleteAsync(item);
            }
        }

        private IEnumerable<PersistedGrant> Filter(PersistedGrantFilter filter)
        {
            var persistedGrants = _persistedGrants.Where(predicate => !filter.ClientId.IsNullOrWhiteSpace() && predicate.ClientId == filter.ClientId)
                                                  .Where(predicate => !filter.SessionId.IsNullOrWhiteSpace() && predicate.SessionId == filter.SessionId)
                                                  .Where(predicate => !filter.SubjectId.IsNullOrWhiteSpace() && predicate.SessionId == filter.SessionId)
                                                  .Where(predicate => !filter.Type.IsNullOrWhiteSpace() && predicate.SessionId == filter.SessionId)
                                                  .ToList();
            return persistedGrants;
        }
    }
}
