using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Services;
using System;

namespace Simple.Application
{
    public class SimpleApplicationService : ApplicationService
    {
        public SimpleApplicationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
