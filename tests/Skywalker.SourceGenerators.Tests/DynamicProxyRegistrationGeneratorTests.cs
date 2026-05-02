using Microsoft.CodeAnalysis;
using Skywalker.Extensions.DynamicProxies;
using Skywalker.Extensions.DynamicProxies.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class DynamicProxyRegistrationGeneratorTests
{
    [Fact]
    public void ProducesNoSources_ForNoCandidateServices()
    {
        var result = GeneratorTestHelper.Run<DynamicProxyRegistrationGenerator>("""
            namespace Demo;

            public sealed class PlainService
            {
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void ProducesNoSources_ForInterceptableCandidateDuringScaffolding()
    {
        var result = GeneratorTestHelper.Run<DynamicProxyRegistrationGenerator>("""
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                Task SubmitAsync();
            }

            public sealed class OrderService : IOrderService
            {
                public Task SubmitAsync() => Task.CompletedTask;
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        Assert.Empty(result.Diagnostics);
    }

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        yield return MetadataReference.CreateFromFile(typeof(IInterceptable).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(Task).Assembly.Location);
    }
}