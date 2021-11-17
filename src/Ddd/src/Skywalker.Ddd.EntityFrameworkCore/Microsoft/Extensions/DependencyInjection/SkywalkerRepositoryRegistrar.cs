using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Domain.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkywalkerRepositoryRegistrar : RepositoryRegistrarBase<SkywalkerDbContextRegistrationOptions>
    {
        public SkywalkerRepositoryRegistrar(SkywalkerDbContextRegistrationOptions options) : base(options)
        {

        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType)
        {
            return typeof(SkywalkerRepository<,>).MakeGenericType(dbContextType, entityType);
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            return typeof(SkywalkerRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
        }
    }
}