using Microsoft.Extensions.FileProviders;

namespace Volo.Abp.VirtualFileSystem;

public class VirtualFileSetInfo
{
    public IFileProvider FileProvider { get; }

    public VirtualFileSetInfo(IFileProvider fileProvider)
    {
        FileProvider = Check.NotNull(fileProvider, nameof(fileProvider));
    }
}
