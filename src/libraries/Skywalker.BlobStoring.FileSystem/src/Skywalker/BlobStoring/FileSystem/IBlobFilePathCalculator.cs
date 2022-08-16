using Skywalker.BlobStoring.Abstractions;

namespace Skywalker.BlobStoring.FileSystem;

public interface IBlobFilePathCalculator
{
    string Calculate(BlobProviderArgs args);
}
