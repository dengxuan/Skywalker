using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Localization;
using Skywalker.Localization.AspNetCore;
using Xunit;

namespace Skywalker.Localization.Tests.AspNetCore;

public class SkywalkerStringLocalizerFactoryTests
{
    private class TestResource { }
    private class AnotherResource { }
    private class DefaultResource { }

    private readonly ServiceCollection _services;
    private readonly LocalizationOptions _options;

    public SkywalkerStringLocalizerFactoryTests()
    {
        _services = new ServiceCollection();
        _options = new LocalizationOptions();
        _services.AddSingleton(Options.Create(_options));
    }

    [Fact]
    public void Constructor_ThrowsOnNullServiceProvider()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SkywalkerStringLocalizerFactory(null!, Options.Create(_options)));
    }

    [Fact]
    public void Constructor_ThrowsOnNullOptions()
    {
        var sp = _services.BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() =>
            new SkywalkerStringLocalizerFactory(sp, null!));
    }

    [Fact]
    public void Create_Generic_ReturnsLocalizer()
    {
        _options.Resources.Add<TestResource>();
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer = factory.Create<TestResource>();

        Assert.NotNull(localizer);
        Assert.IsType<SkywalkerStringLocalizer>(localizer);
    }

    [Fact]
    public void Create_ByType_ReturnsLocalizer()
    {
        _options.Resources.Add<TestResource>();
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer = factory.Create(typeof(TestResource));

        Assert.NotNull(localizer);
        Assert.IsType<SkywalkerStringLocalizer>(localizer);
    }

    [Fact]
    public void Create_NotFoundResource_ReturnsNullLocalizer()
    {
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer = factory.Create(typeof(TestResource));

        Assert.Same(NullStringLocalizer.Instance, localizer);
    }

    [Fact]
    public void Create_CachesLocalizer()
    {
        _options.Resources.Add<TestResource>();
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer1 = factory.Create<TestResource>();
        var localizer2 = factory.Create<TestResource>();

        Assert.Same(localizer1, localizer2);
    }

    [Fact]
    public void Create_DifferentTypes_ReturnsDifferentLocalizers()
    {
        _options.Resources.Add<TestResource>();
        _options.Resources.Add<AnotherResource>();
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer1 = factory.Create<TestResource>();
        var localizer2 = factory.Create<AnotherResource>();

        Assert.NotSame(localizer1, localizer2);
    }

    [Fact]
    public void Create_ByBaseName_FindsByResourceName()
    {
        _options.Resources.Add<TestResource>();
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer = factory.Create(typeof(TestResource).FullName!, "");

        Assert.NotNull(localizer);
        Assert.IsType<SkywalkerStringLocalizer>(localizer);
    }

    [Fact]
    public void Create_ByBaseName_NotFound_ReturnsNullLocalizer()
    {
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer = factory.Create("NotExist", "");

        Assert.Same(NullStringLocalizer.Instance, localizer);
    }

    [Fact]
    public void Create_ByBaseName_NotFound_FallsBackToDefaultResource()
    {
        _options.Resources.Add<DefaultResource>();
        _options.DefaultResourceType = typeof(DefaultResource);
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        var localizer = factory.Create("NotExist", "");

        Assert.NotNull(localizer);
        Assert.IsType<SkywalkerStringLocalizer>(localizer);
    }

    [Fact]
    public void Create_InitializesContributors()
    {
        var contributor = Substitute.For<ILocalizationResourceContributor>();
        var resource = _options.Resources.Add<TestResource>();
        resource.Contributors.Add(contributor);
        var sp = _services.BuildServiceProvider();
        var factory = new SkywalkerStringLocalizerFactory(sp, Options.Create(_options));

        factory.Create<TestResource>();

        contributor.Received(1).Initialize(Arg.Any<LocalizationResourceInitializationContext>());
    }
}

