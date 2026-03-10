namespace Skywalker.Extensions.Emailing.Template;

/// <summary>
/// Email sender that supports template rendering.
/// </summary>
public interface ITemplateEmailSender
{
    /// <summary>
    /// Sends an email using a template.
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="templateName">Name of the template to use</param>
    /// <param name="model">Model object for template rendering</param>
    /// <param name="cultureName">Culture name for localization</param>
    /// <returns></returns>
    Task SendAsync(string to, string templateName, object? model = null, string? cultureName = null);

    /// <summary>
    /// Sends an email using a template with a specific sender.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="templateName">Name of the template to use</param>
    /// <param name="model">Model object for template rendering</param>
    /// <param name="cultureName">Culture name for localization</param>
    /// <returns></returns>
    Task SendAsync(string from, string to, string templateName, object? model = null, string? cultureName = null);

    /// <summary>
    /// Sends an email using a template with multiple recipients.
    /// </summary>
    /// <param name="to">List of recipient email addresses</param>
    /// <param name="templateName">Name of the template to use</param>
    /// <param name="model">Model object for template rendering</param>
    /// <param name="cultureName">Culture name for localization</param>
    /// <returns></returns>
    Task SendAsync(IEnumerable<string> to, string templateName, object? model = null, string? cultureName = null);
}

