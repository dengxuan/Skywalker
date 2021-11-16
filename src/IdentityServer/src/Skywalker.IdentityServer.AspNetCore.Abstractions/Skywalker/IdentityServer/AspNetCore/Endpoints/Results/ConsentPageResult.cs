// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.IdentityServer.AspNetCore.Configuration.DependencyInjection.Options;
using Skywalker.IdentityServer.AspNetCore.Extensions;
using Skywalker.IdentityServer.AspNetCore.Hosting;
using Skywalker.IdentityServer.AspNetCore.Models.Messages;
using Skywalker.IdentityServer.AspNetCore.Stores;
using Skywalker.IdentityServer.AspNetCore.Validation.Models;

namespace Skywalker.IdentityServer.AspNetCore.Endpoints.Results
{
    /// <summary>
    /// Result for consent page
    /// </summary>
    /// <seealso cref="IEndpointResult" />
    public class ConsentPageResult : IEndpointResult
    {
        private readonly ValidatedAuthorizeRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentPageResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="ArgumentNullException">request</exception>
        public ConsentPageResult(ValidatedAuthorizeRequest request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        internal ConsentPageResult(
            ValidatedAuthorizeRequest request,
            IdentityServerOptions options,
            IAuthorizationParametersMessageStore authorizationParametersMessageStore = null)
            : this(request)
        {
            _options = options;
            _authorizationParametersMessageStore = authorizationParametersMessageStore;
        }

        private IdentityServerOptions _options;
        private IAuthorizationParametersMessageStore _authorizationParametersMessageStore;

        private void Init(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _authorizationParametersMessageStore = _authorizationParametersMessageStore ?? context.RequestServices.GetService<IAuthorizationParametersMessageStore>();
        }

        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            var returnUrl = context.GetIdentityServerBasePath().EnsureTrailingSlash() + Constants.ProtocolRoutePaths.AuthorizeCallback;
            if (_authorizationParametersMessageStore != null)
            {
                var msg = new Message<IDictionary<string, string[]>>(_request.Raw.ToFullDictionary());
                var id = await _authorizationParametersMessageStore.WriteAsync(msg);
                returnUrl = returnUrl.AddQueryString(Constants.AuthorizationParamsStore.MessageStoreIdParameterName, id);
            }
            else
            {
                returnUrl = returnUrl.AddQueryString(_request.Raw.ToQueryString());
            }

            var consentUrl = _options.UserInteraction.ConsentUrl;
            if (!consentUrl.IsLocalUrl())
            {
                // this converts the relative redirect path to an absolute one if we're 
                // redirecting to a different server
                returnUrl = context.GetIdentityServerHost().EnsureTrailingSlash() + returnUrl.RemoveLeadingSlash();
            }

            var url = consentUrl.AddQueryString(_options.UserInteraction.ConsentReturnUrlParameter, returnUrl);
            context.Response.RedirectToAbsoluteUrl(url);
        }
    }
}