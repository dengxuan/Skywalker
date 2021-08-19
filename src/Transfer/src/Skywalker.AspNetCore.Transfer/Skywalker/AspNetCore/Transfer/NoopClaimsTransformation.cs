// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Skywalker.AspNetCore.Transfer.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Default claims transformation is a no-op.
    /// </summary>
    public class NoopClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// Returns the principal unchanged.
        /// </summary>
        /// <param name="principal">The user.</param>
        /// <returns>The principal unchanged.</returns>
        public virtual Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            return Task.FromResult(principal);
        }
    }
}
