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
using Skywalker.Extensions.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

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
                            options.UseMySql();
                        });
                        initializer.AddMongodb<SimpleMongoContext>();
                    });
                    builder.AddAutoMapper(options =>
                    {
                        options.AddProfile<SimpleApplicationAutoMapperProfile>();
                    });
                    builder.Services.AddSingleton<IClock, Clock>();
                });
            }).Build();
            Stopwatch stopwatch = Stopwatch.StartNew();
            IRepository<User, short> users = host.Services.GetRequiredService<IRepository<User, short>>();
            ISimpleUserApplicationService simpleUserApplicationService = host.Services.GetRequiredService<ISimpleUserApplicationService>();
            for (int i = 0; i < 1000; i++)
            {
                await users.InsertAsync(new User(0) { Name = $"Laod-{i}" });
                var dto = await simpleUserApplicationService.CreateUserAsync($"Laod-{i}");
            }
            await users.InsertAsync(new User(0) { Name = $"Laod-{1002}" },true);
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
