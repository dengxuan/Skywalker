namespace Skywalker.Lightning.Proxies.Abstractions
{
    /// <summary>
    ///     serice proxy
    /// </summary>
    public interface ILightningProxyFactory
    {
        /// <summary>
        ///     get specify service proxy by service type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>() where T : class;
    }
}
