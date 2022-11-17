using Microsoft.Extensions.DependencyInjection;
using Skywalker.AspNetCore.Security.Abstractions;

namespace Skywalker.AspNetCore.Security;

public class EncryptionProviderFactory : IEncryptionProvider
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
