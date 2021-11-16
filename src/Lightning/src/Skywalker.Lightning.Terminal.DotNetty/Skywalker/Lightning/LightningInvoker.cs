using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Terminal;
using Skywalker.Lightning.Terminal.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Lightning
{
    public class LightningInvoker : ILightningInvoker
    {
        private readonly ILogger<LightningInvoker> _logger;
        private readonly ILightningTerminalFactory _lightningTerminalFactory;
        private readonly Stack<Func<LightningInvokeMiddleware, LightningInvokeMiddleware>> _middlewares;

        public LightningInvoker(ILightningTerminalFactory LightningTerminalFactory, ILogger<LightningInvoker> logger)
        {
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
            var context = new LightningInvokeContext(serviceName, parameters);
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
                var Terminal = await _lightningTerminalFactory.CreateTerminalAsync();
                if (Terminal == null)
                {
                    return new LightningResponse
                    {
                        State = 400,
                        Response = "Server unavailable!"
                    };
                }
                _logger.LogDebug($"invoke: serviceId:{context.ServiceName}, parameters count: {context.Parameters.Count}");

                return await Terminal.SendAsync(new LightningRequest(context.ServiceName, context.Parameters));
            });
        }

        public ILightningInvoker Use(Func<LightningInvokeMiddleware, LightningInvokeMiddleware> middleware)
        {
            _middlewares.Push(middleware);
            return this;
        }
    }
}
