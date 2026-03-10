namespace Skywalker.Extensions.Emailing.Template.Models;

/// <summary>
/// Model for notification email template.
/// </summary>
public class NotificationEmailModel : EmailModelBase
{
    /// <summary>
    /// The user's display name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The notification title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The notification message content.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Optional action button text.
    /// </summary>
    public string? ActionText { get; set; }

    /// <summary>
    /// Optional action button URL.
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// The notification timestamp.
    /// </summary>
    public DateTime? Timestamp { get; set; }
}

