using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;
using System;

namespace Volo.Abp.MongoDB.DependencyInjection
{
    public class AbpMongoDbContextRegistrationOptions : SkywalkerCommonDbContextRegistrationOptions, IAbpMongoDbContextRegistrationOptionsBuilder
    {
        public AbpMongoDbContextRegistrationOptions(Type originalDbContextType, IServiceCollection services) 
            : base(services)
        {
        }
    }
}