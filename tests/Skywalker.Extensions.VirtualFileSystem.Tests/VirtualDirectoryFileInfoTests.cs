using Microsoft.Extensions.FileProviders;
using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class VirtualDirectoryFileInfoTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var now = DateTimeOffset.UtcNow;
        var dir = new VirtualDirectoryFileInfo("/my/dir", "dir", now);

        Assert.True(dir.Exists);
        Assert.True(dir.IsDirectory);
        Assert.Equal("dir", dir.Name);
        Assert.Equal("/my/dir", dir.PhysicalPath);
        Assert.Equal(now, dir.LastModified);
        Assert.Equal(0, dir.Length);
    }

    [Fact]
    public void CreateReadStream_ThrowsInvalidOperationException()
    {
        var dir = new VirtualDirectoryFileInfo("/dir", "dir", DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() => dir.CreateReadStream());
    }
}
