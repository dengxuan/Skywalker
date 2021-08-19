// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.AspNetCore.Transfer.Abstractions;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Extension methods to expose Authentication on HttpContext.
    /// </summary>
    public static class TransferHttpContextExtensions
    {
        /// <summary>
        /// Authenticate the current request using the default authentication scheme.
        /// The default authentication scheme can be configured using <see cref="TransferOptions.DefaultAuthenticateScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <returns>The <see cref="TransferResult"/>.</returns>
        public static Task<TransferResult> AuthenticateAsync(this HttpContext context) =>
            context.AuthenticateAsync(scheme: null);

        /// <summary>
        /// Authenticate the current request using the specified scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The <see cref="TransferResult"/>.</returns>
        public static Task<TransferResult> AuthenticateAsync(this HttpContext context, string? scheme) =>
            context.RequestServices.GetRequiredService<ITransferService>().TransferAsync(context, scheme);

        /// <summary>
        /// Challenge the current request using the specified scheme.
        /// An authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        public static Task ChallengeAsync(this HttpContext context, string? scheme) =>
            context.ChallengeAsync(scheme, properties: null);

        /// <summary>
        /// Challenge the current request using the default challenge scheme.
        /// An authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication.
        /// The default challenge scheme can be configured using <see cref="TransferOptions.DefaultChallengeScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <returns>The task.</returns>
        public static Task ChallengeAsync(this HttpContext context) =>
            context.ChallengeAsync(scheme: null, properties: null);

        /// <summary>
        /// Challenge the current request using the default challenge scheme.
        /// An authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication.
        /// The default challenge scheme can be configured using <see cref="TransferOptions.DefaultChallengeScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task ChallengeAsync(this HttpContext context, TransferProperties? properties) =>
            context.ChallengeAsync(scheme: null, properties: properties);

        /// <summary>
        /// Challenge the current request using the specified scheme.
        /// An authentication challenge can be issued when an unauthenticated user requests an endpoint that requires authentication.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task ChallengeAsync(this HttpContext context, string? scheme, TransferProperties? properties) =>
            context.RequestServices.GetRequiredService<ITransferService>().ChallengeAsync(context, scheme, properties);

        /// <summary>
        /// Forbid the current request using the specified scheme.
        /// Forbid is used when an authenticated user attempts to access a resource they are not permitted to access.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The task.</returns>
        public static Task ForbidAsync(this HttpContext context, string? scheme) =>
            context.ForbidAsync(scheme, properties: null);

        /// <summary>
        /// Forbid the current request using the default forbid scheme.
        /// Forbid is used when an authenticated user attempts to access a resource they are not permitted to access.
        /// The default forbid scheme can be configured using <see cref="TransferOptions.DefaultForbidScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <returns>The task.</returns>
        public static Task ForbidAsync(this HttpContext context) =>
            context.ForbidAsync(scheme: null, properties: null);

        /// <summary>
        /// Forbid the current request using the default forbid scheme.
        /// Forbid is used when an authenticated user attempts to access a resource they are not permitted to access.
        /// The default forbid scheme can be configured using <see cref="TransferOptions.DefaultForbidScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task ForbidAsync(this HttpContext context, TransferProperties? properties) =>
            context.ForbidAsync(scheme: null, properties: properties);

        /// <summary>
        /// Forbid the current request using the specified scheme.
        /// Forbid is used when an authenticated user attempts to access a resource they are not permitted to access.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task ForbidAsync(this HttpContext context, string? scheme, TransferProperties? properties) =>
            context.RequestServices.GetRequiredService<ITransferService>().ForbidAsync(context, scheme, properties);

        /// <summary>
        /// Sign in a principal for the specified scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="principal">The user.</param>
        /// <returns>The task.</returns>
        public static Task SignInAsync(this HttpContext context, string? scheme, ClaimsPrincipal principal) =>
            context.SignInAsync(scheme, principal, properties: null);

        /// <summary>
        /// Sign in a principal for the default authentication scheme.
        /// The default scheme for signing in can be configured using <see cref="TransferOptions.DefaultSignInScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="principal">The user.</param>
        /// <returns>The task.</returns>
        public static Task SignInAsync(this HttpContext context, ClaimsPrincipal principal) =>
            context.SignInAsync(scheme: null, principal: principal, properties: null);

        /// <summary>
        /// Sign in a principal for the default authentication scheme.
        /// The default scheme for signing in can be configured using <see cref="TransferOptions.DefaultForbidScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="principal">The user.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task SignInAsync(this HttpContext context, ClaimsPrincipal principal, TransferProperties? properties) =>
            context.SignInAsync(scheme: null, principal: principal, properties: properties);

        /// <summary>
        /// Sign in a principal for the specified scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="principal">The user.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task SignInAsync(this HttpContext context, string? scheme, ClaimsPrincipal principal, TransferProperties? properties) =>
            context.RequestServices.GetRequiredService<ITransferService>().SignInAsync(context, scheme, principal, properties);

        /// <summary>
        /// Sign out a principal for the default authentication scheme.
        /// The default scheme for signing out can be configured using <see cref="TransferOptions.DefaultSignOutScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <returns>The task.</returns>
        public static Task SignOutAsync(this HttpContext context) => context.SignOutAsync(scheme: null, properties: null);

        /// <summary>
        /// Sign out a principal for the default authentication scheme.
        /// The default scheme for signing out can be configured using <see cref="TransferOptions.DefaultSignOutScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task SignOutAsync(this HttpContext context, TransferProperties? properties) => context.SignOutAsync(scheme: null, properties: properties);

        /// <summary>
        /// Sign out a principal for the specified scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The task.</returns>
        public static Task SignOutAsync(this HttpContext context, string? scheme) => context.SignOutAsync(scheme, properties: null);

        /// <summary>
        /// Sign out a principal for the specified scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="properties">The <see cref="TransferProperties"/> properties.</param>
        /// <returns>The task.</returns>
        public static Task SignOutAsync(this HttpContext context, string? scheme, TransferProperties? properties) =>
            context.RequestServices.GetRequiredService<ITransferService>().SignOutAsync(context, scheme, properties);

        /// <summary>
        /// Authenticates the request using the specified scheme and returns the value for the token.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <param name="tokenName">The name of the token.</param>
        /// <returns>The value of the token if present.</returns>
        public static Task<string?> GetTokenAsync(this HttpContext context, string? scheme, string tokenName) =>
            context.RequestServices.GetRequiredService<ITransferService>().GetTokenAsync(context, scheme, tokenName);

        /// <summary>
        /// Authenticates the request using the default authentication scheme and returns the value for the token.
        /// The default authentication scheme can be configured using <see cref="TransferOptions.DefaultAuthenticateScheme"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        /// <param name="tokenName">The name of the token.</param>
        /// <returns>The value of the token if present.</returns>
        public static Task<string?> GetTokenAsync(this HttpContext context, string tokenName) =>
            context.RequestServices.GetRequiredService<ITransferService>().GetTokenAsync(context, tokenName);
    }
}
