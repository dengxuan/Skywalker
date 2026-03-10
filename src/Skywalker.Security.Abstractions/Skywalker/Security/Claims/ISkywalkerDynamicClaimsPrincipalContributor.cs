// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Security.Claims;

public interface ISkywalkerDynamicClaimsPrincipalContributor
{
    Task ContributeAsync(SkywalkerClaimsPrincipalContributorContext context);
}
