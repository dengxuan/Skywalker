// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Transfer.Abstractions
{
    /// <summary>
    /// Used to provide authentication.
    /// </summary>
    public interface ITransferService
    {
        /// <summary>
        /// Authenticate for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        Task<TransferResult> TransferAsync(HttpContext context, string? scheme);

        /// <summary>
        /// Challenge the specified authentication scheme.
        /// An authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="TransferProperties"/>.</param>
        /// <returns>A task.</returns>
        Task ChallengeAsync(HttpContext context, string? scheme, TransferProperties? properties);

        /// <summary>
        /// Forbids the specified authentication scheme.
        /// Forbid is used when an authenticated user attempts to access a resource they are not permitted to access.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="TransferProperties"/>.</param>
        /// <returns>A task.</returns>
        Task ForbidAsync(HttpContext context, string? scheme, TransferProperties? properties);

        /// <summary>
        /// Sign a principal in for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> to sign in.</param>
        /// <param name="properties">The <see cref="TransferProperties"/>.</param>
        /// <returns>A task.</returns>
        Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, TransferProperties? properties);

        /// <summary>
        /// Sign out the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="TransferProperties"/>.</param>
        /// <returns>A task.</returns>
        Task SignOutAsync(HttpContext context, string? scheme, TransferProperties? properties);
    }
}
