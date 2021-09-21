namespace Skywalker.Spider.Abstractions;

public interface ISpiderBuilder
{

    ISpiderBuilder UseSpider<TRequestSupplier, TResponseHandler>() where TRequestSupplier : class, IRequestSupplier where TResponseHandler : class, IResponseHandler;

    ISpiderBuilder UseSpider<TSpider, TRequestSupplier, TResponseHandler>() where TSpider : class, ISpider where TRequestSupplier : class, IRequestSupplier where TResponseHandler : class, IResponseHandler;
}
