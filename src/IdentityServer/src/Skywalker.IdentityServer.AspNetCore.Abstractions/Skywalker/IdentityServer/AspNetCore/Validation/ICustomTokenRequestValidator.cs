// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Skywalker.IdentityServer.AspNetCore.Validation.Contexts;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.AspNetCore.Validation
{
    /// <summary>
    /// Allows inserting custom validation logic into token requests
    /// </summary>
    public interface ICustomTokenRequestValidator
    {
        /// <summary>
        /// Custom validation logic for a token request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The validation result
        /// </returns>
        Task ValidateAsync(CustomTokenRequestValidationContext context);
    }
}