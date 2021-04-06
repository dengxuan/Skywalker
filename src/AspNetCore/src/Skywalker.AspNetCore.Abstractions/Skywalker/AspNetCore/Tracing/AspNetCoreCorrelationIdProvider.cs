using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Tracing;
using System;

namespace Skywalker.AspNetCore.Tracing
{
    public class AspNetCoreCorrelationIdProvider : ICorrelationIdProvider, ITransientDependency
    {
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected SkywalkerCorrelationIdOptions Options { get; }

        public AspNetCoreCorrelationIdProvider(
            IHttpContextAccessor httpContextAccessor,
            IOptions<SkywalkerCorrelationIdOptions> options)
        {
            HttpContextAccessor = httpContextAccessor;
            Options = options.Value;
        }

        public virtual string Get()
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers == null)
            {
                return CreateNewCorrelationId();
            }

            string correlationId = HttpContextAccessor.HttpContext.Request.Headers[Options.HttpHeaderName];

            if (correlationId.IsNullOrEmpty())
            {
                lock (HttpContextAccessor.HttpContext.Request.Headers)
                {
                    if (correlationId.IsNullOrEmpty())
                    {
                        correlationId = CreateNewCorrelationId();
                        HttpContextAccessor.HttpContext.Request.Headers[Options.HttpHeaderName] = correlationId;
                    }
                }
            }

            return correlationId;
        }

        protected virtual string CreateNewCorrelationId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
