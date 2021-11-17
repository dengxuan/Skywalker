using Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Skywalker.Extensions.AspNetCore.Security.Abstractions
{
    public class EncryptionProviderFactory: IEncryptionProvider
    {
        public EncryptionProviderFactory(Type providerType)
        {
            ProviderType = providerType;
        }

        private Type ProviderType { get; }

        public IEncryptionProvider CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return (IEncryptionProvider)ActivatorUtilities.CreateInstance(serviceProvider, ProviderType, Type.EmptyTypes);
        }

        string IEncryptionProvider.EncodingName
        {
            get { throw new NotSupportedException(); }
        }

        bool IEncryptionProvider.SupportsFlush
        {
            get { throw new NotSupportedException(); }
        }

        Stream IEncryptionProvider.CreateStream(Stream outputStream)
        {
            throw new NotSupportedException();
        }
    }
}
