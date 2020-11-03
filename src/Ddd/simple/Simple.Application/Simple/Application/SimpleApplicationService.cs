using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Application
{
    public class SimpleApplicationService : ApplicationService
    {
        public SimpleApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
    }
}
