using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Simple.EntityFrameworkCore;
using Skywalker.Ddd.Infrastructure.Domain.Repositories;
using Skywalker.Domain.Repositories;
using Skywalker.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    });
                    builder.AddAutoMapper(options =>
                    {
                        options.AddProfile<SimpleApplicationAutoMapperProfile>();
                    });
                });
            }).Build();
            IRepository<User> user = host.Services.GetRequiredService<IRepository<User>>();
            ISimpleUserApplicationService simpleUserApplicationService = host.Services.GetRequiredService<ISimpleUserApplicationService>();
            for (int i = 0; i < 10; i++)
            {
                var dto = await simpleUserApplicationService.CreateUserAsync($"Laod-{i}");
                Console.WriteLine(dto.Name);
            }
            await simpleUserApplicationService.FindUsersAsync();
            await host.RunAsync();
        }
    }
}
