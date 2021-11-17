using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skywalker.Ddd.Tracing
{
    public class DefaultCorrelationIdProvider : ICorrelationIdProvider/*, ISingletonDependency*/
    {
        public string Get()
        {
            return CreateNewCorrelationId();
        }

        protected virtual string CreateNewCorrelationId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}