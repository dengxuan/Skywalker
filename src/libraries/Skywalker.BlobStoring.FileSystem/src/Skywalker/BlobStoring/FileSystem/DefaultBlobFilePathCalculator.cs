using Skywalker.BlobStoring.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.BlobStoring.FileSystem;

public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator,
    ITransientDependency
{
    public virtual string Calculate(BlobProviderArgs args)
    {
        var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
        var blobPath = fileSystemConfiguration.BasePath;

            blobPath = Path.Combine(blobPath, "host");
        

        if (fileSystemConfiguration.AppendContainerNameToBasePath)
        {
            blobPath = Path.Combine(blobPath, args.ContainerName);
        }

        blobPath = Path.Combine(blobPath, args.BlobName);

        return blobPath;
    }
}
