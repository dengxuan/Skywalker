using Microsoft.Extensions.DependencyInjection;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.Domain.ApiResources
{
    public class ApiResoueceManager : DomainService
    {
        private readonly IRepository<ApiResource> _apiResources;

        public ApiResoueceManager(IRepository<ApiResource> apiResources)
        {
            _apiResources = apiResources;
        }

        /// <inheritdoc/>
        public Task<List<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null)
            {
                throw new ArgumentNullException(nameof(apiResourceNames));
            }
            var query = _apiResources.Where(predicate => apiResourceNames.Contains(predicate.Name));
            return Task.FromResult(query.ToList());
        }

        /// <inheritdoc/>
        public Task<List<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }
            var query = _apiResources.Where(predicate => predicate.Scopes.Any(x => scopeNames.Contains(x)));
            return Task.FromResult(query.ToList());
        }
    }
}
