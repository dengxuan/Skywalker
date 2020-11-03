using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Abstractions;
using Skywalker.Lightning.Terminal.Abstractions;
using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using Skywalker.Lightning.LoadBalance;
using Skywalker.Lightning.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Lightning
{
    public class LightningInvoker : ILightningInvoker
    {
        private readonly IAddressSelector _addressSelector;
        private readonly ILogger<LightningInvoker> _logger;
        private readonly ILightningClusterDescriptor _lightningCluster;
        private readonly ILightningTerminalFactory _lightningTerminalFactory;
        private readonly Stack<Func<LightningInvokeMiddleware, LightningInvokeMiddleware>> _middlewares;

        public LightningInvoker(ILightningClusterDescriptor LightningCluster,
            IAddressSelector addressSelector,
            ILightningTerminalFactory LightningTerminalFactory,
            ILogger<LightningInvoker> logger)
        {
            _lightningCluster = LightningCluster;
            _addressSelector = addressSelector;
            _lightningTerminalFactory = LightningTerminalFactory;
            _logger = logger;
            _middlewares = new Stack<Func<LightningInvokeMiddleware, LightningInvokeMiddleware>>();
        }


        public Task InvokeAsync(string serviceName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> InvokeAsync<T>(string serviceName, Dictionary<string, object> parameters)
        {
            ILightningCluster cluster = await GetClusterByServiceNameAsync(serviceName);
            if (cluster == null)
            {
                _logger.LogDebug($"{serviceName} 404, no service cluster is found");
                return default;
            }

            var result = await InvokeAsync<T>(cluster, serviceName, parameters);
            return result;

        }

        protected async Task<T?> InvokeAsync<T>(ILightningCluster cluster, string serviceName, Dictionary<string, object> parameters)
        {
            var context = new LightningInvokeContext(cluster, serviceName, parameters);
            try
            {
                var lastInvoke = GetLastInvoke();

                foreach (var mid in _middlewares)
                {
                    lastInvoke = mid(lastInvoke);
                }
                var result = await lastInvoke(context);
                return (T)result.Response!;
            }
            catch (Exception ex)
            {
                ex.ReThrow();
            }
            return default;
        }

        private LightningInvokeMiddleware GetLastInvoke()
        {
            return new LightningInvokeMiddleware(async context =>
            {
                var Terminal = await _lightningTerminalFactory.CreateTerminalAsync(context.Cluster);
                if (Terminal == null)
                {
                    return new LightningResponse
                    {
                        State = 400,
                        Response = "Server unavailable!"
                    };
                }
                _logger.LogDebug($"invoke: serviceId:{context.Cluster}, parameters count: {context.Parameters.Count}");

                return await Terminal.SendAsync(new LightningRequest(context.ServiceName, context.Parameters));
            });
        }

        private async Task<ILightningCluster> GetClusterByServiceNameAsync(string serviceName)
        {
            ILightningCluster cluster = await _addressSelector.GetAddressAsync(_lightningCluster, serviceName);
            return cluster;
        }

        public ILightningInvoker Use(Func<LightningInvokeMiddleware, LightningInvokeMiddleware> middleware)
        {
            _middlewares.Push(middleware);
            return this;
        }
    }
}
