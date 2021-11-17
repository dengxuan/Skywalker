namespace Caching.Abstractions
{
    public interface ICachingProvider
    {
        ICaching GetCaching(string name);
    }
}
