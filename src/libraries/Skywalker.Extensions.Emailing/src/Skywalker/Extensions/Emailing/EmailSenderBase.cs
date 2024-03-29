using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.Emailing;

/// <summary>
/// This class can be used as base to implement <see cref="IEmailSender"/>.
/// </summary>
public abstract class EmailSenderBase : IEmailSender
{
    protected SmtpEmailSenderConfiguration Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected EmailSenderBase(IOptions<SmtpEmailSenderConfiguration> options)
    {
        Options = options.Value;
    }

    public virtual async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(new MailMessage
        {
            To = { to },
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml
        });
    }

    public virtual async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
    }

    public virtual async Task SendAsync(MailMessage mail, bool normalize = true)
    {
        if (normalize)
        {
            await NormalizeMailAsync(mail);
        }

        await SendEmailAsync(mail);
    }

    public virtual async Task QueueAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(to, subject, body, isBodyHtml);
        return;
    }

    public virtual async Task QueueAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(from, to, subject, body, isBodyHtml);
        return;
    }

    /// <summary>
    /// Should implement this method to send email in derived classes.
    /// </summary>
    /// <param name="mail">Mail to be sent</param>
    protected abstract Task SendEmailAsync(MailMessage mail);

    /// <summary>
    /// Normalizes given email.
    /// Fills <see cref="MailMessage.From"/> if it's not filled before.
    /// Sets encodings to UTF8 if they are not set before.
    /// </summary>
    /// <param name="mail">Mail to be normalized</param>
    protected virtual Task NormalizeMailAsync(MailMessage mail)
    {
        return Task.Run(() =>
        {
            if (mail.From == null || mail.From.Address.IsNullOrEmpty())
            {
                mail.From = new MailAddress(
                    Options.DefaultFromAddress!,
                    Options.DefaultFromDisplayName!,
                    Encoding.UTF8
                    );
            }

            if (mail.HeadersEncoding == null)
            {
                mail.HeadersEncoding = Encoding.UTF8;
            }

            if (mail.SubjectEncoding == null)
            {
                mail.SubjectEncoding = Encoding.UTF8;
            }

            if (mail.BodyEncoding == null)
            {
                mail.BodyEncoding = Encoding.UTF8;
            }
        });
    }
}
