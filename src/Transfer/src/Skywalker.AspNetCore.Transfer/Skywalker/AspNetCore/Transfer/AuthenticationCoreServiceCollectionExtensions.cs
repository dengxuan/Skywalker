// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore.Transfer;
using Skywalker.AspNetCore.Transfer.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up authentication services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class AuthenticationCoreServiceCollectionExtensions
    {
        /// <summary>
        /// Add core authentication services needed for <see cref="ITransferService"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddTransferCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddScoped<ITransferService, AuthenticationService>();
            services.TryAddSingleton<IClaimsTransformation, NoopClaimsTransformation>(); // Can be replaced with scoped ones that use DbContext
            services.TryAddScoped<ITransferHandlerProvider, AuthenticationHandlerProvider>();
            services.TryAddSingleton<ITransferSchemeProvider, AuthenticationSchemeProvider>();
            return services;
        }

        /// <summary>
        /// Add core authentication services needed for <see cref="ITransferService"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">Used to configure the <see cref="TransferOptions"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddTransferCore(this IServiceCollection services, Action<TransferOptions> configureOptions) {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddTransferCore();
            services.Configure(configureOptions);
            return services;
        }
    }
}
