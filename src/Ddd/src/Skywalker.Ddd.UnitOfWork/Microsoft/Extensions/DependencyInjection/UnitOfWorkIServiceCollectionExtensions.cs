using Skywalker.Aspects.Abstractinons;
using Skywalker.Ddd.UnitOfWork;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UnitOfWorkIServiceCollectionExtensions
    {
        public static IServiceCollection UseUnitOfWork(this IServiceCollection services)
        {
            return services;
        }
    }
}
