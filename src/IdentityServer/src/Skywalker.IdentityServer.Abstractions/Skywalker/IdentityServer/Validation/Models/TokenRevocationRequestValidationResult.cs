﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Skywalker.IdentityServer.Models;

namespace Skywalker.IdentityServer.Validation
{
    /// <summary>
    /// Models the validation result of access tokens and id tokens.
    /// </summary>
    public class TokenRevocationRequestValidationResult : ValidationResult
    {
        /// <summary>
        /// Gets or sets the token type hint.
        /// </summary>
        /// <value>
        /// The token type hint.
        /// </value>
        public string TokenTypeHint { get; set; }
        
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public Client Client { get; set; }
    }
}