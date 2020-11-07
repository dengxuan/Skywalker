using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;
using System;

namespace Skywalker.Ddd.Infrastructure.Mongodb.DependencyInjection
{
    public class AbpMongoDbContextRegistrationOptions : SkywalkerCommonDbContextRegistrationOptions, IAbpMongoDbContextRegistrationOptionsBuilder
    {
        public AbpMongoDbContextRegistrationOptions(Type originalDbContextType, IServiceCollection services) 
            : base(services)
        {
        }
    }
}