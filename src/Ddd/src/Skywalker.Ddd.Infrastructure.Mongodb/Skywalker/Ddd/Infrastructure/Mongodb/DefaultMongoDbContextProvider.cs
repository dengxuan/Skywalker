using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Skywalker.Data;
using System;
using Skywalker.Ddd.Infrastructure.Mongodb;

namespace Skywalker.Ddd.Mongodb
{
    public class DefaultMongoDbContextProvider<TMongoDbContext> : IMongoDbContextProvider<TMongoDbContext> where TMongoDbContext : IMongodbContext
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IConnectionStringResolver _connectionStringResolver;

        public DefaultMongoDbContextProvider(IServiceProvider serviceProvider, IConnectionStringResolver connectionStringResolver)
        {
            _serviceProvider = serviceProvider;
            _connectionStringResolver = connectionStringResolver;
        }

        public TMongoDbContext GetDbContext()
        {

            var connectionString = _connectionStringResolver.Resolve<TMongoDbContext>();
            var dbContextKey = $"{typeof(TMongoDbContext).FullName}_{connectionString}";

            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            if (databaseName.IsNullOrWhiteSpace())
            {
                databaseName = ConnectionStringNameAttribute.GetConnStringName<TMongoDbContext>();
            }

            var database = new MongoClient(mongoUrl).GetDatabase(databaseName);

            var dbContext = _serviceProvider.GetRequiredService<TMongoDbContext>();

            dbContext.ToAbpMongoDbContext().InitializeDatabase(database);

            return dbContext;

        }
    }
}
