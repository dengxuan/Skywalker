namespace Skywalker.Extensions.Emailing.Template.Models;

/// <summary>
/// Base class for email template models.
/// </summary>
public abstract class EmailModelBase
{
    /// <summary>
    /// The application name to display in the email.
    /// </summary>
    public string? AppName { get; set; }

    /// <summary>
    /// The application URL.
    /// </summary>
    public string? AppUrl { get; set; }

    /// <summary>
    /// The current year for copyright notices.
    /// </summary>
    public int Year { get; set; } = DateTime.Now.Year;
}

