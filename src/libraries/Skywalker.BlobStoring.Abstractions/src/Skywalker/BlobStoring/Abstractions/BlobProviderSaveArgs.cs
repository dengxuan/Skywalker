using System.IO;
using System.Threading;


namespace Skywalker.BlobStoring.Abstractions;

public class BlobProviderSaveArgs : BlobProviderArgs
{
    
    public Stream BlobStream { get; }

    public bool OverrideExisting { get; }

    public BlobProviderSaveArgs(
         string containerName,
         BlobContainerConfiguration configuration,
         string blobName,
         Stream blobStream,
        bool overrideExisting = false,
        CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            blobName,
            cancellationToken)
    {
        BlobStream = Check.NotNull(blobStream, nameof(blobStream));
        OverrideExisting = overrideExisting;
    }
}
