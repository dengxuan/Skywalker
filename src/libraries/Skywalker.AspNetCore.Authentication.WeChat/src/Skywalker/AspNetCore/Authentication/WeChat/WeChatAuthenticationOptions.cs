using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;

namespace Skywalker.AspNetCore.Authentication.WeChat;

/// <summary>
/// 定义 <see cref="WeChatAuthenticationHandler"/> 使用的一组选项.
/// </summary>
public class WeChatAuthenticationOptions : OAuthOptions
{
    public WeChatAuthenticationOptions()
    {
        ClaimsIssuer = WeChatAuthenticationDefaults.Issuer;
        CallbackPath = WeChatAuthenticationDefaults.CallbackPath;

        AuthorizationEndpoint = WeChatAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = WeChatAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = WeChatAuthenticationDefaults.UserInformationEndpoint;

        Scope.Add("snsapi_login");
        Scope.Add("snsapi_userinfo");

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "unionid");
        ClaimActions.MapJsonKey(ClaimTypes.Gender, "sex");
        ClaimActions.MapJsonKey(ClaimTypes.Country, "country");
        ClaimActions.MapJsonKey(ClaimTypes.StateOrProvince, "province");
        ClaimActions.MapJsonKey(WeChatClaimTypes.Nickname, "nickname");
        ClaimActions.MapJsonKey(WeChatClaimTypes.OpenId, "openid");
        ClaimActions.MapJsonKey(WeChatClaimTypes.Province, "province");
        ClaimActions.MapJsonKey(WeChatClaimTypes.City, "city");
        ClaimActions.MapJsonKey(WeChatClaimTypes.Avatar, "headimgurl");
#if NETSTANDARD
        ClaimActions.MapCustomJson(WeChatClaimTypes.Privilege, user =>
        {

            if (!user.TryGetValue("privilege", out var value) || value.Type != JTokenType.Array)
            {
                return null;
            }
            return string.Join(",", value.AsJEnumerable().Select(element => element.Value<string>()));
        });
#elif NETCOREAPP3_1_OR_GREATER
        ClaimActions.MapCustomJson(WeChatClaimTypes.Privilege, user =>
        {
            if (!user.TryGetProperty("privilege", out var value) || value.ValueKind != JsonValueKind.Array)
            {
                return null;
            }
            return string.Join(",", value.EnumerateArray().Select(element => element.GetString()));
        });
#endif
    }
}
