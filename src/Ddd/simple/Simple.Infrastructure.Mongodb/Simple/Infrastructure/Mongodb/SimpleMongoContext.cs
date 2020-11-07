using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MongoDB;

namespace Simple.Infrastructure.Mongodb
{
    [ConnectionStringName("Mongo")]
    public class SimpleMongoContext : AbpMongoDbContext
    {
        public IDataCollection<MongoUser>? MongoUsers { get; }

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
        }
    }
}
