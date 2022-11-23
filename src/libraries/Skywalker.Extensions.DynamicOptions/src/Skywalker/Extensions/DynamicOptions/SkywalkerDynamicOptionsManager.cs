using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.DynamicOptions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SkywalkerDynamicOptionsManager<T> : OptionsManager<T>
    where T : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    protected SkywalkerDynamicOptionsManager(IOptionsFactory<T> factory)
        : base(factory)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task SetAsync() => SetAsync(Options.DefaultName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual Task SetAsync(string name)
    {
        return OverrideOptionsAsync(name, base.Get(name));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected abstract Task OverrideOptionsAsync(string name, T options);
}
