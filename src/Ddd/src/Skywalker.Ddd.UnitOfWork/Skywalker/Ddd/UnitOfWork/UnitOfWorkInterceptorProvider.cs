using Skywalker.Aspects.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork
{
    public class UnitOfWorkInterceptorProvider : IInterceptorProvider
    {
        public void Use(IInterceptorChainBuilder builder)
        {
            builder.Use<UnitOfWorkInterceptor>(0);
        }
    }
}
