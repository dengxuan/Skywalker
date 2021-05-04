using Skywalker.Ddd.Commands;
using Skywalker.Ddd.Commands.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static  class CommandIServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            services.AddSingleton<ICommander, DefaultCommander>();
            services.AddSingleton(typeof(ICommandPublisher<>), typeof(DefaultCommandPublisher<>));
            services.AddSingleton(typeof(ICommandHandlerProvider<,>), typeof(DefaultCommandHandlerProvider<,>));
            //services.Scan(scanner =>
            //{
            //    scanner.FromApplicationDependencies()
            //           .AddClasses(filter =>
            //           {
            //               filter.AssignableTo(typeof(ICommandHandler<>));
            //           })
            //           .AsImplementedInterfaces()
            //           .WithSingletonLifetime();
            //});
            return services;
        }
    }
}
