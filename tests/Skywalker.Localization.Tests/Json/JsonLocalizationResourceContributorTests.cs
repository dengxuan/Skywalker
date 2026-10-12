using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NSubstitute;
using Skywalker.Localization;
using Skywalker.Localization.Json;
using Xunit;

namespace Skywalker.Localization.Tests.Json;

public class JsonLocalizationResourceContributorTests
{
    private class TestResource { }

    [Fact]
    public void Constructor_ThrowsOnNullVirtualPath()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new JsonLocalizationResourceContributor(null!));
    }

    [Fact]
    public void Initialize_WithoutIFileProvider_UsesHostContentRoot()
    {
        // ASP.NET Core 不注册裸 IFileProvider，只有 IHostEnvironment.ContentRootFileProvider；原实现这里静默加载 0 条
        var root = Directory.CreateTempSubdirectory("skywalker-loc-");
        try
        {
            var dir = Directory.CreateDirectory(Path.Combine(root.FullName, "Localization", "TestResource"));
            File.WriteAllText(Path.Combine(dir.FullName, "zh-CN.json"), """{"Hello": "你好"}""", Encoding.UTF8);

            var env = Substitute.For<Microsoft.Extensions.Hosting.IHostEnvironment>();
            env.ContentRootFileProvider.Returns(new PhysicalFileProvider(root.FullName));
            var services = new ServiceCollection();
            services.AddSingleton(env);
            var sp = services.BuildServiceProvider();

            var contributor = new JsonLocalizationResourceContributor("/Localization/TestResource");
            contributor.Initialize(new LocalizationResourceInitializationContext(new LocalizationResource(typeof(TestResource)), sp));

            Assert.Equal("你好", contributor.GetOrNull("zh-CN", "Hello")?.Value);
        }
        finally
        {
            root.Delete(recursive: true);
        }
    }

    [Fact]
    public void GetOrNull_BeforeInitialize_ReturnsNull()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");

        var result = contributor.GetOrNull("en", "Hello");

        Assert.Null(result);
    }

    [Fact]
    public void GetOrNull_AfterInitialize_ReturnsValue()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Hello": "Hello"}""" },
            { "zh-CN.json", """{"Hello": "你好"}""" }
        });

        contributor.Initialize(context);
        var result = contributor.GetOrNull("zh-CN", "Hello");

        Assert.NotNull(result);
        Assert.Equal("Hello", result.Name);
        Assert.Equal("你好", result.Value);
    }

    [Fact]
    public void GetOrNull_NotFound_ReturnsNull()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Hello": "Hello"}""" }
        });

        contributor.Initialize(context);
        var result = contributor.GetOrNull("en", "NotExist");

        Assert.Null(result);
    }

    [Fact]
    public void GetOrNull_CultureNotFound_ReturnsNull()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Hello": "Hello"}""" }
        });

        contributor.Initialize(context);
        var result = contributor.GetOrNull("fr", "Hello");

        Assert.Null(result);
    }

    [Fact]
    public void Fill_PopulatesDictionary()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Hello": "Hello", "Goodbye": "Goodbye"}""" }
        });

        contributor.Initialize(context);
        var dictionary = new Dictionary<string, LocalizedString>();
        contributor.Fill("en", dictionary);

        Assert.Equal(2, dictionary.Count);
        Assert.Equal("Hello", dictionary["Hello"].Value);
        Assert.Equal("Goodbye", dictionary["Goodbye"].Value);
    }

    [Fact]
    public void GetSupportedCultures_ReturnsCultures()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Hello": "Hello"}""" },
            { "zh-CN.json", """{"Hello": "你好"}""" },
            { "ja.json", """{"Hello": "こんにちは"}""" }
        });

        contributor.Initialize(context);
        var cultures = contributor.GetSupportedCultures().ToList();

        Assert.Equal(3, cultures.Count);
        Assert.Contains("en", cultures);
        Assert.Contains("zh-CN", cultures);
        Assert.Contains("ja", cultures);
    }

    [Fact]
    public void Initialize_IgnoresNonJsonFiles()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Hello": "Hello"}""" },
            { "readme.txt", "This is not a JSON file" }
        });

        contributor.Initialize(context);
        var cultures = contributor.GetSupportedCultures().ToList();

        Assert.Single(cultures);
        Assert.Contains("en", cultures);
    }

    [Fact]
    public void Initialize_HandlesNestedJson()
    {
        var contributor = new JsonLocalizationResourceContributor("/Localization");
        var context = CreateContext(new Dictionary<string, string>
        {
            { "en.json", """{"Menu": {"File": "File", "Edit": "Edit"}}""" }
        });

        contributor.Initialize(context);
        var result = contributor.GetOrNull("en", "Menu:File");

        Assert.NotNull(result);
        Assert.Equal("File", result.Value);
    }

    private static LocalizationResourceInitializationContext CreateContext(
        Dictionary<string, string> files)
    {
        var services = new ServiceCollection();
        var fileProvider = CreateMockFileProvider(files);
        services.AddSingleton<IFileProvider>(fileProvider);

        var resource = new LocalizationResource(typeof(TestResource));
        return new LocalizationResourceInitializationContext(resource, services.BuildServiceProvider());
    }

    private static IFileProvider CreateMockFileProvider(Dictionary<string, string> files)
    {
        var fileProvider = Substitute.For<IFileProvider>();
        var fileInfos = new List<IFileInfo>();

        foreach (var (name, content) in files)
        {
            var fileInfo = Substitute.For<IFileInfo>();
            fileInfo.Name.Returns(name);
            fileInfo.IsDirectory.Returns(false);
            fileInfo.CreateReadStream().Returns(_ =>
                new MemoryStream(Encoding.UTF8.GetBytes(content)));
            fileInfos.Add(fileInfo);
        }

        var directoryContents = Substitute.For<IDirectoryContents>();
        directoryContents.Exists.Returns(true);
        directoryContents.GetEnumerator().Returns(_ => fileInfos.GetEnumerator());

        fileProvider.GetDirectoryContents(Arg.Any<string>()).Returns(directoryContents);
        return fileProvider;
    }
}

