//using Skywalker.Application.Localization.Resources.AbpDdd;
//using Skywalker.Auditing;
//using Skywalker.Localization;
//using Skywalker.Modularity;
//using Skywalker.VirtualFileSystem;

//namespace Skywalker.Application
//{
//    [DependsOn(
//        typeof(AbpAuditingModule),
//        typeof(AbpLocalizationModule)
//        )]
//    public class AbpDddApplicationContractsModule : AbpModule
//    {
//        public override void ConfigureServices(ServiceConfigurationContext context)
//        {
//            Configure<AbpVirtualFileSystemOptions>(options =>
//            {
//                options.FileSets.AddEmbedded<AbpDddApplicationContractsModule>();
//            });

//            Configure<AbpLocalizationOptions>(options =>
//            {
//                options.Resources
//                    .Add<AbpDddApplicationContractsResource>("en")
//                    .AddVirtualJson("/Volo/Abp/Application/Localization/Resources/AbpDdd");
//            });
//        }
//    }
//}
