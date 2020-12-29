using Skywalker.Aspects.Interceptors;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Skywalker.Aspects
{
    public class AspectsOptions : IOptions<AspectsOptions>
    {
        public List<IInterceptorProvider> Providers { get; } = new List<IInterceptorProvider>();

        public AspectsOptions Value => this;

        public AspectsOptions Use<TInterceptorProvider>(TInterceptorProvider provider) where TInterceptorProvider : IInterceptorProvider
        {
            Providers.Add(provider);
            return this;
        }
    }
}
