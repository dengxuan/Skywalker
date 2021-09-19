using Microsoft.Extensions.DependencyInjection;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Pipelines.Abstractions;
using System;
using System.Collections.Generic;

namespace Skywalker.Spider
{
    internal class SpiderBuilder : ISpiderBuilder
    {
        private readonly Dictionary<Type, Action<IPipelineChainBuilder>> _spiders = new();

        private readonly IServiceCollection _services;

        public SpiderBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ISpiderBuilder UseSpider<TRequestSupplier>(Action<IPipelineChainBuilder> pipeline) where TRequestSupplier : class, IRequestSupplier
        {
            return UseSpider<DefaultSpider<TRequestSupplier>, TRequestSupplier>(pipeline);
        }

        public ISpiderBuilder UseSpider<TSpider, TRequestSupplier>(Action<IPipelineChainBuilder> pipeline) where TSpider : class, ISpider<TRequestSupplier> where TRequestSupplier : class, IRequestSupplier
        {
            _services.AddSingleton<TRequestSupplier>();
            _spiders.Add(typeof(TSpider), pipeline);
            return this;
        }

        public IEnumerable<ISpider<IRequestSupplier>> CreateSpider(IServiceProvider serviceProvider)
        {
            var spiders = new List<ISpider<IRequestSupplier>>();
            foreach (var item in _spiders)
            {
                var pipelineChainBuilder = serviceProvider.GetRequiredService<IPipelineChainBuilder>();
                item.Value(pipelineChainBuilder);
                ISpider<IRequestSupplier> spider = (ISpider<IRequestSupplier>)ActivatorUtilities.CreateInstance(serviceProvider, item.Key, new[] { pipelineChainBuilder });
                spiders.Add(spider);
            }
            return spiders;
        }
    }
}
