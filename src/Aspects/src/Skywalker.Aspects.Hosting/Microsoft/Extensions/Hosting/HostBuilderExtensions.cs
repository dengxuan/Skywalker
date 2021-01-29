using Microsoft.Extensions.DependencyInjection;
using Skywalker.Aspects;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseAspects(this IHostBuilder hostBuilder, ServiceProviderOptions? options = null)
        {
            hostBuilder.UseServiceProviderFactory(new InterceptableServiceProviderFactory(options));
            return hostBuilder;
        }
    }
}