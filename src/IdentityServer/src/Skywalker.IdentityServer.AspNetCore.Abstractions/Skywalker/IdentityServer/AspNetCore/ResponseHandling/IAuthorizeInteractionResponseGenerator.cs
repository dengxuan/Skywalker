// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Skywalker.IdentityServer.AspNetCore.Models.Messages;
using Skywalker.IdentityServer.AspNetCore.ResponseHandling.Models;
using Skywalker.IdentityServer.AspNetCore.Validation.Models;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.AspNetCore.ResponseHandling
{
    /// <summary>
    /// Interface for determining if user must login or consent when making requests to the authorization endpoint.
    /// </summary>
    public interface IAuthorizeInteractionResponseGenerator
    {
        /// <summary>
        /// Processes the interaction logic.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="consent">The consent.</param>
        /// <returns></returns>
        Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request, ConsentResponse consent = null);
    }
}