using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.Logging.File;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.Logging;

#if NET5_0_OR_GREATER
    [UnsupportedOSPlatform("browser")]
#endif
public static class FileLoggerExtensions
{
    internal const string TrimmingRequiresUnreferencedCodeMessage = "TOptions's dependent types may have their members trimmed. Ensure all required members are preserved.";

    /// <summary>
    /// Adds a file logger named 'File' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>

#if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
            Justification = "AddConsoleFormatter and RegisterProviderOptions are only dangerous when the Options type cannot be statically analyzed, but that is not the case here. " +
            "The DynamicallyAccessedMembers annotations on them will make sure to preserve the right members from the different options objects.")]
#endif
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.AddFileFormatter<JsonFileFormatter, JsonFileFormatterOptions>();
        builder.AddFileFormatter<SystemdFileFormatter, FileFormatterOptions>();
        builder.AddFileFormatter<SimpleFileFormatter, SimpleFileFormatterOptions>();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<FileLoggerOptions, FileLoggerProvider>(builder.Services);

        return builder;
    }

    /// <summary>
    /// Adds a file logger named 'File' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="FileLogger"/>.</param>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        builder.AddFile();
        builder.Services.Configure(configure);

        return builder;
    }

    /// <summary>
    /// Add the default file log formatter named 'simple' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddSimpleFile(this ILoggingBuilder builder) =>
        builder.AddFormatterWithName(FileFormatterNames.Simple);

    /// <summary>
    /// Add and configure a file log formatter named 'simple' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="FileLogger"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddSimpleFile(this ILoggingBuilder builder, Action<SimpleFileFormatterOptions> configure)
    {
        return builder.AddFileWithFormatter<SimpleFileFormatterOptions>(FileFormatterNames.Simple, configure);
    }

    /// <summary>
    /// Add a file log formatter named 'json' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddJsonFile(this ILoggingBuilder builder) =>
        builder.AddFormatterWithName(FileFormatterNames.Json);

    /// <summary>
    /// Add and configure a file log formatter named 'json' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="FileLogger"/> options for the built-in json log formatter.</param>
    public static ILoggingBuilder AddJsonFile(this ILoggingBuilder builder, Action<JsonFileFormatterOptions> configure)
    {
        return builder.AddFileWithFormatter<JsonFileFormatterOptions>(FileFormatterNames.Json, configure);
    }

    /// <summary>
    /// Add and configure a file log formatter named 'systemd' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="FileLogger"/> options for the built-in systemd log formatter.</param>
    public static ILoggingBuilder AddSystemdFile(this ILoggingBuilder builder, Action<FileFormatterOptions> configure)
    {
        return builder.AddFileWithFormatter<FileFormatterOptions>(FileFormatterNames.Systemd, configure);
    }

    /// <summary>
    /// Add a file log formatter named 'systemd' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddSystemdFile(this ILoggingBuilder builder) =>
        builder.AddFormatterWithName(FileFormatterNames.Systemd);

    internal static ILoggingBuilder AddFileWithFormatter<TOptions>(this ILoggingBuilder builder, string name, Action<TOptions> configure)
        where TOptions : FileFormatterOptions
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }
        builder.AddFormatterWithName(name);
        builder.Services.Configure(configure);

        return builder;
    }

    private static ILoggingBuilder AddFormatterWithName(this ILoggingBuilder builder, string name) =>
        builder.AddFile((FileLoggerOptions options) => options.FormatterName = name);

    /// <summary>
    /// Adds a custom file logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
#if NET5_0_OR_GREATER
    [RequiresUnreferencedCode(TrimmingRequiresUnreferencedCodeMessage)]
    public static ILoggingBuilder AddFileFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(this ILoggingBuilder builder)
#else
    public static ILoggingBuilder AddFileFormatter<TFormatter, TOptions>(this ILoggingBuilder builder)
#endif
        where TOptions : FileFormatterOptions
        where TFormatter : FileFormatter
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<FileFormatter, TFormatter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, FileLoggerFormatterConfigureOptions<TFormatter, TOptions>>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, FileLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());

        return builder;
    }

    /// <summary>
    /// Adds a custom file logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
#if NET5_0_OR_GREATER
    [RequiresUnreferencedCode(TrimmingRequiresUnreferencedCodeMessage)]
    public static ILoggingBuilder AddFileFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
#else
    public static ILoggingBuilder AddFileFormatter<TFormatter, TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
#endif
        where TOptions : FileFormatterOptions
        where TFormatter : FileFormatter
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        builder.AddFileFormatter<TFormatter, TOptions>();
        builder.Services.Configure(configure);
        return builder;
    }
}
#if NET5_0_OR_GREATER
[UnsupportedOSPlatform("browser")]
internal sealed class FileLoggerFormatterConfigureOptions<TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions> : ConfigureFromConfigurationOptions<TOptions>
#else
internal sealed class FileLoggerFormatterConfigureOptions<TFormatter, TOptions> : ConfigureFromConfigurationOptions<TOptions>
#endif
    where TOptions : FileFormatterOptions
    where TFormatter : FileFormatter
{
#if NET5_0_OR_GREATER
    [RequiresUnreferencedCode(FileLoggerExtensions.TrimmingRequiresUnreferencedCodeMessage)]
#endif
    public FileLoggerFormatterConfigureOptions(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration) :
        base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    {
    }
}

#if NET5_0_OR_GREATER
[UnsupportedOSPlatform("browser")]
#endif
internal sealed class FileLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions> : ConfigurationChangeTokenSource<TOptions>
    where TOptions : FileFormatterOptions
    where TFormatter : FileFormatter
{
    public FileLoggerFormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
        : base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    {
    }
}
