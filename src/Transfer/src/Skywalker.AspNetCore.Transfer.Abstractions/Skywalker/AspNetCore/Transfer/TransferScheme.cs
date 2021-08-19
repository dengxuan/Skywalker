// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Skywalker.AspNetCore.Transfer.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// AuthenticationSchemes assign a name to a specific <see cref="ITransferHandler"/>
    /// handlerType.
    /// </summary>
    public class TransferScheme
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TransferScheme"/>.
        /// </summary>
        /// <param name="name">The name for the authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication scheme.</param>
        /// <param name="handlerType">The <see cref="ITransferHandler"/> type that handles this scheme.</param>
        public TransferScheme(string name, string? displayName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            if (!typeof(ITransferHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException("handlerType must implement IAuthenticationHandler.");
            }

            Name = name;
            HandlerType = handlerType;
            DisplayName = displayName;
        }

        /// <summary>
        /// The name of the authentication scheme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The display name for the scheme. Null is valid and used for non user facing schemes.
        /// </summary>
        public string? DisplayName { get; }

        /// <summary>
        /// The <see cref="ITransferHandler"/> type that handles this scheme.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        public Type HandlerType { get; }
    }
}
