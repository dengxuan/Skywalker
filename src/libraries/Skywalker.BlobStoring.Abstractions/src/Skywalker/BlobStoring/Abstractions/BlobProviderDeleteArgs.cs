using System.Threading;


namespace Skywalker.BlobStoring.Abstractions;

public class BlobProviderDeleteArgs : BlobProviderArgs
{
    public BlobProviderDeleteArgs(
         string containerName,
         BlobContainerConfiguration configuration,
         string blobName,
        CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            blobName,
            cancellationToken)
    {
    }
}
