using Skywalker.ExceptionHandler;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.BlobStoring.Abstractions;

public class DefaultBlobProviderSelector : IBlobProviderSelector, ITransientDependency
{
    protected IEnumerable<IBlobProvider> BlobProviders { get; }

    protected IBlobContainerConfigurationProvider ConfigurationProvider { get; }

    public DefaultBlobProviderSelector(
        IBlobContainerConfigurationProvider configurationProvider,
        IEnumerable<IBlobProvider> blobProviders)
    {
        ConfigurationProvider = configurationProvider;
        BlobProviders = blobProviders;
    }


    public virtual IBlobProvider Get(string containerName)
    {
        Check.NotNull(containerName, nameof(containerName));

        var configuration = ConfigurationProvider.Get(containerName);

        if (!BlobProviders.Any())
        {
            throw new SkywalkerException("No BLOB Storage provider was registered! At least one provider must be registered to be able to use the BLOB Storing System.");
        }

        if (configuration.ProviderType == null)
        {
            throw new SkywalkerException("No BLOB Storage provider was used! At least one provider must be configured to be able to use the BLOB Storing System.");
        }

        foreach (var provider in BlobProviders)
        {
            if (provider.GetType().IsAssignableTo(configuration.ProviderType))
            {
                return provider;
            }
        }

        throw new SkywalkerException(
            $"Could not find the BLOB Storage provider with the type ({configuration.ProviderType.AssemblyQualifiedName}) configured for the container {containerName} and no default provider was set."
        );
    }
}
