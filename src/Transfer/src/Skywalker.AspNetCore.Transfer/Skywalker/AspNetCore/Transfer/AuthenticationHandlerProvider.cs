// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.AspNetCore.Transfer.Abstractions;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Implementation of <see cref="ITransferHandlerProvider"/>.
    /// </summary>
    public class AuthenticationHandlerProvider : ITransferHandlerProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="ITransferHandlerProvider"/>.</param>
        public AuthenticationHandlerProvider(ITransferSchemeProvider schemes)
        {
            Schemes = schemes;
        }

        /// <summary>
        /// The <see cref="ITransferHandlerProvider"/>.
        /// </summary>
        public ITransferSchemeProvider Schemes { get; }

        // handler instance cache, need to initialize once per request
        private readonly Dictionary<string, ITransferHandler> _handlerMap = new Dictionary<string, ITransferHandler>(StringComparer.Ordinal);

        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationScheme">The name of the authentication scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        public async Task<ITransferHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
        {
            if (_handlerMap.TryGetValue(authenticationScheme, out var value))
            {
                return value;
            }

            var scheme = await Schemes.GetSchemeAsync(authenticationScheme);
            if (scheme == null)
            {
                return null;
            }
            var handler = (context.RequestServices.GetService(scheme.HandlerType) ??
                ActivatorUtilities.CreateInstance(context.RequestServices, scheme.HandlerType))
                as ITransferHandler;
            if (handler != null)
            {
                await handler.InitializeAsync(scheme, context);
                _handlerMap[authenticationScheme] = handler;
            }
            return handler;
        }
    }
}
