// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Extensions.Logging;
using Skywalker.IdentityServer.AspNetCore.Services;
using Skywalker.IdentityServer.AspNetCore.Validation.Models;
using Skywalker.IdentityServer.Domain.Stores;

namespace Skywalker.IdentityServer.AspNetCore.Validation.Default
{
    /// <summary>
    /// Default custom token validator
    /// </summary>
    public class DefaultCustomTokenValidator : ICustomTokenValidator
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The user service
        /// </summary>
        protected readonly IProfileService Profile;

        /// <summary>
        /// The client store
        /// </summary>
        protected readonly IClientStore Clients;

        /// <summary>
        /// Custom validation logic for access tokens.
        /// </summary>
        /// <param name="result">The validation result so far.</param>
        /// <returns>
        /// The validation result
        /// </returns>
        public virtual Task<TokenValidationResult> ValidateAccessTokenAsync(TokenValidationResult result)
        {
            return Task.FromResult(result);
        }

        /// <summary>
        /// Custom validation logic for identity tokens.
        /// </summary>
        /// <param name="result">The validation result so far.</param>
        /// <returns>
        /// The validation result
        /// </returns>
        public virtual Task<TokenValidationResult> ValidateIdentityTokenAsync(TokenValidationResult result)
        {
            return Task.FromResult(result);
        }
    }
}