using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class VirtualFileProviderTests
{
    [Fact]
    public void GetFileInfo_DelegatesToHybridProvider()
    {
        var dynamicProvider = new DynamicFileProvider();
        var options = Options.Create(new SkywalkerVirtualFileSystemOptions());
        var provider = new VirtualFileProvider(options, dynamicProvider);

        var result = provider.GetFileInfo("/nonexistent.txt");

        Assert.False(result.Exists);
    }

    [Fact]
    public void GetFileInfo_FindsDynamicFile()
    {
        var dynamicProvider = new DynamicFileProvider();
        dynamicProvider.AddOrUpdate(new InMemoryFileInfo("/test.txt", System.Text.Encoding.UTF8.GetBytes("content"), "test.txt"));

        var options = Options.Create(new SkywalkerVirtualFileSystemOptions());
        var provider = new VirtualFileProvider(options, dynamicProvider);

        var result = provider.GetFileInfo("/test.txt");

        Assert.True(result.Exists);
        Assert.Equal("test.txt", result.Name);
    }

    [Fact]
    public void GetDirectoryContents_EmptySubpath_BecomesRoot()
    {
        var dynamicProvider = new DynamicFileProvider();
        var options = Options.Create(new SkywalkerVirtualFileSystemOptions());
        var provider = new VirtualFileProvider(options, dynamicProvider);

        // Empty subpath is normalized to "/"
        var result = provider.GetDirectoryContents("");

        Assert.NotNull(result);
    }

    [Fact]
    public void Watch_DelegatesToHybridProvider()
    {
        var dynamicProvider = new DynamicFileProvider();
        var options = Options.Create(new SkywalkerVirtualFileSystemOptions());
        var provider = new VirtualFileProvider(options, dynamicProvider);

        var token = provider.Watch("/test.txt");

        Assert.NotNull(token);
    }

    [Fact]
    public void DynamicProvider_HasHigherPriority_ThanFileSets()
    {
        var dynamicProvider = new DynamicFileProvider();
        dynamicProvider.AddOrUpdate(new InMemoryFileInfo("/test.txt", System.Text.Encoding.UTF8.GetBytes("dynamic"), "test.txt"));

        var options = Options.Create(new SkywalkerVirtualFileSystemOptions());
        var provider = new VirtualFileProvider(options, dynamicProvider);

        var result = provider.GetFileInfo("/test.txt");
        Assert.True(result.Exists);

        using var stream = result.CreateReadStream();
        using var reader = new System.IO.StreamReader(stream);
        Assert.Equal("dynamic", reader.ReadToEnd());
    }
}
