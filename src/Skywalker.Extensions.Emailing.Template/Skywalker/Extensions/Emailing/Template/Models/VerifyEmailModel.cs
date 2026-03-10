namespace Skywalker.Extensions.Emailing.Template.Models;

/// <summary>
/// Model for email verification template.
/// </summary>
public class VerifyEmailModel : EmailModelBase
{
    /// <summary>
    /// The user's display name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The email verification link.
    /// </summary>
    public string? VerificationLink { get; set; }

    /// <summary>
    /// The verification code (alternative to link).
    /// </summary>
    public string? VerificationCode { get; set; }

    /// <summary>
    /// The expiration time for the verification in hours.
    /// </summary>
    public int ExpirationHours { get; set; } = 24;
}

