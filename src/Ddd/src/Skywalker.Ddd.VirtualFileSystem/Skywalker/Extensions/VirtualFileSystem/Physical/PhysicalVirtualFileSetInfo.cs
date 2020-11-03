using Microsoft.Extensions.FileProviders;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.VirtualFileSystem.Physical
{
    public class PhysicalVirtualFileSetInfo : VirtualFileSetInfo
    {
        public string Root { get; }

        public PhysicalVirtualFileSetInfo(
            [NotNull] IFileProvider fileProvider,
            [NotNull] string root
            )
            : base(fileProvider)
        {
            Root = Check.NotNullOrWhiteSpace(root, nameof(root));
        }
    }
}