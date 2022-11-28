// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.Security.Claims;

public class SkywalkerClaimsPrincipalFactoryOptions
{
    public ITypeList<ISkywalkerClaimsPrincipalContributor> Contributors { get; }

    public SkywalkerClaimsPrincipalFactoryOptions()
    {
        Contributors = new TypeList<ISkywalkerClaimsPrincipalContributor>();
    }
}
