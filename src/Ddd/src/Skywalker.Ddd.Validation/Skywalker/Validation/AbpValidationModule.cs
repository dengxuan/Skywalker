//using System;
//using System.Collections.Generic;
//using Microsoft.Extensions.DependencyInjection;
//using Skywalker.Localization;
//using Skywalker.Modularity;
//using Skywalker.Validation.Localization;
//using Skywalker.Extensions.VirtualFileSystem;

//namespace Skywalker.Validation
//{
//    [DependsOn(
//        typeof(AbpValidationAbstractionsModule),
//        typeof(AbpLocalizationModule)
//        )]
//    public class AbpValidationModule : AbpModule
//    {
//        public override void PreConfigureServices(ServiceConfigurationContext context)
//        {
//            context.Services.OnRegistred(ValidationInterceptorRegistrar.RegisterIfNeeded);
//            AutoAddObjectValidationContributors(context.Services);
//        }

//        public override void ConfigureServices(ServiceConfigurationContext context)
//        {
//            Configure<AbpVirtualFileSystemOptions>(options =>
//            {
//                options.FileSets.AddEmbedded<AbpValidationResource>();
//            });

//            Configure<AbpLocalizationOptions>(options =>
//            {
//                options.Resources
//                    .Add<AbpValidationResource>("en")
//                    .AddVirtualJson("/Volo/Abp/Validation/Localization");
//            });
//        }

//        private static void AutoAddObjectValidationContributors(IServiceCollection services)
//        {
//            var contributorTypes = new List<Type>();

//            services.OnRegistred(context =>
//            {
//                if (typeof(IObjectValidationContributor).IsAssignableFrom(context.ImplementationType))
//                {
//                    contributorTypes.Add(context.ImplementationType);
//                }
//            });

//            services.Configure<AbpValidationOptions>(options =>
//            {
//                options.ObjectValidationContributors.AddIfNotContains(contributorTypes);
//            });
//        }
//    }
//}
