using Microsoft.Extensions.DependencyInjection;
using Skywalker.Spider.Abstractions;

namespace Skywalker.Spider
{
    internal class SpiderBuilder : ISpiderBuilder
    {
        private readonly IServiceCollection _services;

        public SpiderBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ISpiderBuilder UseSpider<TRequestSupplier, TResponseHandler>()
            where TRequestSupplier : class, IRequestSupplier
            where TResponseHandler : class, IResponseHandler
        {
            return UseSpider<DefaultSpider<TRequestSupplier, TResponseHandler>, TRequestSupplier, TResponseHandler>();
        }

        public ISpiderBuilder UseSpider<TSpider, TRequestSupplier, TResponseHandler>()
            where TSpider : class, ISpider
            where TRequestSupplier : class, IRequestSupplier
            where TResponseHandler : class, IResponseHandler
        {
            _services.AddSingleton<TRequestSupplier>();
            _services.AddSingleton<TResponseHandler>();
            _services.AddSingleton<ISpider, TSpider>();
            return this;
        }
    }
}
