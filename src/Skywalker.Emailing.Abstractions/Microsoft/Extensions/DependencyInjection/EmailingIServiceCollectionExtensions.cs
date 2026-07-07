using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Emailing;


namespace Microsoft.Extensions.DependencyInjection;

public static class EmailingIServiceCollectionExtensions
{
    /// <summary>
    /// Adds Emailing services with NullEmailSender as default.
    /// Add a delivery provider package (e.g. Skywalker.Emailing.Smtp) to actually send emails.
    /// </summary>
    public static IServiceCollection AddEmailing(this IServiceCollection services)
    {
        // 默认使用 NullEmailSender；实际投递由 provider 包（如 AddSmtpEmailing）覆盖注册
        services.TryAddSingleton<IEmailSender, NullEmailSender>();
        return services;
    }

    /// <summary>
    /// Adds Emailing services with sender defaults (from address / display name).
    /// </summary>
    public static IServiceCollection AddEmailing(this IServiceCollection services, Action<EmailSenderOptions> configure)
    {
        services.Configure(configure);
        return services.AddEmailing();
    }
}
