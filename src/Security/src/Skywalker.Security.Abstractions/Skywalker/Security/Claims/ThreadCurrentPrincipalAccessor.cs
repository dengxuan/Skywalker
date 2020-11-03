﻿using System;
using System.Security.Claims;
using System.Threading;

namespace Skywalker.Security.Claims
{
    public class ThreadCurrentPrincipalAccessor : ICurrentPrincipalAccessor
    {
        public ClaimsPrincipal? Principal => _currentPrincipal.Value ?? GetClaimsPrincipal();

        private readonly AsyncLocal<ClaimsPrincipal?> _currentPrincipal = new AsyncLocal<ClaimsPrincipal?>();

        public virtual ClaimsPrincipal? GetClaimsPrincipal()
        {
            return Thread.CurrentPrincipal as ClaimsPrincipal;
        }

        public virtual IDisposable Change(ClaimsPrincipal principal)
        {
            return SetCurrent(principal);
        }

        private IDisposable SetCurrent(ClaimsPrincipal principal)
        {
            var parent = Principal;
            _currentPrincipal.Value = principal;
            return new DisposeAction(() =>
            {
                _currentPrincipal.Value = parent!;
            });
        }
    }
}
