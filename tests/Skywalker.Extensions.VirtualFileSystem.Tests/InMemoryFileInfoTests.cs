using System.Text;
using Microsoft.Extensions.FileProviders;
using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class InMemoryFileInfoTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var content = Encoding.UTF8.GetBytes("test content");
        var fileInfo = new InMemoryFileInfo("/path/test.txt", content, "test.txt");

        Assert.True(fileInfo.Exists);
        Assert.Equal("test.txt", fileInfo.Name);
        Assert.Equal("/path/test.txt", fileInfo.DynamicPath);
        Assert.Null(fileInfo.PhysicalPath);
        Assert.False(fileInfo.IsDirectory);
        Assert.Equal(content.Length, fileInfo.Length);
    }

    [Fact]
    public void CreateReadStream_ReturnsFileContent()
    {
        var content = "hello world";
        var bytes = Encoding.UTF8.GetBytes(content);
        var fileInfo = new InMemoryFileInfo("/test.txt", bytes, "test.txt");

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();

        Assert.Equal(content, result);
    }

    [Fact]
    public void CreateReadStream_ReturnsReadOnlyStream()
    {
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("data"), "test.txt");

        using var stream = fileInfo.CreateReadStream();

        Assert.False(stream.CanWrite);
    }

    [Fact]
    public void LastModified_IsRecentTimestamp()
    {
        var before = DateTimeOffset.Now.AddSeconds(-1);
        var fileInfo = new InMemoryFileInfo("/test.txt", Array.Empty<byte>(), "test.txt");
        var after = DateTimeOffset.Now.AddSeconds(1);

        Assert.InRange(fileInfo.LastModified, before, after);
    }

    [Fact]
    public void EmptyContent_HasZeroLength()
    {
        var fileInfo = new InMemoryFileInfo("/test.txt", Array.Empty<byte>(), "test.txt");

        Assert.Equal(0, fileInfo.Length);
    }
}
