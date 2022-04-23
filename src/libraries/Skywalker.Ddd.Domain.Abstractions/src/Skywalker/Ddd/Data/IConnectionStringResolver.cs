using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Data;

public interface IConnectionStringResolver : ITransientDependency
{
    string Resolve(string connectionStringName);
}
