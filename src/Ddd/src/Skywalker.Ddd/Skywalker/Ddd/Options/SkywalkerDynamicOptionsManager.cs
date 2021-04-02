using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Skywalker.Ddd.Options
{
    public abstract class SkywalkerDynamicOptionsManager<T> : OptionsManager<T>
        where T : class
    {
        protected SkywalkerDynamicOptionsManager(IOptionsFactory<T> factory)
            : base(factory)
        {

        }

        public Task SetAsync() => SetAsync(Microsoft.Extensions.Options.Options.DefaultName);

        public virtual Task SetAsync(string name)
        {
            return OverrideOptionsAsync(name, base.Get(name));
        }

        protected abstract Task OverrideOptionsAsync(string name, T options);
    }
}
