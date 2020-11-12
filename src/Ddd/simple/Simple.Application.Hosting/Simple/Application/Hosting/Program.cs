using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Simple.Infrastructure.EntityFrameworkCore;
using Simple.Infrastructure.Mongodb;
using Skywalker;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;
using Skywalker.Extensions.GuidGenerator;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.Application.Hosting
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args).ConfigureServices(configure =>
            {
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
                    builder.AddInfrastructure(initializer =>
                    {
                        initializer.AddEntityFrameworkCore<SimpleDbContext>(options =>
                        {
                            options.UseMySql();
                        });
                        initializer.AddMongodb<SimpleMongoContext>();
                    });
                    builder.AddAutoMapper(options =>
                    {
                        options.AddProfile<SimpleApplicationAutoMapperProfile>();
                    });
                });
            }).Build();
            using IServiceScope scope = host.Services.CreateScope();
            ISimpleUserApplicationService simpleUserApplicationService = scope.ServiceProvider.GetRequiredService<ISimpleUserApplicationService>();
            await simpleUserApplicationService.CreateUserAsync("dengxuan");
            List<UserDto> list = await simpleUserApplicationService.FindUsersAsync("d");

            foreach (var item in list)
            {
                System.Console.WriteLine(item.Name);
            }

            await host.RunAsync();
        }
    }
}
