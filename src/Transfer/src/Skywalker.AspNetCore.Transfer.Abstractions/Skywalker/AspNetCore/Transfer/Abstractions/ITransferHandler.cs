// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Transfer.Abstractions
{
    /// <summary>
    /// Created per request to handle authentication for a particular scheme.
    /// </summary>
    public interface ITransferHandler
    {
        /// <summary>
        /// Initialize the authentication handler. The handler should initialize anything it needs from the request and scheme as part of this method.
        /// </summary>
        /// <param name="scheme">The <see cref="TransferScheme"/> scheme.</param>
        /// <param name="context">The <see cref="HttpContext"/> context.</param>
        Task InitializeAsync(TransferScheme scheme, HttpContext context);

        /// <summary>
        /// Authenticate the current request.
        /// </summary>
        /// <returns>The <see cref="TransferResult"/> result.</returns>
        Task<TransferResult> TransferAsync();

        /// <summary>
        /// Challenge the current request.
        /// </summary>
        /// <param name="properties">The <see cref="TransferProperties"/> that contains the extra meta-data arriving with the authentication.</param>
        Task ChallengeAsync(TransferProperties? properties);

        /// <summary>
        /// Forbid the current request.
        /// </summary>
        /// <param name="properties">The <see cref="TransferProperties"/> that contains the extra meta-data arriving with the authentication.</param>
        Task ForbidAsync(TransferProperties? properties);
    }
}
