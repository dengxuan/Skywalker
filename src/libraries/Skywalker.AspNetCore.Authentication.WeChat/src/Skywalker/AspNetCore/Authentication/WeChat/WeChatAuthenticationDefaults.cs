using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Skywalker.AspNetCore.Authentication.WeChat;

/// <summary>
/// 微信认证的默认值.
/// </summary>
public static class WeChatAuthenticationDefaults
{
    /// <summary>
    /// <see cref="AuthenticationScheme.Name"/> 的默认值.
    /// </summary>
    public const string AuthenticationScheme = "WeChat";

    /// <summary>
    /// <see cref="AuthenticationScheme.DisplayName"/> 的默认值.
    /// </summary>
    public const string DisplayName = "WeChat";

    /// <summary>
    /// <see cref="RemoteAuthenticationOptions.CallbackPath"/> 的默认值.
    /// </summary>
    public const string CallbackPath = "/signin-wechat";

    /// <summary>
    /// <see cref="AuthenticationSchemeOptions.ClaimsIssuer"/> 的默认值.
    /// </summary>
    public const string Issuer = "WeChat";

    /// <summary>
    /// <see cref="OAuthOptions.AuthorizationEndpoint"/> 的默认值.
    /// </summary>
    public const string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/qrconnect";

    /// <summary>
    ///  <see cref="OAuthOptions.TokenEndpoint"/> 的默认值.
    /// </summary>
    public const string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

    /// <summary>
    /// <see cref="OAuthOptions.UserInformationEndpoint"/> 的默认值.
    /// </summary>
    public const string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";
}
