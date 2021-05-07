// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Contains user identity information as well as additional authentication state.
    /// </summary>
    public class TransferTicket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferTicket"/> class
        /// </summary>
        /// <param name="principal">the <see cref="ClaimsPrincipal"/> that represents the authenticated user.</param>
        /// <param name="properties">additional properties that can be consumed by the user or runtime.</param>
        /// <param name="authenticationScheme">the authentication scheme that was responsible for this ticket.</param>
        public TransferTicket(ClaimsPrincipal principal, TransferProperties? properties, string authenticationScheme)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            AuthenticationScheme = authenticationScheme;
            Principal = principal;
            Properties = properties ?? new TransferProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferTicket"/> class
        /// </summary>
        /// <param name="principal">the <see cref="ClaimsPrincipal"/> that represents the authenticated user.</param>
        /// <param name="authenticationScheme">the authentication scheme that was responsible for this ticket.</param>
        public TransferTicket(ClaimsPrincipal principal, string authenticationScheme) 
            : this(principal, properties: null, authenticationScheme: authenticationScheme)
        { }

        /// <summary>
        /// Gets the authentication scheme that was responsible for this ticket.
        /// </summary>
        public string AuthenticationScheme { get; }

        /// <summary>
        /// Gets the claims-principal with authenticated user identities.
        /// </summary>
        public ClaimsPrincipal Principal { get; }

        /// <summary>
        /// Additional state values for the authentication session.
        /// </summary>
        public TransferProperties Properties { get; }

        /// <summary>
        /// Returns a copy of the ticket.
        /// </summary>
        /// <remarks>
        /// The method clones the <see cref="Principal"/> by calling <see cref="ClaimsIdentity.Clone"/> on each of the <see cref="ClaimsPrincipal.Identities"/>.
        /// </remarks>
        /// <returns>A copy of the ticket</returns>
        public TransferTicket Clone()
        {
            var principal = new ClaimsPrincipal();
            foreach (var identity in Principal.Identities)
            {
                principal.AddIdentity(identity.Clone());
            }
            return new TransferTicket(principal, Properties.Clone(), AuthenticationScheme);
        }
    }
}
