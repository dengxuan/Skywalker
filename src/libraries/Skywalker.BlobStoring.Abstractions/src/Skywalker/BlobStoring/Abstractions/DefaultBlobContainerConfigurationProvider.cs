using Microsoft.Extensions.Options;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.BlobStoring.Abstractions;

public class DefaultBlobContainerConfigurationProvider : IBlobContainerConfigurationProvider,
    ITransientDependency
{
    protected AbpBlobStoringOptions Options { get; }

    public DefaultBlobContainerConfigurationProvider(IOptions<AbpBlobStoringOptions> options)
    {
        Options = options.Value;
    }

    public virtual BlobContainerConfiguration Get(string name)
    {
        return Options.Containers.GetConfiguration(name);
    }
}
