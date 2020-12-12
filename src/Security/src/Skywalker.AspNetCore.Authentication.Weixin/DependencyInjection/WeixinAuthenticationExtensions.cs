using Microsoft.AspNetCore.Authentication;
using Skywalker.AspNetCore.Authentication.Weixin;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 向HTTP应用程序添加Weixin身份验证功能的扩展方法.
    /// </summary>
    public static class WeixinAuthenticationExtensions
    {
        /// <summary>
        /// 将 <see cref="WeixinAuthenticationHandler"/> 添加到指定的
        /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
        /// </summary>
        /// <param name="builder">认证建造器.</param>
        /// <returns>当操作完成后返回当前引用的实例.</returns>
        public static AuthenticationBuilder AddWeixin([NotNull] this AuthenticationBuilder builder)
        {
            return builder.AddWeixin(WeixinAuthenticationDefaults.AuthenticationScheme, options => { });
        }

        /// <summary>
        /// 将 <see cref="WeixinAuthenticationHandler"/> 添加到指定的
        /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
        /// </summary>
        /// <param name="builder">认证建造器.</param>
        /// <param name="configuration">OpenID 2.0的选项配置委托</param>
        /// <returns>当操作完成后返回当前引用的实例.</returns>
        public static AuthenticationBuilder AddWeixin(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] Action<WeixinAuthenticationOptions> configuration)
        {
            return builder.AddWeixin(WeixinAuthenticationDefaults.AuthenticationScheme, configuration);
        }

        /// <summary>
        /// 将 <see cref="WeixinAuthenticationHandler"/> 添加到指定的
        /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
        /// </summary>
        /// <param name="builder">认证建造器.</param>
        /// <param name="scheme">与当前实例关联的身份认证方案.</param>
        /// <param name="configuration">微信选项配置委托.</param>
        /// <returns> <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddWeixin(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string scheme,
            [NotNull] Action<WeixinAuthenticationOptions> configuration)
        {
            return builder.AddWeixin(scheme, WeixinAuthenticationDefaults.DisplayName, configuration);
        }

        /// <summary>
        /// 将 <see cref="WeixinAuthenticationHandler"/> 添加到指定的
        /// <see cref="AuthenticationBuilder"/>, 它启用微信认证.
        /// </summary>
        /// <param name="builder">认证建造器.</param>
        /// <param name="scheme">与当前实例关联的身份认证方案.</param>
        /// <param name="caption">与当前实例相关联的可选显示名.</param>
        /// <param name="configuration">微信选项配置委托.</param>
        /// <returns><see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddWeixin(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string scheme,
            string caption,
            [NotNull] Action<WeixinAuthenticationOptions> configuration)
        {
            return builder.AddOAuth<WeixinAuthenticationOptions, WeixinAuthenticationHandler>(scheme, caption, configuration);
        }
    }
}
