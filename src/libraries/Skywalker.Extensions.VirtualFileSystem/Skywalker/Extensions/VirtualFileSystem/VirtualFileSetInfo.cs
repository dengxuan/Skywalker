using Microsoft.Extensions.FileProviders;

namespace Skywalker.Extensions.VirtualFileSystem;

public class VirtualFileSetInfo
{
    public IFileProvider FileProvider { get; }

    public VirtualFileSetInfo(IFileProvider fileProvider)
    {
        FileProvider = fileProvider.NotNull(nameof(fileProvider));
    }
}
