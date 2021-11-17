// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Authentication;
using Skywalker.IdentityServer.AspNetCore.Configuration.DependencyInjection.Options;
using Skywalker.IdentityServer.AspNetCore.Models.Messages;
using Skywalker.IdentityServer.AspNetCore.Extensions;
using Skywalker.IdentityServer.AspNetCore.Validation.Models;
using Skywalker.IdentityServer.AspNetCore.Hosting;
using Skywalker.IdentityServer.AspNetCore.Stores;

namespace Skywalker.IdentityServer.AspNetCore.Endpoints.Results
{
    /// <summary>
    /// Result for endsession
    /// </summary>
    /// <seealso cref="IEndpointResult" />
    public class EndSessionResult : IEndpointResult
    {
        private readonly EndSessionValidationResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndSessionResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="ArgumentNullException">result</exception>
        public EndSessionResult(EndSessionValidationResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        internal EndSessionResult(
            EndSessionValidationResult result,
            IdentityServerOptions options,
            ISystemClock clock,
            IMessageStore<LogoutMessage> logoutMessageStore)
            : this(result)
        {
            _options = options;
            _clock = clock;
            _logoutMessageStore = logoutMessageStore;
        }

        private IdentityServerOptions _options;
        private ISystemClock _clock;
        private IMessageStore<LogoutMessage> _logoutMessageStore;

        private void Init(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _clock = _clock ?? context.RequestServices.GetRequiredService<ISystemClock>();
            _logoutMessageStore = _logoutMessageStore ?? context.RequestServices.GetRequiredService<IMessageStore<LogoutMessage>>();
        }

        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            var validatedRequest = _result.IsError ? null : _result.ValidatedRequest;

            string id = null;

            if (validatedRequest != null)
            {
                var logoutMessage = new LogoutMessage(validatedRequest);
                if (logoutMessage.ContainsPayload)
                {
                    var msg = new Message<LogoutMessage>(logoutMessage, _clock.UtcNow.UtcDateTime);
                    id = await _logoutMessageStore.WriteAsync(msg);
                }
            }

            var redirect = _options.UserInteraction.LogoutUrl;

            if (redirect.IsLocalUrl())
            {
                redirect = context.GetIdentityServerRelativeUrl(redirect);
            }

            if (id != null)
            {
                redirect = redirect.AddQueryString(_options.UserInteraction.LogoutIdParameter, id);
            }

            context.Response.Redirect(redirect);
        }
    }
}
