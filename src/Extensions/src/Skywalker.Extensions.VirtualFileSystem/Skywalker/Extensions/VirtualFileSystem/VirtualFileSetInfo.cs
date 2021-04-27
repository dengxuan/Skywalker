using Microsoft.Extensions.FileProviders;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Extensions.VirtualFileSystem
{
    public class VirtualFileSetInfo
    {
        public IFileProvider FileProvider { get; }

        public VirtualFileSetInfo([NotNull] IFileProvider fileProvider)
        {
            FileProvider = Check.NotNull(fileProvider, nameof(fileProvider));
        }
    }
}