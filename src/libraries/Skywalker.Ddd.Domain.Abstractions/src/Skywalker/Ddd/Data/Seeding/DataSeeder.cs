//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;

//namespace Skywalker.Data.Seeding
//{
//    //TODO: Create a Skywalker.Data.Seeding namespace?
//    public class DataSeeder : IDataSeeder/*, ITransientDependency*/
//    {
//        protected IHybridServiceScopeFactory ServiceScopeFactory { get; }
//        protected SkywalkerDataSeedOptions Options { get; }

//        public DataSeeder(
//            IOptions<SkywalkerDataSeedOptions> options,
//            IHybridServiceScopeFactory serviceScopeFactory)
//        {
//            ServiceScopeFactory = serviceScopeFactory;
//            Options = options.Value;
//        }

//        public virtual async Task SeedAsync(DataSeedContext context)
//        {
//            using var scope = ServiceScopeFactory.CreateScope();
//            foreach (var contributorType in Options.Contributors)
//            {
//                var contributor = (IDataSeedContributor)scope
//                    .ServiceProvider
//                    .GetRequiredService(contributorType);

//                await contributor.SeedAsync(context);
//            }
//        }
//    }
//}