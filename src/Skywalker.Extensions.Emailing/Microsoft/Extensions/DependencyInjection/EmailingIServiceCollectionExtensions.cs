using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.Emailing;
using Skywalker.Extensions.Emailing.Smtp;

[assembly: Skywalker.SkywalkerModule("Emailing")]

namespace Microsoft.Extensions.DependencyInjection;

public static class EmailingIServiceCollectionExtensions
{
    /// <summary>
    /// Adds Emailing services with NullEmailSender as default.
    /// Use Configure&lt;SmtpEmailSenderConfiguration&gt; to configure SMTP settings.
    /// </summary>
    public static IServiceCollection AddEmailing(this IServiceCollection services)
    {
        // 默认使用 NullEmailSender，如果配置了 SMTP 则使用 SmtpEmailSender
        services.TryAddSingleton<IEmailSender, NullEmailSender>();
        return services;
    }

    /// <summary>
    /// Adds Emailing services with SMTP configuration.
    /// </summary>
    public static IServiceCollection AddEmailing(this IServiceCollection services, Action<SmtpEmailSenderConfiguration> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddSingleton<ISmtpEmailSender, SmtpEmailSender>();
        return services;
    }
}
