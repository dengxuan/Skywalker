namespace Skywalker.BlobStoring.Abstractions;

public class AbpBlobStoringOptions
{
    public BlobContainerConfigurations Containers { get; }

    public AbpBlobStoringOptions()
    {
        Containers = new BlobContainerConfigurations();
    }
}
