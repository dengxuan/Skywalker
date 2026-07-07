using Microsoft.Extensions.DependencyInjection;
using Skywalker.Emailing;
using Skywalker.Emailing.Smtp;


namespace Microsoft.Extensions.DependencyInjection;

public static class SmtpEmailingIServiceCollectionExtensions
{
    /// <summary>
    /// Adds SMTP email delivery. Registers <see cref="SmtpEmailSender"/> as <see cref="IEmailSender"/>.
    /// </summary>
    public static IServiceCollection AddSmtpEmailing(this IServiceCollection services, Action<SmtpEmailSenderConfiguration> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddSingleton<ISmtpEmailSender, SmtpEmailSender>();
        return services;
    }
}
