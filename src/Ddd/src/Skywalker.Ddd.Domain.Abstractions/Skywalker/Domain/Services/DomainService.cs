using Skywalker.Caching.Abstractions;

namespace Skywalker.Domain.Services
{
    public abstract class DomainService : IDomainService
    {
        protected readonly ICachingProvider CachingProvider;

        protected DomainService(ICachingProvider cachingProvider)
        {
            CachingProvider = cachingProvider;
        }
    }
}
