// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCoreTest;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore.Repositories;
using Skywalker.Ddd.EntityFrameworkCoreTest.Domain.Entities;

var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
{
    services.AddDomainServices(builder =>
    {
        builder.AddDomainServicesCore();
    });
    services.AddEntityFrameworkCore(options =>
    {
    }).AddDbContextFactory<TestDbContext>();
    services.TryAddTransient(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
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
    var _dbContextFactory = host.Services.GetRequiredService<IDbContextFactory<TestDbContext>>();
    using var context = await _dbContextFactory.CreateDbContextAsync();
    for (int j = 1; j <= 1000; j++)
    {
        var users = context.Users.Add(new User(Guid.NewGuid(), (i*j).ToString()));
    }
    await context.SaveChangesAsync();
});
Console.WriteLine(result.IsCompleted);
await host.RunAsync();
