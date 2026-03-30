using System.Net.Mail;
using Microsoft.Extensions.Options;
using Skywalker.Template.Abstractions;

namespace Skywalker.Extensions.Emailing.Template;

/// <summary>
/// Default implementation of <see cref="ITemplateEmailSender"/>.
/// </summary>
public class TemplateEmailSender : ITemplateEmailSender
{
    protected IEmailSender EmailSender { get; }
    protected ITemplateRenderer TemplateRenderer { get; }
    protected EmailTemplateOptions Options { get; }

    public TemplateEmailSender(
        IEmailSender emailSender,
        ITemplateRenderer templateRenderer,
        IOptions<EmailTemplateOptions> options)
    {
        EmailSender = emailSender;
        TemplateRenderer = templateRenderer;
        Options = options.Value;
    }

    public virtual async Task SendAsync(string to, string templateName, object? model = null, string? cultureName = null)
    {
        var (subject, body) = await RenderEmailAsync(templateName, model, cultureName);
        await EmailSender.SendAsync(to, subject, body, isBodyHtml: true);
    }

    public virtual async Task SendAsync(string from, string to, string templateName, object? model = null, string? cultureName = null)
    {
        var (subject, body) = await RenderEmailAsync(templateName, model, cultureName);
        await EmailSender.SendAsync(from, to, subject, body, isBodyHtml: true);
    }

    public virtual async Task SendAsync(IEnumerable<string> to, string templateName, object? model = null, string? cultureName = null)
    {
        var (subject, body) = await RenderEmailAsync(templateName, model, cultureName);
        var mail = new MailMessage
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var recipient in to)
        {
            mail.To.Add(recipient);
        }

        await EmailSender.SendAsync(mail);
    }

    protected virtual async Task<(string Subject, string Body)> RenderEmailAsync(
        string templateName,
        object? model,
        string? cultureName)
    {
        var subjectTemplateName = GetSubjectTemplateName(templateName);
        var bodyTemplateName = GetBodyTemplateName(templateName);

        var subject = await TemplateRenderer.RenderAsync(subjectTemplateName, model, cultureName);
        var body = await TemplateRenderer.RenderAsync(bodyTemplateName, model, cultureName);

        // Apply layout if configured
        if (!string.IsNullOrEmpty(Options.DefaultLayoutTemplate))
        {
            var layoutModel = new { Content = body };
            body = await TemplateRenderer.RenderAsync(Options.DefaultLayoutTemplate, layoutModel, cultureName);
        }

        return (subject.Trim(), body);
    }

    protected virtual string GetSubjectTemplateName(string templateName)
    {
        return $"{templateName}{Options.SubjectTemplateSuffix}";
    }

    protected virtual string GetBodyTemplateName(string templateName)
    {
        return $"{templateName}{Options.BodyTemplateSuffix}";
    }
}

