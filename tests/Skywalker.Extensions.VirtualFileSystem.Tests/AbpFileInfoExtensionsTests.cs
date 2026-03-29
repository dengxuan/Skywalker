using System.Text;
using Microsoft.Extensions.FileProviders;
using Skywalker.Extensions.VirtualFileSystem;
using Skywalker.Extensions.VirtualFileSystem.Embedded;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class AbpFileInfoExtensionsTests
{
    [Fact]
    public void ReadAsString_ReturnsUtf8Content()
    {
        var content = "Hello, 世界!";
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes(content), "test.txt");

        var result = fileInfo.ReadAsString();

        Assert.Equal(content, result);
    }

    [Fact]
    public void ReadAsString_WithEncoding_ReturnsCorrectContent()
    {
        var content = "test data";
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.ASCII.GetBytes(content), "test.txt");

        var result = fileInfo.ReadAsString(Encoding.ASCII);

        Assert.Equal(content, result);
    }

    [Fact]
    public async Task ReadAsStringAsync_ReturnsContent()
    {
        var content = "async content";
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes(content), "test.txt");

        var result = await fileInfo.ReadAsStringAsync();

        Assert.Equal(content, result);
    }

    [Fact]
    public void ReadBytes_ReturnsFileBytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var fileInfo = new InMemoryFileInfo("/test.bin", bytes, "test.bin");

        var result = fileInfo.ReadBytes();

        Assert.Equal(bytes, result);
    }

    [Fact]
    public async Task ReadBytesAsync_ReturnsFileBytes()
    {
        var bytes = new byte[] { 10, 20, 30 };
        var fileInfo = new InMemoryFileInfo("/test.bin", bytes, "test.bin");

        var result = await fileInfo.ReadBytesAsync();

        Assert.Equal(bytes, result);
    }

    [Fact]
    public void GetVirtualOrPhysicalPathOrNull_InMemoryFileInfo_ReturnsDynamicPath()
    {
        var fileInfo = new InMemoryFileInfo("/my/path.txt", Array.Empty<byte>(), "path.txt");

        var result = fileInfo.GetVirtualOrPhysicalPathOrNull();

        Assert.Equal("/my/path.txt", result);
    }

    [Fact]
    public void GetVirtualOrPhysicalPathOrNull_EmbeddedResourceFileInfo_ReturnsVirtualPath()
    {
        var assembly = typeof(AbpFileInfoExtensionsTests).Assembly;
        var fileInfo = new EmbeddedResourceFileInfo(
            assembly,
            "SomeResource",
            "/virtual/path.txt",
            "path.txt",
            DateTimeOffset.UtcNow);

        var result = fileInfo.GetVirtualOrPhysicalPathOrNull();

        Assert.Equal("/virtual/path.txt", result);
    }
}
