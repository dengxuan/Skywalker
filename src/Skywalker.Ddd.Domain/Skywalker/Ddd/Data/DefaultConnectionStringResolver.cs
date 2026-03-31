using Microsoft.Extensions.Configuration;

namespace Skywalker.Ddd.Data;

public class DefaultConnectionStringResolver : IConnectionStringResolver
{
    protected SkywalkerDbConnectionOptions Options { get; }

    public DefaultConnectionStringResolver(IConfiguration configuration)
    {
        Options = new SkywalkerDbConnectionOptions();
        configuration.Bind(Options);
    }

    public virtual string Resolve(string connectionStringName)
    {
        //Get module specific value if provided
        if (!connectionStringName!.IsNullOrEmpty())
        {
            var moduleConnString = Options.ConnectionStrings.GetOrDefault(connectionStringName);
            if (!moduleConnString.IsNullOrEmpty())
            {
                return moduleConnString!;
            }
        }

        //Get default value
        return Options.ConnectionStrings.Default;
    }
}
