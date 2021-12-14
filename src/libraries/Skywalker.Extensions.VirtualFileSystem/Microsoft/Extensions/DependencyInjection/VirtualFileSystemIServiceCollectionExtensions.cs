using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VirtualFileSystemIServiceCollectionExtensions
    {
        public static IServiceCollection AddVirtualFileSystem(this IServiceCollection services)
        {
            services.AddSingleton<IVirtualFileProvider, VirtualFileProvider>();
            services.AddSingleton<IDynamicFileProvider, DynamicFileProvider>();
            return services;
        }
    }
}
