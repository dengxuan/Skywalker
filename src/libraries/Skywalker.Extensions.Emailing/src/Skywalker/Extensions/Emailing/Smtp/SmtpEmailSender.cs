using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.Emailing.Smtp;

/// <summary>
/// Used to send emails over SMTP.
/// </summary>
public class SmtpEmailSender : EmailSenderBase, ISmtpEmailSender, ITransientDependency
{
    /// <summary>
    /// Creates a new <see cref="SmtpEmailSender"/>.
    /// </summary>
    public SmtpEmailSender(
       IOptions<SmtpEmailSenderConfiguration> smtpConfiguration)
        : base(smtpConfiguration)
    {
    }

    public Task<SmtpClient> BuildClientAsync()
    {
        return Task.Run(() =>
         {
             var smtpClient = new SmtpClient(Options.Host, Options.Port);

             try
             {
                 smtpClient.EnableSsl = Options.EnableSsl;

                 if (!Options.Domain.IsNullOrEmpty())
                 {
                     smtpClient.Credentials = new NetworkCredential(Options.UserName, Options.Password, Options.Domain);
                 }
                 else
                 {
                     smtpClient.Credentials = new NetworkCredential(Options.UserName, Options.Password);
                 }

                 return smtpClient;
             }
             catch
             {
                 smtpClient.Dispose();
                 throw;
             }
         });
    }

    protected override async Task SendEmailAsync(MailMessage mail)
    {
        using (var smtpClient = await BuildClientAsync())
        {
            await smtpClient.SendMailAsync(mail);
        }
    }
}
