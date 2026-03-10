using Skywalker.DependencyInjection;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.VirtualFiles;

namespace Skywalker.Extensions.Emailing.Template;

/// <summary>
/// Provides email template definitions.
/// </summary>
public class EmailTemplateDefinitionProvider : TemplateDefinitionProvider, ITransientDependency
{
    public override void Define(ITemplateDefinitionContext context)
    {
        // Layout template
        context.Add(new TemplateDefinition(EmailTemplateNames.Layout)
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/Layout.tpl", isInlineLocalized: false));

        // Welcome email
        context.Add(new TemplateDefinition($"{EmailTemplateNames.Welcome}.Subject")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/Welcome.Subject.tpl", isInlineLocalized: false));
        context.Add(new TemplateDefinition($"{EmailTemplateNames.Welcome}.Body")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/Welcome.Body.tpl", isInlineLocalized: false));

        // Password reset email
        context.Add(new TemplateDefinition($"{EmailTemplateNames.ResetPassword}.Subject")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/ResetPassword.Subject.tpl", isInlineLocalized: false));
        context.Add(new TemplateDefinition($"{EmailTemplateNames.ResetPassword}.Body")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/ResetPassword.Body.tpl", isInlineLocalized: false));

        // Email verification
        context.Add(new TemplateDefinition($"{EmailTemplateNames.VerifyEmail}.Subject")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/VerifyEmail.Subject.tpl", isInlineLocalized: false));
        context.Add(new TemplateDefinition($"{EmailTemplateNames.VerifyEmail}.Body")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/VerifyEmail.Body.tpl", isInlineLocalized: false));

        // Notification email
        context.Add(new TemplateDefinition($"{EmailTemplateNames.Notification}.Subject")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/Notification.Subject.tpl", isInlineLocalized: false));
        context.Add(new TemplateDefinition($"{EmailTemplateNames.Notification}.Body")
            .WithVirtualFilePath("/Skywalker/Extensions/Emailing/Template/Templates/Notification.Body.tpl", isInlineLocalized: false));
    }
}

