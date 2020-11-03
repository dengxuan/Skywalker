using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.EntityFrameworkCore;
using Skywalker.Distributed;
using Skywalker.Distributed.Client.Abstractions;
using Skywalker.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        async static Task MainAsync(string[] args)
        {
            using var host = new HostBuilder()
                 .ConfigureAppConfiguration((context, configure) =>
                 {
                     configure.AddJsonFile("appsettings.json", optional: true);
                     configure.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                     configure.AddEnvironmentVariables();
                 })
                  .ConfigureServices((context, services) =>
                  {
                      services.AddSkywalker(builder =>
                      {
                          builder.AddEntityFrameworkCore<SimpleDbContext>(options =>
                          {
                              options.UseSqlServer();
                          });
                      });
                      //services.AddRedisCaching(configure =>
                      //{
                      //    configure.ConnectionString = "localhost:6379,defaultDatabase=0";
                      //});
                      //services.AddSkywalkerDbContext<SimpleDbContext>(optionsBuilder =>
                      //{
                      //    optionsBuilder.AddDefaultRepositories();
                      //});
                      services.AddSingleton<IDistributedClientFactory, DistributedClientFactory>();
                  })
                  .ConfigureLogging((context, logging) =>
                  {
                      logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                      logging.AddConsole();
                  })
                  .Build();
            //ILoggerProvider loggerProvider = host.Services.GetRequiredService<ILoggerProvider>();
            //InternalLoggerFactory.DefaultFactory.AddProvider(loggerProvider);
            //IDistributedClientFactory distributedClientFactory = host.Services.GetRequiredService<IDistributedClientFactory>();
            //IDistributedProxyGenerator distributedProxyGenerator = host.Services.GetRequiredService<IDistributedProxyGenerator>();
            //IEnumerable<Type> types = distributedProxyGenerator.GenerateProxy(new List<Type> { typeof(IUserAppService) });
            //IDistributedClusterDescriptor distributedClusterDescriptor = host.Services.GetRequiredService<IDistributedClusterDescriptor>();
            //distributedClusterDescriptor.Id = "Simple";
            //distributedClusterDescriptor.DistributedClusters = new List<IDistributedCluster>()
            //{
            //    new DistributedCluster
            //    {
            //        Address = "127.0.0.1",
            //        Port=30000,
            //        EnableTls=false,
            //        Weight=10
            //    }
            //};
            //IDistributedProxyFactory distributedProxy = host.Services.GetRequiredService<IDistributedProxyFactory>();
            //IUserAppService userAppService = distributedProxy.GetService<IUserAppService>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    User user = await userAppService.CreateAsync(new User() { Name = $"abc+{i}" });
            //    Console.WriteLine(user.Id);
            //}
            //IDistributedServiceFactory distributedServiceFactory = host.Services.GetRequiredService<IDistributedServiceFactory>();
            //distributedServiceFactory.LoadServices(new List<Assembly> { typeof(IUserAppService).Assembly });
            //DistributedServiceDescriptor distributedServiceDescriptor = distributedServiceFactory.GetDistributedServiceDescriptor("IUserAppService@CreateAsync[User]");
            //Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
            //{
            //    { "user", new User(Guid.NewGuid()){ Name="asd" }  }
            //};
            //var obj = await distributedServiceDescriptor.InvokeHandler.Invoke(keyValuePairs);
            //var descripts = distributedServiceFactory.GetDistributedServiceDescriptors();
            await host.StartAsync();
            Console.ReadLine();
            await host.StopAsync();
        }
    }
}
