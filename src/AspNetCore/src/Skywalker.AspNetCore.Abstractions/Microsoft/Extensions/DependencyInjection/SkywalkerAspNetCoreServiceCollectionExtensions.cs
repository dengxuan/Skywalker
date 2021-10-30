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

        public static SkywalkerDddBuilder AddAspNetCore(this SkywalkerDddBuilder skywalkerBuilder)
        {
            skywalkerBuilder.Services.AddHttpContextAccessor();
            return skywalkerBuilder;
        }
    }
}
