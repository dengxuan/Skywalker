using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Services;

namespace Simple.Application
{
    public class SimpleApplicationService : ApplicationService
    {
        public SimpleApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
    }
}
