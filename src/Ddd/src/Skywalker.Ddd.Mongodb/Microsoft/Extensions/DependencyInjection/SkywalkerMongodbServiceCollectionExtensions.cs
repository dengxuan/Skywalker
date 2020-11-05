using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.Mongodb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerMongodbServiceCollectionExtensions
    {
        public static SkywalkerBuilder AddMongodb<TDbContext>(this SkywalkerBuilder builder) where TDbContext : IAbpMongoDbContext
        {
            builder.Services.TryAddTransient( typeof(IMongoDbContextProvider<>),typeof(DefaultMongoDbContextProvider<>));

            builder.Services.TryAddTransient(typeof(IMongoDbRepositoryFilterer<>),typeof(MongoDbRepositoryFilterer<>));

            builder.Services.TryAddTransient(typeof(IMongoDbRepositoryFilterer<,>),typeof(MongoDbRepositoryFilterer<,>));

            return builder;
        }
    }
}
