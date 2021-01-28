using Microsoft.Extensions.DependencyInjection;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Skywalker.Application.Services;
using Skywalker.Aspects;
using Skywalker.Ddd.UnitOfWork;
using Skywalker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Application
{
    [Interceptor(typeof(UnitOfWorkInterceptor))]
    public class SimpleApplicationService : ApplicationService
    {
        public SimpleApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
    }
}
