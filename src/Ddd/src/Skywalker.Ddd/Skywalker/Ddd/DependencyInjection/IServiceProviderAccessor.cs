using System;

namespace Skywalker.Ddd.DependencyInjection
{
    public interface IServiceProviderAccessor
    {
        IServiceProvider? ServiceProvider { get; }
    }
}
