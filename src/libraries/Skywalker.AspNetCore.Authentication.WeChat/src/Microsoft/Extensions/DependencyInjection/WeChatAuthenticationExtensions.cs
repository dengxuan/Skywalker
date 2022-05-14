using Microsoft.AspNetCore.Authentication;
using Skywalker.AspNetCore.Authentication.WeChat;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 向HTTP应用程序添加Weixin身份验证功能的扩展方法.
/// </summary>
public static class WeChatAuthenticationExtensions
{
    /// <summary>
    /// 将 <see cref="WeChatAuthenticationHandler"/> 添加到指定的
    /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
    /// </summary>
    /// <param name="builder">认证建造器.</param>
    /// <returns>当操作完成后返回当前引用的实例.</returns>
    public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder)
    {
        return builder.AddWeChat(WeChatAuthenticationDefaults.AuthenticationScheme, options => { });
    }

    /// <summary>
    /// 将 <see cref="WeChatAuthenticationHandler"/> 添加到指定的
    /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
    /// </summary>
    /// <param name="builder">认证建造器.</param>
    /// <param name="options">OpenID 2.0的选项配置委托</param>
    /// <returns>当操作完成后返回当前引用的实例.</returns>
    public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, Action<WeChatAuthenticationOptions> options)
    {
        return builder.AddWeChat(WeChatAuthenticationDefaults.AuthenticationScheme, options);
    }

    /// <summary>
    /// 将 <see cref="WeChatAuthenticationHandler"/> 添加到指定的
    /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
    /// </summary>
    /// <param name="builder">认证建造器.</param>
    /// <param name="scheme">与当前实例关联的身份认证方案.</param>
    /// <param name="options">微信选项配置委托.</param>
    /// <returns> <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string scheme, Action<WeChatAuthenticationOptions> options)
    {
        return builder.AddWeChat(scheme, WeChatAuthenticationDefaults.DisplayName, options);
    }

    /// <summary>
    /// 将 <see cref="WeChatAuthenticationHandler"/> 添加到指定的
    /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
    /// </summary>
    /// <param name="builder">认证建造器.</param>
    /// <param name="scheme">与当前实例关联的身份认证方案.</param>
    /// <param name="caption">与当前实例相关联的可选显示名.</param>
    /// <param name="options">微信选项配置委托.</param>
    /// <returns><see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string scheme, string caption, Action<WeChatAuthenticationOptions> options)
    {
        return builder.AddOAuth<WeChatAuthenticationOptions, WeChatAuthenticationHandler>(scheme, caption, options);
    }
}
