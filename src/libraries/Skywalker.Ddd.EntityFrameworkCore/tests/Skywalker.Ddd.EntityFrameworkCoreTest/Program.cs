// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCoreTest;
using Skywalker.Ddd.EntityFrameworkCoreTest.Domain.Entities;

var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
{
    services.AddSkywalkerCore()
    .AddEntityFrameworkCore()
    .AddDbContext<TestDbContext>(options =>
    {
    });
}).ConfigureLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Warning);
    logging.AddConsole();
})
.Build();
var result= Parallel.For(1, 1000, async i =>
{
    var _dbContextFactory = host.Services.GetRequiredService<IUserDomainService>();
    for (int j = 1; j <= 1000; j++)
    {
        await _dbContextFactory.InsertAsync(new User());
    }
});
Console.WriteLine(result.IsCompleted);
await host.RunAsync();
