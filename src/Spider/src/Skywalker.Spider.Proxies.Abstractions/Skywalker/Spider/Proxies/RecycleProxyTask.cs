using Skywalker.Extensions.WheelTimer;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies
{
    internal class RecycleProxyTask : IWheelTimerTask
    {
        private readonly Uri _proxy;
        private readonly IProxyPool _pool;

        public RecycleProxyTask(IProxyPool pool, Uri proxy)
        {
            _pool = pool;
            _proxy = proxy;
        }

        public Task RunAsync(IWheelTimeout timeout)
        {
            return _pool.RecycleAsync(_proxy);
        }
    }
}
