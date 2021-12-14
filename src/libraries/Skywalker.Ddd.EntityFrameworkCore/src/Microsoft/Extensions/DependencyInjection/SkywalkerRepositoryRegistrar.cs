using Skywalker.Ddd.EntityFrameworkCore.Repositories;
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
            return typeof(Repository<,>).MakeGenericType(dbContextType, entityType);
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            return typeof(Repository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
        }
    }
}