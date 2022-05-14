// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.AspNetCore.Authentication.WeChat;

namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static WeChatUser ToWeChatUser(this ClaimsPrincipal principal)
    {
        var unionId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var openId = principal.FindFirst(WeChatClaimTypes.OpenId)?.Value;
        var avatar = principal.FindFirst(WeChatClaimTypes.Avatar)?.Value;
        var nickName = principal.FindFirst(WeChatClaimTypes.Nickname)?.Value;
        var gender = (principal.FindFirst(ClaimTypes.Gender)?.Value) switch
        {
            "0" => Genders.Male,
            "1" => Genders.Female,
            _ => Genders.Unknow,
        };

        return new WeChatUser(unionId!, openId!, avatar!, nickName!, gender);
    }
}
