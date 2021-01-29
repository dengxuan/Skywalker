using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.Application;
using Simple.Application.Hosting;
using Simple.EntityFrameworkCore;
using Skywalker;
using Skywalker.Aspects;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.UnitOfWork;
using Skywalker.Extensions.GuidGenerator;
using System;

try
{
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
        configure.AddSkywalker(builder =>
        {
            builder.AddEntityFrameworkCore<SimpleDbContext>(options =>
            {
                options.UseMySql();
            });
            builder.AddAutoMapper(options =>
            {
                options.AddProfile<SimpleApplicationAutoMapperProfile>();
            });
        });
        configure.AddTransient<IHostedService, SimpleHostedService>();
        configure.AddUnitOfWork(options=>
        {
            options.Timeout = 30000;
        });
        configure.Configure<AspectsOptions>(options =>
        {
            options.Use(new UnitOfWorkInterceptorProvider());
        });
    })
    .UseAspects()
    .StartAsync();

}
catch (Exception ex)
{
    Console.WriteLine(ex);
}