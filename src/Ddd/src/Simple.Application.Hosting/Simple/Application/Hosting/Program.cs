using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Simple.EntityFrameworkCore;
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
                    builder.AddEntityFrameworkCore<SimpleDbContext>(options =>
                    {
                        options.Configure(configure =>
                        {
                            configure.UseSqlServer();
                        });
                    });
                    builder.AddAutoMapper(options =>
                    {
                        options.AddProfile<SimpleApplicationAutoMapperProfile>();
                    });
                });
            }).Build();
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
