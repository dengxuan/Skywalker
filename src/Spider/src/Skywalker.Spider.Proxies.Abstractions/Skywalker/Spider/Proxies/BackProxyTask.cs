using Skywalker.Extensions.WheelTimer.Abstractions;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies
{
    internal class BackProxyTask : IWheelTimerTask
    {
        private readonly Uri _proxy;
        private readonly IProxyPool _pool;

        public BackProxyTask(IProxyPool pool, Uri proxy)
        {
            _pool = pool;
            _proxy = proxy;
        }

        public async Task RunAsync(IWheelTimeout timeout)
        {
            await _pool.BackAsync(_proxy, HttpStatusCode.OK);
        }
    }
}
