using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;
using Skywalker.Sample.Website.Application;
using Skywalker.Settings;
using Skywalker.Settings.Abstractions;
using Skywalker.Template;
using Skywalker.Template.Abstractions;

namespace Skywalker.Sample.Website.Web.Infrastructure;

/// <summary>站点权限定义。</summary>
public sealed class WebsitePermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public void Define(PermissionDefinitionContext context)
    {
        context.AddPermission(WebsitePermissions.ManageProducts, "管理商品");
    }
}

/// <summary>站点设置定义（默认值即 DefaultValueSettingValueProvider 的取值来源）。</summary>
public sealed class WebsiteSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(new SettingDefinition(WebsiteSettings.FreeShippingThreshold, defaultValue: "99"));
        context.Add(new SettingDefinition(WebsiteSettings.SiteName, defaultValue: "Skywalker Shop"));
    }
}

/// <summary>
/// 站点邮件模板定义：模板内容是站点资产（嵌入资源），
/// 用什么模板、什么内容完全由站点决定，框架只提供渲染管线。
/// </summary>
public sealed class WebsiteTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        // 单文件模板必须 isInlineLocalized: true——内容提供器只在 inline-localized 模式下
        // 才会用 culture 无关方式读取单文件内容
        context.Add(new TemplateDefinition(WebsiteTemplates.VerificationEmail, typeof(WebsiteResource))
            .WithVirtualFilePath("/Templates/VerificationEmail.tpl", isInlineLocalized: true));

        context.Add(new TemplateDefinition(WebsiteTemplates.OrderConfirmedEmail, typeof(WebsiteResource))
            .WithVirtualFilePath("/Templates/OrderConfirmedEmail.tpl", isInlineLocalized: true));
    }
}
