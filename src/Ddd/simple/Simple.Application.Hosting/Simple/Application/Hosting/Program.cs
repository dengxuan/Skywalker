using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Simple.Infrastructure.EntityFrameworkCore;
using Simple.Infrastructure.Mongodb;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;
using Skywalker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                configure.AddSkywalker(builder =>
                {
                    builder.AddInfrastructure(initializer =>
                    {
                        initializer.AddEntityFrameworkCore<SimpleDbContext>(options =>
                        {
                            options.UseSqlServer();
                        });
                        initializer.AddMongodb<SimpleMongoContext>();
                    });
                    builder.AddAutoMapper(options =>
                    {
                        options.AddProfile<SimpleApplicationAutoMapperProfile>();
                    });
                });
            }).Build();
            Stopwatch stopwatch = Stopwatch.StartNew();
            ISimpleUserApplicationService simpleUserApplicationService = host.Services.GetRequiredService<ISimpleUserApplicationService>();
            for (int i = 0; i < 1000; i++)
            {
                var dto = await simpleUserApplicationService.CreateUserAsync($"Laod-{i}");
            }
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
            List<UserDto> userDtos = await simpleUserApplicationService.FindUsersAsync();
            stopwatch.Stop();
            foreach (var item in userDtos)
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            await host.RunAsync();
        }
    }
}
