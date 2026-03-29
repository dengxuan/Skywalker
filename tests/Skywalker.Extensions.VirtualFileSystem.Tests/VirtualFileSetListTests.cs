using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class VirtualFileSetListTests
{
    [Fact]
    public void SkywalkerVirtualFileSystemOptions_HasEmptyFileSets()
    {
        var options = new SkywalkerVirtualFileSystemOptions();

        Assert.NotNull(options.FileSets);
        Assert.Empty(options.FileSets);
    }

    [Fact]
    public void SectionName_HasCorrectValue()
    {
        Assert.Equal("Skywalker:VirtualFileSystem", SkywalkerVirtualFileSystemOptions.SectionName);
    }

    [Fact]
    public void VirtualFileSetList_IsListOfVirtualFileSetInfo()
    {
        var list = new VirtualFileSetList();

        Assert.IsType<VirtualFileSetList>(list);
        Assert.Empty(list);
    }
}
