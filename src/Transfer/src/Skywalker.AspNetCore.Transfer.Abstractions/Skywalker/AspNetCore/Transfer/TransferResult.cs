// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Contains the result of an Authenticate call
    /// </summary>
    public class TransferResult
    {
        /// <summary>
        /// Creates a new <see cref="TransferResult"/> instance.
        /// </summary>
        protected TransferResult() { }

        /// <summary>
        /// If a ticket was produced, authenticate was successful.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Ticket))]
        public bool Succeeded => Ticket != null;

        /// <summary>
        /// The authentication ticket.
        /// </summary>
        public TransferTicket? Ticket { get; protected set; }

        /// <summary>
        /// Gets the claims-principal with authenticated user identities.
        /// </summary>
        public ClaimsPrincipal? Principal => Ticket?.Principal;

        /// <summary>
        /// Additional state values for the authentication session.
        /// </summary>
        public TransferProperties? Properties { get; protected set; }

        /// <summary>
        /// Holds failure information from the authentication.
        /// </summary>
        public Exception? Failure { get; protected set; }

        /// <summary>
        /// Indicates that there was no information returned for this authentication scheme.
        /// </summary>
        public bool None { get; protected set; }

        /// <summary>
        /// Create a new deep copy of the result
        /// </summary>
        /// <returns>A copy of the result</returns>
        public TransferResult Clone()
        {
            if (None)
            {
                return NoResult();
            }
            if (Failure != null)
            {
                return Fail(Failure, Properties?.Clone());
            }
            if (Succeeded)
            {
                return Success(Ticket!.Clone());
            }
            // This shouldn't happen
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that authentication was successful.
        /// </summary>
        /// <param name="ticket">The ticket representing the authentication result.</param>
        /// <returns>The result.</returns>
        public static TransferResult Success(TransferTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            return new TransferResult() { Ticket = ticket, Properties = ticket.Properties };
        }

        /// <summary>
        /// Indicates that there was no information returned for this authentication scheme.
        /// </summary>
        /// <returns>The result.</returns>
        public static TransferResult NoResult()
        {
            return new TransferResult() { None = true };
        }

        /// <summary>
        /// Indicates that there was a failure during authentication.
        /// </summary>
        /// <param name="failure">The failure exception.</param>
        /// <returns>The result.</returns>
        public static TransferResult Fail(Exception failure)
        {
            return new TransferResult() { Failure = failure };
        }

        /// <summary>
        /// Indicates that there was a failure during authentication.
        /// </summary>
        /// <param name="failure">The failure exception.</param>
        /// <param name="properties">Additional state values for the authentication session.</param>
        /// <returns>The result.</returns>
        public static TransferResult Fail(Exception failure, TransferProperties? properties)
        {
            return new TransferResult() { Failure = failure, Properties = properties };
        }

        /// <summary>
        /// Indicates that there was a failure during authentication.
        /// </summary>
        /// <param name="failureMessage">The failure message.</param>
        /// <returns>The result.</returns>
        public static TransferResult Fail(string failureMessage)
            => Fail(new Exception(failureMessage));

        /// <summary>
        /// Indicates that there was a failure during authentication.
        /// </summary>
        /// <param name="failureMessage">The failure message.</param>
        /// <param name="properties">Additional state values for the authentication session.</param>
        /// <returns>The result.</returns>
        public static TransferResult Fail(string failureMessage, TransferProperties? properties)
            => Fail(new Exception(failureMessage), properties);
    }
}
