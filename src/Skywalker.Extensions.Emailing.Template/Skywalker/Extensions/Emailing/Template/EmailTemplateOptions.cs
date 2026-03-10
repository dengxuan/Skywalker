namespace Skywalker.Extensions.Emailing.Template;

/// <summary>
/// Options for email template rendering.
/// </summary>
public class EmailTemplateOptions
{
    /// <summary>
    /// The suffix for subject templates. Default is ".Subject".
    /// </summary>
    public string SubjectTemplateSuffix { get; set; } = ".Subject";

    /// <summary>
    /// The suffix for body templates. Default is ".Body".
    /// </summary>
    public string BodyTemplateSuffix { get; set; } = ".Body";

    /// <summary>
    /// The default layout template name to wrap email body.
    /// If null or empty, no layout is applied.
    /// </summary>
    public string? DefaultLayoutTemplate { get; set; }

    /// <summary>
    /// The base path for email templates. Default is "EmailTemplates".
    /// </summary>
    public string TemplateBasePath { get; set; } = "EmailTemplates";
}

