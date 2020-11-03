using Skywalker.Lightning.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Proxies
{
    public abstract class LightningProxy
    {
        private readonly ILightningInvoker _LightningInvoker;

        protected LightningProxy(ILightningInvoker LightningInvoker)
        {
            _LightningInvoker = LightningInvoker;
        }

        protected async Task<T> InvokeAsync<T>(string serviceId, Dictionary<string, object> parameters)
        {
            return await _LightningInvoker.InvokeAsync<T>(serviceId, parameters);
        }

        protected T Invoke<T>(string serviceId, Dictionary<string, object> parameters)
        {
            return _LightningInvoker.InvokeAsync<T>(serviceId, parameters).Result;
        }

        protected async Task InvokeVoidAsync(string serviceId, Dictionary<string, object> parameters)
        {
            await _LightningInvoker.InvokeAsync(serviceId, parameters);
        }

        protected void InvokeVoid(string serviceId, Dictionary<string, object> parameters)
        {
            _LightningInvoker.InvokeAsync(serviceId, parameters);
        }
    }
}
