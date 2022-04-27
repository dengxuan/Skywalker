using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Data;

public interface IConnectionStringResolver : ISingletonDependency
{
    string Resolve(string connectionStringName);
}
