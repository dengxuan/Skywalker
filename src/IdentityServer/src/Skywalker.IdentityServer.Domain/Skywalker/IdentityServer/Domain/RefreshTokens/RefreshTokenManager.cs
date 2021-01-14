﻿using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Models;

namespace Skywalker.IdentityServer.Domain.RefreshTokens
{
    public class RefreshTokenManager : DomainService
    {
        private readonly IRepository<RefreshToken> _refreshTokens;

        public RefreshTokenManager(IRepository<RefreshToken> refreshTokens, ICachingProvider cachingProvider) : base(cachingProvider)
        {
            _refreshTokens = refreshTokens;
        }


    }
}
