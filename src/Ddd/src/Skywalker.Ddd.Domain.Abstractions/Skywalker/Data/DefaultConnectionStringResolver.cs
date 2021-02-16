using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Skywalker.Data
{
    public class DefaultConnectionStringResolver : IConnectionStringResolver, ITransientDependency
    {
        protected SkywalkerDbConnectionOptions Options { get; }

        public DefaultConnectionStringResolver(IOptionsSnapshot<SkywalkerDbConnectionOptions> options)
        {
            Options = options.Value;
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
}