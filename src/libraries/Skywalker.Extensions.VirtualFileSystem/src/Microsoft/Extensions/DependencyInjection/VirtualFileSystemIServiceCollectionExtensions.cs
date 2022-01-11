using Skywalker.Extensions.VirtualFileSystem;

namespace Microsoft.Extensions.DependencyInjection;

public static class VirtualFileSystemIServiceCollectionExtensions
{
    public static IServiceCollection AddVirtualFileSystem(this IServiceCollection services)
    {
        services.AddSingleton<IVirtualFileProvider, VirtualFileProvider>();
        services.AddSingleton<IDynamicFileProvider, DynamicFileProvider>();
        return services;
    }
}
