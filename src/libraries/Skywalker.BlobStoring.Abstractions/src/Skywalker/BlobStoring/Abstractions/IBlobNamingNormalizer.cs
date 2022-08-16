namespace Skywalker.BlobStoring.Abstractions;

public interface IBlobNamingNormalizer
{
    string NormalizeContainerName(string containerName);

    string NormalizeBlobName(string blobName);
}
