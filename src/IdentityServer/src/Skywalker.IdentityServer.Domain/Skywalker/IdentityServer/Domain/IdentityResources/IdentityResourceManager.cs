using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;

namespace Skywalker.IdentityServer.Domain.IdentityResources
{
    public class IdentityResourceManager : DomainService
    {
        private readonly IRepository<IdentityResource> _identityResources;

        public IdentityResourceManager(IRepository<IdentityResource> identityResources)
        {
            _identityResources = identityResources;
        }

        /// <inheritdoc/>
        public Task<List<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }
            return Task.Run(() =>
            {
                return _identityResources.Where(predicate => scopeNames.Contains(predicate.Name)).ToList();
            });
        }
    }
}
