using Microsoft.Extensions.FileProviders;

namespace Skywalker.Extensions.VirtualFileSystem.Physical;

public class PhysicalVirtualFileSetInfo : VirtualFileSetInfo
{
    public string Root { get; }

    public PhysicalVirtualFileSetInfo(IFileProvider fileProvider, string root) : base(fileProvider)
    {
        Root = root.NotNullOrWhiteSpace(nameof(root));
    }
}
