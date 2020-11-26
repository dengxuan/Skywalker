using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
