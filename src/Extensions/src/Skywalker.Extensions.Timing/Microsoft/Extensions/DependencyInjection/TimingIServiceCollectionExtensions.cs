using Skywalker.Extensions.Timing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TimingIServiceCollectionExtensions
    {
        public static IServiceCollection AddTiming(this IServiceCollection services)
        {
            //services.Configure<AbpVirtualFileSystemOptions>(options =>
            //{
            //    options.FileSets.AddEmbedded<AbpTimingResource>();
            //});

            //services.Configure<AbpLocalizationOptions>(options =>
            //{
            //    options
            //        .Resources
            //        .Add<AbpTimingResource>("en")
            //        .AddVirtualJson("/Volo/Abp/Timing/Localization");
            //});
            return services;
        }
    }
}
