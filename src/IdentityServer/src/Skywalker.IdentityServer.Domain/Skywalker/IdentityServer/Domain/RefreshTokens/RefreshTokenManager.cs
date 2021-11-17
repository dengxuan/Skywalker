using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;

namespace Skywalker.IdentityServer.Domain.RefreshTokens
{
    public class RefreshTokenManager : DomainService
    {
        private readonly IRepository<RefreshToken> _refreshTokens;

        public RefreshTokenManager(IRepository<RefreshToken> refreshTokens)
        {
            _refreshTokens = refreshTokens;
        }


    }
}
