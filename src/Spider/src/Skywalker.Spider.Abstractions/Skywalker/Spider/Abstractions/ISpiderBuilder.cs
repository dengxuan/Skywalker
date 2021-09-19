using Skywalker.Spider.Pipelines.Abstractions;
using System;
using System.Collections.Generic;

namespace Skywalker.Spider.Abstractions;

public interface ISpiderBuilder
{

    ISpiderBuilder UseSpider<TRequestSupplier>(Action<IPipelineChainBuilder> pipeline) where TRequestSupplier : class, IRequestSupplier;

    ISpiderBuilder UseSpider<TSpider, TRequestSupplier>(Action<IPipelineChainBuilder> pipeline) where TSpider : class, ISpider<TRequestSupplier> where TRequestSupplier : class, IRequestSupplier;

    internal IEnumerable<ISpider<IRequestSupplier>> CreateSpider(IServiceProvider serviceProvider);
}
