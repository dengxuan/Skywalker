namespace Skywalker.BlobStoring.Abstractions;

public class BlobNormalizeNaming
{
    public string ContainerName { get; }

    public string BlobName { get; }

    public BlobNormalizeNaming(string? containerName, string? blobName)
    {
        ContainerName = containerName ?? string.Empty;
        BlobName = blobName ?? string.Empty;
    }
}
