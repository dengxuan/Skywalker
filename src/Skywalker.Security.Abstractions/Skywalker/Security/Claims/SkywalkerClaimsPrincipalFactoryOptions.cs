// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.Security.Claims;

public class SkywalkerClaimsPrincipalFactoryOptions
{
    public List<string> DynamicClaims { get; }

    public Dictionary<string, List<string>> ClaimsMap { get; set; }

    public ITypeList<ISkywalkerClaimsPrincipalContributor> Contributors { get; }

    public ITypeList<ISkywalkerDynamicClaimsPrincipalContributor> DynamicContributors { get; }

    public SkywalkerClaimsPrincipalFactoryOptions()
    {
        Contributors = new TypeList<ISkywalkerClaimsPrincipalContributor>();
        DynamicContributors = new TypeList<ISkywalkerDynamicClaimsPrincipalContributor>();

        DynamicClaims =
        [
            SkywalkerClaimTypes.UserName,
            SkywalkerClaimTypes.NickName,
            SkywalkerClaimTypes.Role,
            SkywalkerClaimTypes.Email,
            SkywalkerClaimTypes.EmailVerified,
            SkywalkerClaimTypes.PhoneNumber,
            SkywalkerClaimTypes.PhoneNumberVerified
        ];

        ClaimsMap = new Dictionary<string, List<string>>()
        {
            { SkywalkerClaimTypes.UserName, new List<string> { "preferred_username", "unique_name", ClaimTypes.Name }},
            { SkywalkerClaimTypes.NickName, new List<string> { "nickname", ClaimTypes.Surname }},
            { SkywalkerClaimTypes.Role, new List<string> { "role", "roles", ClaimTypes.Role }},
            { SkywalkerClaimTypes.Email, new List<string> { "email", ClaimTypes.Email }},
        };
    }
}
