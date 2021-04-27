//using Skywalker.Localization.Resources.AbpLocalization;
//using Skywalker.Modularity;
//using Skywalker.Settings;
//using Skywalker.Extensions.VirtualFileSystem;

//namespace Skywalker.Localization
//{
//    [DependsOn(
//        typeof(AbpVirtualFileSystemModule),
//        typeof(AbpSettingsModule),
//        typeof(AbpLocalizationAbstractionsModule)
//        )]
//    public class AbpLocalizationModule : AbpModule
//    {
//        public override void ConfigureServices(ServiceConfigurationContext context)
//        {
//            AbpStringLocalizerFactory.Replace(context.Services);

//            Configure<AbpVirtualFileSystemOptions>(options =>
//            {
//                options.FileSets.AddEmbedded<AbpLocalizationModule>("Skywalker", "Volo/Abp");
//            });

//            Configure<AbpLocalizationOptions>(options =>
//            {
//                options
//                    .Resources
//                    .Add<DefaultResource>("en");

//                options
//                    .Resources
//                    .Add<AbpLocalizationResource>("en")
//                    .AddVirtualJson("/Localization/Resources/AbpLocalization");
//            });
//        }
//    }
//}
