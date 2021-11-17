using Skywalker.Lightning.Terminal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    public interface ILightningInvoker
    {
        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <param name="serviceName">service name</param>
        /// <param name="parameters">service parameters </param>
        /// <returns></returns>
        Task<T> InvokeAsync<T>(string serviceName, Dictionary<string, object> parameters);

        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <param name="serviceName">service name</param>
        /// <param name="parameters">service parameters </param>
        /// <returns></returns>
        Task InvokeAsync(string serviceName, Dictionary<string, object> parameters);


        /// <summary>
        ///     add middleware in request
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        ILightningInvoker Use(Func<LightningInvokeMiddleware, LightningInvokeMiddleware> middleware);
    }
}
