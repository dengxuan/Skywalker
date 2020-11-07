using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.Mongodb;

namespace Simple.Infrastructure.Mongodb
{
    [ConnectionStringName("Mongo")]
    public class SimpleMongoContext : SkywalkerMongodbContext
    {
        public IDataCollection<MongoUser>? MongoUsers { get; }

        public SimpleMongoContext(IMongoModelSource mongoModelSource):base(mongoModelSource)
        {
        }

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.Entity<MongoUser>(b =>
            {
                b.CollectionName = "MongoUsers";
            });
        }
    }
}
