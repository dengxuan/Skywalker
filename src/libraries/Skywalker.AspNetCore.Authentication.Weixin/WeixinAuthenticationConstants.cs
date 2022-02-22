namespace Skywalker.AspNetCore.Authentication.Weixin
{
    /// <summary>
    /// <see cref="WeixinAuthenticationHandler"/> 的常量定义.
    /// </summary>
    public static class WeixinAuthenticationConstants
    {
        public static class Claims
        {
            public const string City = "urn:weixin:city";
            public const string HeadImgUrl = "urn:weixin:headimgurl";
            public const string OpenId = "urn:weixin:openid";
            public const string Privilege = "urn:weixin:privilege";
            public const string Province = "urn:weixin:province";
        }
    }
}
