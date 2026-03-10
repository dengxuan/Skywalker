namespace Skywalker.Extensions.Emailing.Template.Models;

/// <summary>
/// Model for welcome email template.
/// </summary>
public class WelcomeEmailModel : EmailModelBase
{
    /// <summary>
    /// The user's display name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The activation link for the user's account.
    /// </summary>
    public string? ActivationLink { get; set; }

    /// <summary>
    /// The expiration time for the activation link in hours.
    /// </summary>
    public int ActivationLinkExpirationHours { get; set; } = 24;
}

