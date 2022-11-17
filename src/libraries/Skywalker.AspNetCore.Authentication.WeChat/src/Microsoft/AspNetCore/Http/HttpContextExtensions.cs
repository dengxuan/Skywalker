// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authentication;
using Skywalker.AspNetCore.Authentication.WeChat;

#if NETSTANDARD
using Newtonsoft.Json.Linq;
#else
using System.Text.Json;
#endif

namespace Microsoft.AspNetCore.Http;


public static class HttpContextExtensions
{
    /// <summary> 
    ///  获取微信登录的扩展信息
    /// </summary> 
    public static async Task<Dictionary<string, string?>?> GetWeChatUserInfoAsync(this HttpContext httpContext)
    {
        var auth = await httpContext.AuthenticateAsync(WeChatAuthenticationDefaults.AuthenticationScheme);

        var items = auth?.Properties?.Items;
        if ((items?.TryGetValue(".AuthScheme", out var scheme)) != true || scheme != WeChatAuthenticationDefaults.AuthenticationScheme)
        {
            return null;
        }

        var userInfo = auth?.Principal?.FindFirst(WeChatClaimTypes.UserInfo);
        if (string.IsNullOrEmpty(userInfo?.Value))
        {
            return null;
        }

        return GetUserInfo(userInfo!.Value);
    }

    private static Dictionary<string, string?> GetUserInfo(string json)
    {
        var userInfos = new Dictionary<string, string?>();

#if NETSTANDARD
        var jObject = JObject.Parse(json);

        foreach (var item in jObject)
        {
            userInfos[item.Key] = item.Value.ToString();
        }
#else
        var document = JsonDocument.Parse(json);

        foreach (var item in document.RootElement.EnumerateObject())
        {
            userInfos[item.Name] = item.Value.GetString();
        }
#endif
        return userInfos;
    }
}
