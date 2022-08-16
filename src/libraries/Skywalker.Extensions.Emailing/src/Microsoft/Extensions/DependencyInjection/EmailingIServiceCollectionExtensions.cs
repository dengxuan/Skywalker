using Skywalker.Extensions.Emailing;
using Skywalker.Extensions.Emailing.Smtp;

namespace Microsoft.Extensions.DependencyInjection;

public static class EmailingIServiceCollectionExtensions
{
    public static IServiceCollection AddEmailing(this IServiceCollection service, Action<SmtpEmailSenderConfiguration> options)
    {
        service.Configure(options);

        service.AddSingleton<IEmailSender, SmtpEmailSender>();
        service.AddSingleton<ISmtpEmailSender, SmtpEmailSender>();

        return service;
    }
}
