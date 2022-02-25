﻿using System.Security.Claims;

namespace Skywalker.Security.Claims;

public interface ICurrentPrincipalAccessor
{
    ClaimsPrincipal? Principal { get; }

    IDisposable Change(ClaimsPrincipal principal);
}