using Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.Application;
using Simple.Application.Hosting;
using Simple.Infrastructure.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Extensions.GuidGenerator;

Host.CreateDefaultBuilder(args).ConfigureServices(configure =>
{
    configure.AddRedisCaching(configure =>
    {
        configure.ConnectionString = "127.0.0.1";
    });
    configure.AddLogging(configure =>
    {
        configure.SetMinimumLevel(LogLevel.Debug);
    });
    configure.Configure<SequentialGuidGeneratorOptions>(option =>
    {
        option.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString;
    });
    configure.AddAutoMapper(options =>
    {
        options.AddProfile<SimpleApplicationAutoMapperProfile>();
    });
    configure.AddSkywalker(builder =>
    {
        builder.AddEntityFrameworkCore<SimpleDbContext>(options =>
        {
            options.UseMySql();
        });
    });
    configure.AddTransient<IHostedService, SimpleHostedService>();
})
.RunConsoleAsync();
