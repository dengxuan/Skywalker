// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Transfer.Abstractions
{
    /// <summary>
    /// Responsible for managing what authenticationSchemes are supported.
    /// </summary>
    public interface ITransferSchemeProvider
    {
        /// <summary>
        /// Returns all currently registered <see cref="TransferScheme"/>s.
        /// </summary>
        /// <returns>All currently registered <see cref="TransferScheme"/>s.</returns>
        Task<IEnumerable<TransferScheme>> GetAllSchemesAsync();

        /// <summary>
        /// Returns the <see cref="TransferScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        Task<TransferScheme?> GetSchemeAsync(string name);

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.TransferAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultAuthenticateScheme"/>.
        /// Otherwise, this will fallback to <see cref="TransferOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.TransferAsync(HttpContext, string)"/>.</returns>
        Task<TransferScheme?> GetDefaultAuthenticateSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.ChallengeAsync(HttpContext, string, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultChallengeScheme"/>.
        /// Otherwise, this will fallback to <see cref="TransferOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.ChallengeAsync(HttpContext, string, TransferProperties)"/>.</returns>
        Task<TransferScheme?> GetDefaultChallengeSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.ForbidAsync(HttpContext, string, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultForbidScheme"/>.
        /// Otherwise, this will fallback to <see cref="GetDefaultChallengeSchemeAsync"/> .
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.ForbidAsync(HttpContext, string, TransferProperties)"/>.</returns>
        Task<TransferScheme?> GetDefaultForbidSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultSignInScheme"/>.
        /// Otherwise, this will fallback to <see cref="TransferOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, TransferProperties)"/>.</returns>
        Task<TransferScheme?> GetDefaultSignInSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.SignOutAsync(HttpContext, string, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultSignOutScheme"/>.
        /// Otherwise, this will fallback to <see cref="GetDefaultSignInSchemeAsync"/> .
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.SignOutAsync(HttpContext, string, TransferProperties)"/>.</returns>
        Task<TransferScheme?> GetDefaultSignOutSchemeAsync();

        /// <summary>
        /// Registers a scheme for use by <see cref="ITransferService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        void AddScheme(TransferScheme scheme);

        /// <summary>
        /// Registers a scheme for use by <see cref="ITransferService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns>true if the scheme was added successfully.</returns>
        bool TryAddScheme(TransferScheme scheme)
        {
            try
            {
                AddScheme(scheme);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="ITransferService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        void RemoveScheme(string name);

        /// <summary>
        /// Returns the schemes in priority order for request handling.
        /// </summary>
        /// <returns>The schemes in priority order for request handling</returns>
        Task<IEnumerable<TransferScheme>> GetRequestHandlerSchemesAsync();
    }
}
