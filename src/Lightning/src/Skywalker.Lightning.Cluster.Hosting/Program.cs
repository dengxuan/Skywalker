using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

IHost host = Host.CreateDefaultBuilder().ConfigureLogging((context, loggingBuilder) =>
{
    loggingBuilder.AddConfiguration(context.Configuration);
    loggingBuilder.AddConsole();
}).ConfigureServices(services =>
{
    services.AddLighting(lightner =>
    {
        lightner.Services.AddLightningCluster();
        lightner.UseMessagePack();
    });
}).ConfigureAppConfiguration(configure =>
{
    configure.AddEnvironmentVariables();
    configure.AddCommandLine(args);
    configure.AddJsonFile("appsettings.json");
}).Build();
IOptionsMonitor<ConsoleLoggerOptions> optionsMonitor = host.Services.GetRequiredService<IOptionsMonitor<ConsoleLoggerOptions>>();
InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider(optionsMonitor));
await host.RunAsync();