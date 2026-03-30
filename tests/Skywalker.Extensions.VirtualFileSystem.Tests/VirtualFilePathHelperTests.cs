using Skywalker.Extensions.VirtualFileSystem;

namespace Skywalker.Extensions.VirtualFileSystem.Tests;

public class VirtualFilePathHelperTests
{
    [Fact]
    public void NormalizePath_RootPath_ReturnsEmpty()
    {
        var result = InvokeNormalizePath("/");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void NormalizePath_SimpleFile_ReturnsSame()
    {
        var result = InvokeNormalizePath("file.txt");

        Assert.Equal("file.txt", result);
    }

    [Fact]
    public void NormalizePath_FileInFolder_ReplacesDots()
    {
        // "Folder.SubFolder.file.txt" → "Folder/SubFolder/file.txt"
        var result = InvokeNormalizePath("Folder.SubFolder.file.txt");

        Assert.Equal("Folder/SubFolder/file.txt", result);
    }

    [Fact]
    public void NormalizePath_FolderWithHyphens_ReplacesWithUnderscores()
    {
        // "My-Folder.file.txt" → "My_Folder/file.txt"
        var result = InvokeNormalizePath("My-Folder.file.txt");

        Assert.Equal("My_Folder/file.txt", result);
    }

    [Fact]
    public void NormalizePath_NoExtension_ReturnsSame()
    {
        var result = InvokeNormalizePath("foldername");

        Assert.Equal("foldername", result);
    }

    [Fact]
    public void NormalizePath_DirectoryPathWithDot_ReplacesDotWithSlash()
    {
        // "some.folder/path" - dots are treated as directory separators
        var result = InvokeNormalizePath("some.folder/path");

        Assert.Equal("some/folder/path", result);
    }

    [Fact]
    public void NormalizePath_PreservesFileExtension()
    {
        var result = InvokeNormalizePath("Namespace.Class.cs");

        Assert.Equal("Namespace/Class.cs", result);
    }

    /// <summary>
    /// Use reflection to access internal static VirtualFilePathHelper.NormalizePath
    /// </summary>
    private static string InvokeNormalizePath(string path)
    {
        var type = typeof(DynamicFileProvider).Assembly.GetType("Skywalker.Extensions.VirtualFileSystem.VirtualFilePathHelper");
        Assert.NotNull(type);
        var method = type!.GetMethod("NormalizePath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(method);
        return (string)method!.Invoke(null, new object[] { path })!;
    }
}
