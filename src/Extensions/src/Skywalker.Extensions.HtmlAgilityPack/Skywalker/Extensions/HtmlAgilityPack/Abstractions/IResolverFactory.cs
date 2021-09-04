namespace Skywalker.Extensions.HtmlAgilityPack.Abstractions
{
    public interface IResolverFactory
    {
        IResolver CreateResolver(string contentType);
    }
}
