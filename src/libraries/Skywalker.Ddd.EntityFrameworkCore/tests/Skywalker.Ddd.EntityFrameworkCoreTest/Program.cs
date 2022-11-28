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
    services.AddSkywalker()
    .AddEntityFrameworkCore().AddDbContext<TestDbContext>(options =>
    {
        options.UseMySql("",ServerVersion.AutoDetect(""));
    });
    services.AddEntityFrameworkCore(options =>
    {
        options.UseDbContext<TestDbContext>();
        //options.UseDbContext<TestDbContext>();
        //options.UsePooledDbContextFactory<TestDbContext>(options =>
        //{
        //});
    });
    services.AddPooledDbContextFactory<TestDbContext>(options =>
    {
        options.UseMySql("Server=47.108.173.4;Database=Test;UserId=root;Password=QrBl&X0@NZZJ^ohnw33I;MaximumPoolSize=1024;", ServerVersion.AutoDetect("Server=47.108.173.4;Database=Test;UserId=root;Password=QrBl&X0@NZZJ^ohnw33I;MaximumPoolSize=1024;"));
    },1024);
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
