// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Used to build <see cref="TransferScheme"/>s.
    /// </summary>
    public class TransferSchemeBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the scheme being built.</param>
        public TransferSchemeBuilder(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the scheme being built.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the display name for the scheme being built.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAuthenticationHandler"/> type responsible for this scheme.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        public Type? HandlerType { get; set; }

        /// <summary>
        /// Builds the <see cref="TransferScheme"/> instance.
        /// </summary>
        /// <returns>The <see cref="TransferScheme"/>.</returns>
        public TransferScheme Build()
        {
            if (HandlerType is null)
            {
                throw new InvalidOperationException($"{nameof(HandlerType)} must be configured to build an {nameof(TransferScheme)}.");
            }

            return new TransferScheme(Name, DisplayName, HandlerType);
        }
    }
}
