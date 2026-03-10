namespace Skywalker.Extensions.Emailing.Template;

/// <summary>
/// Standard email template names.
/// </summary>
public static class EmailTemplateNames
{
    private const string Prefix = "Skywalker.Emailing.";

    /// <summary>
    /// Welcome email template.
    /// </summary>
    public const string Welcome = Prefix + "Welcome";

    /// <summary>
    /// Password reset email template.
    /// </summary>
    public const string ResetPassword = Prefix + "ResetPassword";

    /// <summary>
    /// Email verification template.
    /// </summary>
    public const string VerifyEmail = Prefix + "VerifyEmail";

    /// <summary>
    /// General notification email template.
    /// </summary>
    public const string Notification = Prefix + "Notification";

    /// <summary>
    /// Email layout template.
    /// </summary>
    public const string Layout = Prefix + "Layout";
}

