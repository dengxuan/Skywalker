namespace Skywalker.Sample.Website.Application;

public static class WebsitePermissions
{
    public const string ManageProducts = "Website.Products.Manage";
}

public static class WebsiteSettings
{
    /// <summary>免运费门槛（元）。</summary>
    public const string FreeShippingThreshold = "Website.FreeShippingThreshold";

    /// <summary>站点显示名。</summary>
    public const string SiteName = "Website.SiteName";
}

public static class WebsiteTemplates
{
    public const string VerificationEmail = "Website.VerificationEmail";
    public const string OrderConfirmedEmail = "Website.OrderConfirmedEmail";
}

/// <summary>本地化资源标记类型（资源内容在 Web 项目的 Localization/*.json）。</summary>
public class WebsiteResource
{
}
