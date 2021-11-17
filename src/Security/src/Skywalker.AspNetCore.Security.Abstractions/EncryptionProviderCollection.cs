﻿using Abstractions;
using System;
using System.Collections.ObjectModel;

namespace Skywalker.Extensions.AspNetCore.Security.Abstractions
{
    public class EncryptionProviderCollection : Collection<IEncryptionProvider>
    {
        /// <summary>
        /// Adds a type representing an <see cref="IEncryptionProvider"/>.
        /// </summary>
        /// <remarks>
        /// Provider instances will be created using an <see cref="IServiceProvider" />.
        /// </remarks>
        public void Add<TEncryptionProvider>() where TEncryptionProvider : IEncryptionProvider
        {
            Add(typeof(TEncryptionProvider));
        }

        /// <summary>
        /// Adds a type representing an <see cref="IEncryptionProvider"/>.
        /// </summary>
        /// <param name="providerType">Type representing an <see cref="IEncryptionProvider"/>.</param>
        /// <remarks>
        /// Provider instances will be created using an <see cref="IServiceProvider" />.
        /// </remarks>
        public void Add(Type providerType)
        {
            if (providerType == null)
            {
                throw new ArgumentNullException(nameof(providerType));
            }

            if (!typeof(IEncryptionProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException($"The provider must implement {nameof(IEncryptionProvider)}.", nameof(providerType));
            }

            var factory = new EncryptionProviderFactory(providerType);
            Add(factory);
        }
    }
}