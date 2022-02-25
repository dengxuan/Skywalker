namespace Skywalker.Caching.Abstractions;

public interface ICachingProvider
{
    ICaching GetCaching(string name);
}
