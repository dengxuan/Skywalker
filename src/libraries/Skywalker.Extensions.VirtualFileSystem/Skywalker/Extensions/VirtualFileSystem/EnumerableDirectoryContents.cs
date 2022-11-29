using System.Collections;
using Microsoft.Extensions.FileProviders;

namespace Volo.Abp.VirtualFileSystem;

internal class EnumerableDirectoryContents : IDirectoryContents
{
    private readonly IEnumerable<IFileInfo> _entries;

    public EnumerableDirectoryContents(IEnumerable<IFileInfo> entries)
    {
        Check.NotNull(entries, nameof(entries));

        _entries = entries;
    }

    public bool Exists => true;

    public IEnumerator<IFileInfo> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _entries.GetEnumerator();
    }
}
