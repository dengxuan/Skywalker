namespace Skywalker.Extensions.Caching.Abstractions
{
    public interface ICachingProvider
    {
        ICaching GetCaching(string name);
    }
}
