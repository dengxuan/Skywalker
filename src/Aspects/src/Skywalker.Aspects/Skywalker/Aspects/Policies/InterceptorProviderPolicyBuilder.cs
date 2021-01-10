using Skywalker.Aspects.Abstractinons;
using System;

namespace Skywalker.Aspects.Policies
{
    internal class InterceptorProviderPolicyBuilder<TInterceptorProvider> : IInterceptorProviderPolicyBuilder
    {
        private readonly InterceptorProviderPolicy _interceptorProviderPolicy;

        public InterceptorProviderPolicyBuilder(Func<IInterceptorProvider> interceptorProviderFactory)
        {
            Check.NotNull(interceptorProviderFactory, nameof(interceptorProviderFactory));
            _interceptorProviderPolicy = new InterceptorProviderPolicy(typeof(TInterceptorProvider), interceptorProviderFactory);
        }

        public IInterceptorProviderPolicyBuilder To<TTarget>(Action<ITargetPolicyBuilder<TTarget>> configure)
        {
            Check.NotNull(configure, nameof(configure));
            var builder = new TargetPolicyBuilder<TTarget>();
            configure.Invoke(builder);
            _interceptorProviderPolicy.TargetPolicies.Add(builder.Build());
            return this;
        }

        public InterceptorProviderPolicy Build() => _interceptorProviderPolicy;
    }
}
