using System.Text;
using Microsoft.Extensions.FileProviders;
using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class DynamicFileProviderTests
{
    private readonly DynamicFileProvider _provider;

    public DynamicFileProviderTests()
    {
        _provider = new DynamicFileProvider();
    }

    [Fact]
    public void GetFileInfo_ReturnsNotFound_WhenFileDoesNotExist()
    {
        var result = _provider.GetFileInfo("/nonexistent.txt");

        Assert.False(result.Exists);
    }

    [Fact]
    public void AddOrUpdate_ThenGetFileInfo_ReturnsFile()
    {
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("hello"), "test.txt");

        _provider.AddOrUpdate(fileInfo);

        var result = _provider.GetFileInfo("/test.txt");
        Assert.True(result.Exists);
        Assert.Equal("test.txt", result.Name);
    }

    [Fact]
    public void AddOrUpdate_OverwritesExistingFile()
    {
        var file1 = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("v1"), "test.txt");
        var file2 = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("v2"), "test.txt");

        _provider.AddOrUpdate(file1);
        _provider.AddOrUpdate(file2);

        var result = _provider.GetFileInfo("/test.txt");
        using var stream = result.CreateReadStream();
        using var reader = new StreamReader(stream);
        Assert.Equal("v2", reader.ReadToEnd());
    }

    [Fact]
    public void Delete_RemovesFile()
    {
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("hello"), "test.txt");
        _provider.AddOrUpdate(fileInfo);

        var deleted = _provider.Delete("/test.txt");

        Assert.True(deleted);
        Assert.False(_provider.GetFileInfo("/test.txt").Exists);
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenFileDoesNotExist()
    {
        var deleted = _provider.Delete("/nonexistent.txt");

        Assert.False(deleted);
    }

    [Fact]
    public void Watch_ReturnsChangeToken_ThatTriggersOnAddOrUpdate()
    {
        var token = _provider.Watch("/test.txt");

        Assert.False(token.HasChanged);

        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("hello"), "test.txt");
        _provider.AddOrUpdate(fileInfo);

        Assert.True(token.HasChanged);
    }

    [Fact]
    public void Watch_ReturnsChangeToken_ThatTriggersOnDelete()
    {
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("hello"), "test.txt");
        _provider.AddOrUpdate(fileInfo);

        var token = _provider.Watch("/test.txt");
        Assert.False(token.HasChanged);

        _provider.Delete("/test.txt");

        Assert.True(token.HasChanged);
    }

    [Fact]
    public void Watch_ReturnsNewToken_AfterChange()
    {
        var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes("hello"), "test.txt");
        _provider.AddOrUpdate(fileInfo);

        var token1 = _provider.Watch("/test.txt");
        _provider.AddOrUpdate(fileInfo); // triggers token1

        var token2 = _provider.Watch("/test.txt");
        Assert.False(token2.HasChanged);
    }

    [Fact]
    public void GetDirectoryContents_ReturnsNotFound_WhenNotADirectory()
    {
        var result = _provider.GetDirectoryContents("/nonexistent");

        Assert.False(result.Exists);
    }

    [Fact]
    public void AddOrUpdate_MultipleTimes_DoesNotThrow()
    {
        for (int i = 0; i < 100; i++)
        {
            var fileInfo = new InMemoryFileInfo("/test.txt", Encoding.UTF8.GetBytes($"version {i}"), "test.txt");
            _provider.AddOrUpdate(fileInfo);
        }

        var result = _provider.GetFileInfo("/test.txt");
        Assert.True(result.Exists);
    }
}
