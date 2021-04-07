using Microsoft.AspNetCore.Hosting;
using Skywalker;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerAspNetCoreServiceCollectionExtensions
    {
        public static IWebHostEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            return services.GetSingletonInstance<IWebHostEnvironment>();
        }

        public static SkywalkerBuilder AddAspNetCore(this SkywalkerBuilder skywalkerBuilder)
        {
            skywalkerBuilder.Services.AddHttpContextAccessor();
            return skywalkerBuilder;
        }
    }
}
