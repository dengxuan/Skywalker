namespace Skywalker.Extensions.Emailing.Template.Models;

/// <summary>
/// Model for password reset email template.
/// </summary>
public class ResetPasswordEmailModel : EmailModelBase
{
    /// <summary>
    /// The user's display name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The password reset link.
    /// </summary>
    public string? ResetLink { get; set; }

    /// <summary>
    /// The expiration time for the reset link in hours.
    /// </summary>
    public int ResetLinkExpirationHours { get; set; } = 24;
}

