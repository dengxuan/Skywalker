// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.Transfer;
using Skywalker.AspNetCore.Transfer.Abstractions;

namespace Skywalker.AspNetCore.Transfer
{
    /// <summary>
    /// Implements <see cref="ITransferSchemeProvider"/>.
    /// </summary>
    public class AuthenticationSchemeProvider : ITransferSchemeProvider
    {
        /// <summary>
        /// Creates an instance of <see cref="AuthenticationSchemeProvider"/>
        /// using the specified <paramref name="options"/>,
        /// </summary>
        /// <param name="options">The <see cref="TransferOptions"/> options.</param>
        public AuthenticationSchemeProvider(IOptions<TransferOptions> options)
            : this(options, new Dictionary<string, TransferScheme>(StringComparer.Ordinal))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AuthenticationSchemeProvider"/>
        /// using the specified <paramref name="options"/> and <paramref name="schemes"/>.
        /// </summary>
        /// <param name="options">The <see cref="TransferOptions"/> options.</param>
        /// <param name="schemes">The dictionary used to store authentication schemes.</param>
        protected AuthenticationSchemeProvider(IOptions<TransferOptions> options, IDictionary<string, TransferScheme> schemes)
        {
            _options = options.Value;

            _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _requestHandlers = new List<TransferScheme>();

            foreach (var builder in _options.Schemes)
            {
                var scheme = builder.Build();
                AddScheme(scheme);
            }
        }

        private readonly TransferOptions _options;
        private readonly object _lock = new object();

        private readonly IDictionary<string, TransferScheme> _schemes;
        private readonly List<TransferScheme> _requestHandlers;
        // Used as a safe return value for enumeration apis
        private IEnumerable<TransferScheme> _schemesCopy = Array.Empty<TransferScheme>();
        private IEnumerable<TransferScheme> _requestHandlersCopy = Array.Empty<TransferScheme>();

        private Task<TransferScheme?> GetDefaultSchemeAsync()
            => _options.DefaultScheme != null
            ? GetSchemeAsync(_options.DefaultScheme)
            : Task.FromResult<TransferScheme?>(null);

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.TransferAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultAuthenticateScheme"/>.
        /// Otherwise, this will fallback to <see cref="TransferOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.TransferAsync(HttpContext, string)"/>.</returns>
        public virtual Task<TransferScheme?> GetDefaultAuthenticateSchemeAsync()
            => _options.DefaultAuthenticateScheme != null
            ? GetSchemeAsync(_options.DefaultAuthenticateScheme)
            : GetDefaultSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.ChallengeAsync(HttpContext, string, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultChallengeScheme"/>.
        /// Otherwise, this will fallback to <see cref="TransferOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.ChallengeAsync(HttpContext, string, TransferProperties)"/>.</returns>
        public virtual Task<TransferScheme?> GetDefaultChallengeSchemeAsync()
            => _options.DefaultChallengeScheme != null
            ? GetSchemeAsync(_options.DefaultChallengeScheme)
            : GetDefaultSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.ForbidAsync(HttpContext, string, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultForbidScheme"/>.
        /// Otherwise, this will fallback to <see cref="GetDefaultChallengeSchemeAsync"/> .
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.ForbidAsync(HttpContext, string, TransferProperties)"/>.</returns>
        public virtual Task<TransferScheme?> GetDefaultForbidSchemeAsync()
            => _options.DefaultForbidScheme != null
            ? GetSchemeAsync(_options.DefaultForbidScheme)
            : GetDefaultChallengeSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultSignInScheme"/>.
        /// Otherwise, this will fallback to <see cref="TransferOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.SignInAsync(HttpContext, string, System.Security.Claims.ClaimsPrincipal, TransferProperties)"/>.</returns>
        public virtual Task<TransferScheme?> GetDefaultSignInSchemeAsync()
            => _options.DefaultSignInScheme != null
            ? GetSchemeAsync(_options.DefaultSignInScheme)
            : GetDefaultSchemeAsync();

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="ITransferService.SignOutAsync(HttpContext, string, TransferProperties)"/>.
        /// This is typically specified via <see cref="TransferOptions.DefaultSignOutScheme"/>.
        /// Otherwise this will fallback to <see cref="GetDefaultSignInSchemeAsync"/> if that supports sign out.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="ITransferService.SignOutAsync(HttpContext, string, TransferProperties)"/>.</returns>
        public virtual Task<TransferScheme?> GetDefaultSignOutSchemeAsync()
            => _options.DefaultSignOutScheme != null
            ? GetSchemeAsync(_options.DefaultSignOutScheme)
            : GetDefaultSignInSchemeAsync();

        /// <summary>
        /// Returns the <see cref="TransferScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        public virtual Task<TransferScheme?> GetSchemeAsync(string name)
            => Task.FromResult(_schemes.ContainsKey(name) ? _schemes[name] : null);

        /// <summary>
        /// Returns the schemes in priority order for request handling.
        /// </summary>
        /// <returns>The schemes in priority order for request handling</returns>
        public virtual Task<IEnumerable<TransferScheme>> GetRequestHandlerSchemesAsync()
            => Task.FromResult(_requestHandlersCopy);

        /// <summary>
        /// Registers a scheme for use by <see cref="ITransferService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns>true if the scheme was added successfully.</returns>
        public virtual bool TryAddScheme(TransferScheme scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                return false;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(scheme.Name))
                {
                    return false;
                }
                if (typeof(ITransferRequestHandler).IsAssignableFrom(scheme.HandlerType))
                {
                    _requestHandlers.Add(scheme);
                    _requestHandlersCopy = _requestHandlers.ToArray();
                }
                _schemes[scheme.Name] = scheme;
                _schemesCopy = _schemes.Values.ToArray();
                return true;
            }
        }

        /// <summary>
        /// Registers a scheme for use by <see cref="ITransferService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public virtual void AddScheme(TransferScheme scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
            }
            lock (_lock)
            {
                if (!TryAddScheme(scheme))
                {
                    throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
                }
            }
        }

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="ITransferService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        public virtual void RemoveScheme(string name)
        {
            if (!_schemes.ContainsKey(name))
            {
                return;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(name))
                {
                    var scheme = _schemes[name];
                    if (_requestHandlers.Remove(scheme))
                    {
                        _requestHandlersCopy = _requestHandlers.ToArray();
                    }
                    _schemes.Remove(name);
                    _schemesCopy = _schemes.Values.ToArray();
                }
            }
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<TransferScheme>> GetAllSchemesAsync()
            => Task.FromResult(_schemesCopy);
    }
}
