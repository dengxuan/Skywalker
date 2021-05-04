using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.Domain.ApiScopes
{
    public class ApiScopeManager : DomainService
    {
        private readonly IRepository<ApiScope> _apiScopes;

        public ApiScopeManager(IRepository<ApiScope> apiScopes)
        {
            _apiScopes = apiScopes;
        }

        /// <inheritdoc/>
        public Task<List<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }
            return Task.Run(() =>
            {
                return _apiScopes.Where(predicate => scopeNames.Contains(predicate.Name)).ToList();
            });
        }
    }
}
