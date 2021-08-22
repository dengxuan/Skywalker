using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Spider.Abstractions
{
    public interface ISpiderBuilder
    {
        IServiceCollection Services { get; }
    }
}
