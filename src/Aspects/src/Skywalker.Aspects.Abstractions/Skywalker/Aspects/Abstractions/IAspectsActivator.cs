namespace Skywalker.Aspects.Abstractions;

public interface IAspectsActivator<TProxy>
{
    /// <summary>
    /// Get or create TProxy instance from IServiceProvider
    /// </summary>
    /// <param name="interceptor">The public of TProxy methods interceptor</param>
    /// <returns>The TProxy instance if secceed</returns>
    TProxy CreateInstance(IAspectsInterceptor interceptor);
}
