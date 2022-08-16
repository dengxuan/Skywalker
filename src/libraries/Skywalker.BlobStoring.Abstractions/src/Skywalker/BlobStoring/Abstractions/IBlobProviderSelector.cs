

namespace Skywalker.BlobStoring.Abstractions;

public interface IBlobProviderSelector
{
    
    IBlobProvider Get( string containerName);
}
