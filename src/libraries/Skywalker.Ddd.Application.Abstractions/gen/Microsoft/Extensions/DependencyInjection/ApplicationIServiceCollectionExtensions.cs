//using Skywalker.Ddd.Application;
//using Skywalker.Ddd.Application.Abstractions;

//namespace Microsoft.Extensions.DependencyInjection
//{
//    public static class ApplicationIServiceCollectionExtensions
//    {
//        public static IServiceCollection AddApplication(this IServiceCollection services)
//        {
//            services.AddScoped<IApplication, Application>();
//            services.AddScoped(typeof(IExecuteHandlerProvider<>), typeof(ExecuteHandlerProvider<>));
//            services.AddScoped(typeof(IExecuteHandlerProvider<,>), typeof(ExecuteQueryHandlerProvider<,>));
//            return services;
//        }
//    }
//}
