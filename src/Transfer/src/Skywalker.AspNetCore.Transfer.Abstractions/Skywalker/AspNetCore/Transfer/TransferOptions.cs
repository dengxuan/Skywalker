// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Skywalker.AspNetCore.Transfer.Abstractions;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Options to configure authentication.
    /// </summary>
    public class TransferOptions
    {
        private readonly IList<TransferSchemeBuilder> _schemes = new List<TransferSchemeBuilder>();

        /// <summary>
        /// Returns the schemes in the order they were added (important for request handling priority)
        /// </summary>
        public IEnumerable<TransferSchemeBuilder> Schemes => _schemes;

        /// <summary>
        /// Maps schemes by name.
        /// </summary>
        public IDictionary<string, TransferSchemeBuilder> SchemeMap { get; } = new Dictionary<string, TransferSchemeBuilder>(StringComparer.Ordinal);

        /// <summary>
        /// Adds an <see cref="TransferScheme"/>.
        /// </summary>
        /// <param name="name">The name of the scheme being added.</param>
        /// <param name="configureBuilder">Configures the scheme.</param>
        public void AddScheme(string name, Action<TransferSchemeBuilder> configureBuilder)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureBuilder == null)
            {
                throw new ArgumentNullException(nameof(configureBuilder));
            }
            if (SchemeMap.ContainsKey(name))
            {
                throw new InvalidOperationException("Scheme already exists: " + name);
            }

            var builder = new TransferSchemeBuilder(name);
            configureBuilder(builder);
            _schemes.Add(builder);
            SchemeMap[name] = builder;
        }

        /// <summary>
        /// Adds an <see cref="TransferScheme"/>.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="ITransferHandler"/> responsible for the scheme.</typeparam>
        /// <param name="name">The name of the scheme being added.</param>
        /// <param name="displayName">The display name for the scheme.</param>
        public void AddScheme<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]THandler>(string name, string? displayName) where THandler : ITransferHandler
            => AddScheme(name, b =>
            {
                b.DisplayName = displayName;
                b.HandlerType = typeof(THandler);
            });

        /// <summary>
        /// Used as the fallback default scheme for all the other defaults.
        /// </summary>
        public string? DefaultScheme { get; set; }

        /// <summary>
        /// Used as the default scheme by <see cref="ITransferService.TransferAsync(HttpContext, string)"/>.
        /// </summary>
        public string? DefaultAuthenticateScheme { get; set; }

        /// <summary>
        /// Used as the default scheme by <see cref="ITransferService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, TransferProperties)"/>.
        /// </summary>
        public string? DefaultSignInScheme { get; set; }

        /// <summary>
        /// Used as the default scheme by <see cref="ITransferService.SignOutAsync(HttpContext, string, TransferProperties)"/>.
        /// </summary>
        public string? DefaultSignOutScheme { get; set; }

        /// <summary>
        /// Used as the default scheme by <see cref="ITransferService.ChallengeAsync(HttpContext, string, TransferProperties)"/>.
        /// </summary>
        public string? DefaultChallengeScheme { get; set; }

        /// <summary>
        /// Used as the default scheme by <see cref="ITransferService.ForbidAsync(HttpContext, string, TransferProperties)"/>.
        /// </summary>
        public string? DefaultForbidScheme { get; set; }

        /// <summary>
        /// If true, SignIn should throw if attempted with a user is not authenticated.
        /// A user is considered authenticated if <see cref="ClaimsIdentity.IsAuthenticated"/> returns <see langword="true" /> for the <see cref="ClaimsPrincipal"/> associated with the HTTP request.
        /// </summary>
        public bool RequireAuthenticatedSignIn { get; set; } = true;
    }
}
