// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Transfer.Abstractions
{
    /// <summary>
    /// Used to determine if a handler supports SignOut.
    /// </summary>
    public interface ITransferSignOutHandler : ITransferHandler
    {
        /// <summary>
        /// Signout behavior.
        /// </summary>
        /// <param name="properties">The <see cref="TransferProperties"/> that contains the extra meta-data arriving with the authentication.</param>
        /// <returns>A task.</returns>
        Task SignOutAsync(TransferProperties? properties);
    }

}
